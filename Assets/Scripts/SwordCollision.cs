using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    private EnemyAI enemyAIScript;

    private CharacterMovement characterMovementScript;

    private void Awake()
    {
        characterMovementScript = GameObject.Find("xbot").GetComponent<CharacterMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //only triggers if you are attacking
        if (other.tag == "Enemy" && characterMovementScript.isAttacking == true)
        {
            //once the sword collides with the enemy, the sword finds the script containing that enemy's AI and tells it that it has been struck
            enemyAIScript = other.gameObject.GetComponent<EnemyAI>();
            enemyAIScript.isHit = true;
        }
    }
}
