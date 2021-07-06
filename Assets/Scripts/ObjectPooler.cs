using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum WhichPrefab //Nog kijken om dit beter te kunnen doen :)
{ 
    playerBullet = 0,
    playerBulletImpact,
    playerExplosion,
    enemyType1,
    enemyType2,
    enemyBullet,
    enemyBulletImpact,
    enemyExplosion,
    powerUp
}

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler m_Instance { get; private set; }

    [SerializeField] private Dictionary<WhichPrefab, Queue<GameObject>> m_PoolDictionary;
    [SerializeField] private List<Pool> m_Pools = new List<Pool>();
    private GameObject m_ObjectToSetActive;

    [System.Serializable]
    public class Pool
    {
        [SerializeField] private string m_ElementName; //Om te makelijk te organiseren in de Unity Inspector
        public WhichPrefab m_WhichPrefab;
        public GameObject m_Prefab;
        public Transform m_Parent; //Waar het in de hierarchy gezet word
        public int m_CopyAmount;
    }

    private void Awake()
    {
        
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else if (m_Instance != null)
        {
            Destroy(this);
        }

        m_PoolDictionary = new Dictionary<WhichPrefab, Queue<GameObject>>();

        foreach (Pool pool in m_Pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.m_CopyAmount; i++)
            {
                GameObject copy = Instantiate(pool.m_Prefab, pool.m_Parent);
                copy.SetActive(false);

                objectPool.Enqueue(copy);
            }

            m_PoolDictionary.Add(pool.m_WhichPrefab, objectPool);
        }
    }

    public GameObject SetActiveFromPool(WhichPrefab whichPrefab, Vector2 position, Quaternion rotation)
    {
        if (!m_PoolDictionary.ContainsKey(whichPrefab))
        {
            Debug.LogError("De enum: " + whichPrefab + " staat nog niet in één van de elementen in de Inspector!");
            return null;
        }

        m_ObjectToSetActive = m_PoolDictionary[whichPrefab].Dequeue();

        //Net zoals de Instantiate :D
        m_ObjectToSetActive.SetActive(true);
        m_ObjectToSetActive.transform.position = position;
        m_ObjectToSetActive.transform.rotation = rotation;

        m_PoolDictionary[whichPrefab].Enqueue(m_ObjectToSetActive);

        return m_ObjectToSetActive;
    }
}
