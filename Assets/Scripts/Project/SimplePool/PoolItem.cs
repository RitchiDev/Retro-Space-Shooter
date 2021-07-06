using UnityEngine;

// based on https://www.youtube.com/watch?v=PwYklTo_qyc
namespace SimplePool
{
    public class PoolItem : MonoBehaviour
    {
        ObjectPoolRene myPool;

        /// <summary>
        /// The object pool we belong to.
        /// </summary>
        public ObjectPoolRene Pool 
        { 
            set { myPool = value; }
        }

        /// <summary>
        /// This is called just before this item is activated.
        /// </summary>
        protected virtual void Reset() { }

        /// <summary>
        /// This is called just before this item is deactivated.
        /// </summary>
        protected virtual void Deactivate() { }

        /// <summary>
        /// Initialialize this item and activate it.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        public virtual void Init(Vector3 position, Quaternion rotation, Transform parent)
        {
            // put item into position
            transform.position = position;
            transform.rotation = rotation;
            transform.parent   = parent;

            // allow item to reset itself
            Reset();

            // activate item
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Call this to return this item to the pool it belongs to.
        /// </summary>
        public virtual void ReturnToPool()
        {
            // allow item deactivate itself
            Deactivate();

            // add item to the pool again
            myPool.AddItem(this);
        }

        /// <summary>
        /// Convenience method that checks if the game object is a pool item.
        /// </summary>
        /// <param name="gameObject">The game object that must be returned to a pool or destroyed.</param>
        /// <param name="pooling_enabled">If false the object is always destroyed regardless if it is a pool item or not.</param>
        public static void ReturnToPoolOrDestroy(GameObject gameObject, bool pooling_enabled=true)
        {
            // process pooled item
            if (pooling_enabled)
            {
                SimplePool.PoolItem pool_item = gameObject.GetComponent<SimplePool.PoolItem>() as SimplePool.PoolItem;
                if (pool_item != null)
                {
                    pool_item.ReturnToPool();
                    return;
                }
            }

            // destroy the object
            Destroy(gameObject);
        }
    }
}
