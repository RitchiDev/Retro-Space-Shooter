using UnityEngine;

// https://www.youtube.com/watch?v=PwYklTo_qyc
namespace SimplePool
{
    public class TestFireScript : MonoBehaviour
    {
        public ObjectPoolRene bulletPool;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject _ = bulletPool.GetPooledObject(transform.position, transform.rotation);
            }
        }
    }
}