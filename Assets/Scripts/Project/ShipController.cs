using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
	[Header("Movement:")]
	[SerializeField] private float m_MovementSpeed = 7f; // The players movement speed
	[SerializeField] private Vector2 m_BaseMoveLimit;
	private Vector2 m_MovementInput;

	[Header("Weapons:")]
	public GameObject        playerBullet;                        // Reference to the players bullet prefab
	public GameObject        startWeapon;                         // The players initial 'turret' gameobject
	public List<GameObject>  tripleShotTurrets;                   //
	public List<GameObject>  wideShotTurrets;                     // References to the upgrade weapon turrets
	public List<GameObject>  scatterShotTurrets;                  //
	public List<GameObject>  activePlayerTurrets;                 //
	public float             scatterShotTurretReloadTime = 2.0f;  // Reload time for the scatter shot turret!

	[SerializeField] private float m_ShootDelay = 3;
	[SerializeField] private float m_BulletSpeed = 3;
	private bool m_IsShooting;
	private bool m_AllowShooting;

	[Header("Effects:")]
	//public GameObject        explosion;                           // Reference to the Explosion prefab
	public ParticleSystem    playerThrust;                        // The particle effect for the ships thruster

	[Header("Debug:")]
	public bool              godMode                     = false; // Set to true to enable god mode (no game over)
	public int               upgradeState                = 0;     // A reference to the upgrade state of the player
	
	// private stuff
	private Rigidbody2D      playerRigidbody;                     // The players rigidbody: Required to apply directional force to move the player
	private Renderer         playerRenderer;                      // The Renderer for the players ship sprite
	private CircleCollider2D playerCollider;                      // The Players ship collider
	private AudioSource      shootSoundFX;                        // The player shooting sound effect

	void Start()
	{
		playerCollider      = gameObject.GetComponent<CircleCollider2D>();
		playerRenderer      = gameObject.GetComponent<Renderer>();
        activePlayerTurrets = new List<GameObject>{ startWeapon };
        shootSoundFX        = gameObject.GetComponent<AudioSource>();
		playerRigidbody     = GetComponent<Rigidbody2D>();
		m_AllowShooting = true;
	}

	private void FixedUpdate()
	{
		Vector2 moveLimit = m_BaseMoveLimit - (Vector2)(playerCollider.bounds.size) * 0.5f;
		Vector2 position = playerRigidbody.position + (m_MovementInput * m_MovementSpeed * Time.fixedDeltaTime);
		position.x = Mathf.Clamp(position.x, -moveLimit.x, moveLimit.x);
		position.y = Mathf.Clamp(position.y, -moveLimit.y, moveLimit.y);
		playerRigidbody.MovePosition(position);


		//playerRigidbody.position = BoundaryManager.Instance.Clamp(playerRigidbody.position);
		//playerRigidbody.MovePosition(playerRigidbody.position + m_MovementInput * moveSpeed * Time.fixedDeltaTime);

		if(m_AllowShooting)
		{
			if(m_IsShooting)
			{
				Shoot(activePlayerTurrets);
				StartCoroutine(ShootTimer());
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		CollectPowerup powerUp = collision.GetComponent<CollectPowerup>();
		if(powerUp != null)
		{
			powerUp.PowerupCollected();
			UpgradeWeapons();
		}

		if(!godMode)
		{
			if(collision.GetComponent<EnemyController>() || collision.GetComponent<Bullet>()) //Enemy projectile staat op een andere layer
			{
				StartCoroutine(ActivateGameOver());
			}
		}
	}

	private void Shoot(List<GameObject> turrets)
	{
		foreach (GameObject turret in turrets)
		{
			GameObject projectile = ObjectPooler.m_Instance.SetActiveFromPool(WhichPrefab.playerBullet, turret.transform.position, turret.transform.rotation);
			projectile.GetComponent<Rigidbody2D>().AddForce(turret.transform.up * m_BulletSpeed, ForceMode2D.Impulse);
		}

		shootSoundFX.Play();
	}

	private void UpgradeWeapons()
	{
		if(upgradeState == 0)
		{
			foreach (GameObject turret in tripleShotTurrets)
			{
				activePlayerTurrets.Add(turret);
			}
		}
		else if (upgradeState == 1)
		{
			foreach (GameObject turret in wideShotTurrets)
			{
				activePlayerTurrets.Add(turret);
			}
		}
		else if (upgradeState == 2)
		{
			StartCoroutine(ActivateScatterShotTurret());
		}
		else
		{
			return;
		}
		upgradeState++;
	}

	private IEnumerator ActivateScatterShotTurret()
	{
		while(true)
		{
			Shoot(scatterShotTurrets);
			yield return new WaitForSeconds(scatterShotTurretReloadTime);
		}
	}

	public void MovementInputInfo(InputAction.CallbackContext context)
	{
		m_MovementInput = context.ReadValue<Vector2>();
	}

	public void ShootInputInfo(InputAction.CallbackContext context)
	{
		m_IsShooting = context.performed;
	}

	private IEnumerator ShootTimer()
	{
		m_AllowShooting = false;
		yield return new WaitForSeconds(m_ShootDelay);
		m_AllowShooting = true;
	}

	private IEnumerator ActivateGameOver()
	{
		StopCoroutine(ActivateScatterShotTurret());
		StopCoroutine(ShootTimer());
		m_AllowShooting = false;

		GameManager.Instance.ShowGameOver();  // If the player is hit by an enemy ship or laser it's game over.
		playerRenderer.enabled = false;       // We can't destroy the player game object straight away or any code from this point on will not be executed
		playerCollider.enabled = false;       // We turn off the players renderer so the player is not longer displayed and turn off the players collider
		playerThrust.Stop();
		ObjectPooler.m_Instance.SetActiveFromPool(WhichPrefab.playerExplosion, playerRigidbody.position, playerRigidbody.transform.rotation);
		//Instantiate(explosion, transform.position, transform.rotation);   // Then we Instantiate the explosions... one at the centre and some additional around the players location for a bigger bang!
		for (int i = 0; i < 8; i++)
		{
			Vector3 randomOffset = new Vector3(transform.position.x + Random.Range(-0.6f, 0.6f), transform.position.y + Random.Range(-0.6f, 0.6f), 0.0f);
			ObjectPooler.m_Instance.SetActiveFromPool(WhichPrefab.playerExplosion, randomOffset, playerRigidbody.transform.rotation);
			//Instantiate(explosion, randomOffset, transform.rotation);
		}

		yield return new WaitForSeconds(0.5f);
		gameObject.SetActive(false);
		//Destroy(gameObject, 1.0f); // The second parameter in Destroy is a delay to make sure we have finished exploding before we remove the player from the scene.
	}
}
