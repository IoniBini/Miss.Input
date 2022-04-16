using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordCollision : MonoBehaviour
{
    private CharacterMovement characterMovementScript;
    private Animator enemyAnimator;
    private bool canBeHit = true;

    private void Awake()
    {
        characterMovementScript = GameObject.Find("xbot").GetComponent<CharacterMovement>();
        enemyAnimator = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //only triggers if enemy is attacking, and only if within the VERY GENEROUS time range I created on the enumerator on the bottom. The code also prevents damaging you when the enemy dies
        if (other.tag == "Player" && enemyAnimator.GetBool("isAttacking") == true && canBeHit == true && enemyAnimator.GetBool("isDead") == false)
        {
            //once the sword collides with the enemy, the sword finds tells the player that it has been struck
            StartCoroutine(characterMovementScript.beingDamaged());

            StartCoroutine(CanBeHitCoroutine());
        }
    }

    IEnumerator CanBeHitCoroutine()
    {
        canBeHit = false;

        yield return new WaitForSeconds(1f);

        canBeHit = true;
    }
}
