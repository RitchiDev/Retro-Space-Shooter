using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void DeactivateProjectile()
    {
        transform.position = Vector2.zero;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<ShipController>() || collision.GetComponent<Bullet>())
        {
            ObjectPooler.m_Instance.SetActiveFromPool(WhichPrefab.enemyBulletImpact, transform.position, Quaternion.identity);
        }
        else if(collision.GetComponent<EnemyController>())
        {
            ScoreManager.m_Instance.AddPoints(100);
            ObjectPooler.m_Instance.SetActiveFromPool(WhichPrefab.playerBulletImpact, transform.position, Quaternion.identity);
        }
        DeactivateProjectile();
    }
}
