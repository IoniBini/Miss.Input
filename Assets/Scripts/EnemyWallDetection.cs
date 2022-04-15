using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWallDetection : MonoBehaviour
{
    public EnemyAI enemyAI;
    public GameObject enemyFloor;

    private void Start()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //when this trigger stops touching hte floor, it flips the enemy
        if (other.gameObject.layer == 6)
        {
            enemyAI.Flip();
            StartCoroutine(FlipWait());
        }
    }

    IEnumerator FlipWait()
    {
        enemyFloor.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        enemyFloor.SetActive(true);
    }
}
