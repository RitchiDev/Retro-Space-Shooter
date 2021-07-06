using System.Collections;
using UnityEngine;

public enum MovementType
{ 
	straight = 0,
	random,
}

public class EnemyController : MonoBehaviour
{
	[SerializeField] private MovementType m_MovementType;
	[SerializeField] private Vector2 m_BaseMoveLimit;
	[SerializeField] private float m_MovementSpeed;
	[SerializeField] private float m_BulletSpeed = 5;

	public GameObject powerUp;
	//public GameObject explosion;
	public GameObject bullet;
	public float minReloadTime = 1.0f;
	public float maxReloadTime = 2.0f;

	private Rigidbody2D m_Rigidbody;

	private float m_DirectionChangeTimer = 0.0f;
	private float m_RandomX;
	private float m_RandomY;

	private void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		StartCoroutine(Shoot());
	}

	private void OnEnable()
	{
		if(m_MovementType == MovementType.straight)
		{
			m_Rigidbody.AddForce(Vector2.down * m_MovementSpeed, ForceMode2D.Impulse);
		}
	}

	private void FixedUpdate()
	{
		if (m_MovementType == MovementType.random)
		{
			if (Time.fixedTime >= m_DirectionChangeTimer)
			{
				m_RandomX = Random.Range(-2.0f, 2.0f);
				m_RandomY = Random.Range(-2.0f, 2.0f);

				m_DirectionChangeTimer = Time.fixedTime + Random.Range(0.5f, 1.5f);
			}

			Vector2 newDirection = new Vector2(m_RandomX, m_RandomY);
			m_Rigidbody.transform.Translate(newDirection * (m_MovementSpeed * 0.4f) * Time.fixedDeltaTime);

			if(!BoundaryManager.Instance.WithinBoundaryX(m_Rigidbody.position.x))
			{
				m_RandomX = -m_RandomX;
			}

			if (!BoundaryManager.Instance.WithinBoundaryY(m_Rigidbody.position.y))
			{
				m_RandomY = -m_RandomY;
			}

			m_Rigidbody.position = BoundaryManager.Instance.Clamp(m_Rigidbody.position);
		}
	}

	private IEnumerator Shoot()
	{
		yield return new WaitForSeconds((Random.Range(minReloadTime, maxReloadTime)));
		while (true)
		{
			GameObject bullet = ObjectPooler.m_Instance.SetActiveFromPool(WhichPrefab.enemyBullet, transform.position, transform.rotation);
			bullet.GetComponent<Rigidbody2D>().AddForce(Vector2.down * m_BulletSpeed, ForceMode2D.Impulse);
			yield return new WaitForSeconds((Random.Range(minReloadTime, maxReloadTime)));
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.GetComponent<Bullet>()) //Enemy projectile staat op een andere layer
		{
			int randomNumber = Random.Range(0, 10);
			if(randomNumber > 9)
			{
				ObjectPooler.m_Instance.SetActiveFromPool(WhichPrefab.powerUp, m_Rigidbody.position, Quaternion.identity);
			}

			ObjectPooler.m_Instance.SetActiveFromPool(WhichPrefab.enemyExplosion, m_Rigidbody.position, Quaternion.identity);

			gameObject.SetActive(false);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		Boundary boundary = collision.GetComponent<Boundary>();
		if(boundary != null)
		{
			if(boundary.location == Boundary.BoundaryLocation.BOTTOM)
			{
				gameObject.SetActive(false);
			}
		}
	}
}
