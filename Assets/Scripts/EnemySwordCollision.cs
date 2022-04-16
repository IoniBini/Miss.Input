using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordCollision : MonoBehaviour
{
    private CharacterMovement characterMovementScript;
    private Animator enemyAnimator;
    public EnemyAI enemyAIScript;

    private void Awake()
    {
        characterMovementScript = GameObject.Find("xbot").GetComponent<CharacterMovement>();
        enemyAnimator = GetComponentInParent<Animator>();
        enemyAIScript = GetComponentInParent<EnemyAI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //only triggers if enemy is attacking, and only if within the time range I created events within the attack animation of the skeleton. The code also prevents damaging you when the enemy dies
        if (other.tag == "Player" && enemyAnimator.GetBool("isAttacking") == true && enemyAIScript.hitFrame == true && enemyAnimator.GetBool("isDead") == false && characterMovementScript.preventDamage == false)
        {
            //once the sword collides with the enemy, the sword finds tells the player that it has been struck
            StartCoroutine(characterMovementScript.beingDamaged());
        }
    }
}
