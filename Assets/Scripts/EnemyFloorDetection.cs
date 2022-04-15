using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFloorDetection : MonoBehaviour
{
    public EnemyAI enemyAI;

    private void Start()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6 && other.tag != "Player")
        {
            enemyAI.Flip();
        }
    }
}
