using UnityEngine;

// based on https://www.youtube.com/watch?v=PwYklTo_qyc
namespace SimplePool
{
    /// <summary>
    /// This pool item destroys itself after x seconds.
    /// </summary>
    public class PoolItemAutoDestroy : PoolItem
    {
        [SerializeField] float lifetime = 3.0f;

        private float deathTimer = 0.0f;

        /// <summary>
        /// Reset the timer.
        /// </summary>
        protected override void Reset()
        {
            deathTimer = 0.0f;
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(Vector3.forward * Time.deltaTime * 10.0f);

            deathTimer += Time.deltaTime;
            if (deathTimer >= lifetime)
                ReturnToPool();
        }
    }
}
