using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public EnemyAI enemyAI;

    private void Start()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            enemyAI.playerDectected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            enemyAI.playerDectected = false;
        }
    }
}
