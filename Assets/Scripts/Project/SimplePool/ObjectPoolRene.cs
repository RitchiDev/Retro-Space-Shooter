using System.Collections.Generic;
using UnityEngine;

// based on https://www.youtube.com/watch?v=PwYklTo_qyc
namespace SimplePool
{
    public class ObjectPoolRene : MonoBehaviour
    {
        public GameObject pooledObject;
        public int        poolSize      = 10;
        public bool       autoExpand    = false;
        public int        expensionSize = 10;

        private Stack<PoolItem> objectPool;

        // Start is called before the first frame update
        void Start()
        {
            objectPool = new Stack<PoolItem>(poolSize);
            Expand(poolSize);
        }

        /// <summary>
        /// Expand the pool with the specified number of instantiations.
        /// </summary>
        /// <param name="size">The expansion size.</param>
        private void Expand(int size)
        {
            // check config
            if (pooledObject == null)
            {
                Debug.LogError("ObjectPool: No object to pool");
                return;
            }
            if (size == 0)
            {
                Debug.LogError("ObjectPool: Invalid expension size");
                return;
            }

            // expand pool
            for (int i=0; i< size; i++)
            {
                GameObject new_object = Instantiate(pooledObject);
                PoolItem   item       = new_object.GetComponent<PoolItem>();
                item.Pool = this;
                AddItem(item);
            }
        }

        /// <summary>
        /// Add item to the object pool.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(PoolItem item)
        {
            if (item.gameObject.activeSelf)
                item.gameObject.SetActive(false);
            item.transform.parent = transform;
            objectPool.Push(item);
        }

        /// <summary>
        /// Get an object from the object pool.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        /// <returns>An instantiation of the pool object, or null if the pool is empty.</returns>
        public GameObject GetPooledObject(Vector3 position, Quaternion rotation, Transform parent=null)
        {
            // make sure the pool is not empty
            if (objectPool.Count <= 0)
            {
                // expand if requested
                if (autoExpand && expensionSize > 0)
                    Expand(expensionSize);
                else
                {
                    Debug.LogError("ObjectPool: Pool is empty");
                    return null;
                }
            }

            // prep the item about to be 'instantiated'
            objectPool.Peek().Init(position, rotation, parent != null ? parent : null);
            PoolItem item = objectPool.Pop();
            return item.gameObject; 
        }
    }
}
