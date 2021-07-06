using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	//--------------------------------------------------------------------------------
	// Make sure this class is a singleton
	//--------------------------------------------------------------------------------
	private static GameManager instance;
	public static GameManager Instance { get => instance; }

	//--------------------------------------------------------------------------------
	// Class implementation
	//--------------------------------------------------------------------------------

	[Header("UI:")]
	public Text   gameOverLabel;
	public Button restartGameButton;

	[Header("Enemy objects:")]
	public GameObject enemyType1;
	public GameObject enemyType2;

	[Header("Enemy config:")]
	public float startWait      = 1.0f;
	public float waveInterval   = 2.0f;
	public float spawnInterval  = 0.5f;
	public int   enemiesPerWave = 5;

	[Header("Debug:")]
	public bool useObjectPool = true;

	private bool m_SpawnEnemies;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			//DontDestroyOnLoad(this);
		}
		else if (instance != this)
			Destroy(this);
	}

	private void Start()
	{
		m_SpawnEnemies = true;
		StartCoroutine(SpawnEnemyWaves());
	}

	private IEnumerator SpawnEnemyWaves()
	{
		while(BoundaryManager.Instance.TopLeft() == Vector3.zero || BoundaryManager.Instance.TopRight() == Vector3.zero)
		{
			yield return new WaitForSeconds(0.01f);
		}

		Vector2 topLeft = BoundaryManager.Instance.TopLeft();
		Vector2 topRight = BoundaryManager.Instance.TopRight();
		Quaternion spawnRotation = Quaternion.Euler(0, 0, 180);

		yield return new WaitForSeconds(startWait);

		while(m_SpawnEnemies)
		{
			for (int i = 0; i < enemiesPerWave; i++)
			{
				Vector2 spawnPosition = new Vector2(Random.Range(topLeft.x, topRight.x), topLeft.y);
				WhichPrefab randomEnemy = Random.Range(0, 2) == 0 ? WhichPrefab.enemyType1 : WhichPrefab.enemyType2;
				ObjectPooler.m_Instance.SetActiveFromPool(randomEnemy, spawnPosition, spawnRotation);
				yield return new WaitForSeconds(spawnInterval);
			}
			yield return new WaitForSeconds(waveInterval);
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}


	public void ShowGameOver()
	{
		m_SpawnEnemies = false;
		gameOverLabel.rectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
		restartGameButton.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, -50, 0);
	}

	public void RestartGame()
	{
		SceneManager.LoadScene("GameScene");
	}
}
