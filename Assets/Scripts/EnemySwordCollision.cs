using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordCollision : MonoBehaviour
{
    private CharacterMovement characterMovementScript;
    private Animator enemyAnimator;

    private void Awake()
    {
        characterMovementScript = GameObject.Find("xbot").GetComponent<CharacterMovement>();
        enemyAnimator = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //only triggers if you are attacking
        if (other.tag == "Player" && enemyAnimator.GetBool("isAttacking") == true)
        {
            //once the sword collides with the enemy, the sword finds tells the player that it has been struck
            characterMovementScript.beingDamaged();
        }
    }
}
