using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public int enemyHealth = 3;

    //damage variables
    public bool isHit = false;
    public bool beingDamaged = false;
    public float hitDelay = 0.5f;

    //materials used to make enemy flash when hit
    public Material damagedShader;
    public Material regularShader;
    public Material skeletonBody;

    //enemy movement variables
    public Vector3 enemyMovement;
    public float enemySpeed = 10f;
    private Rigidbody enemyController;
    public float orientation = 1;

    //fight mode variables
    public bool playerDectected = false;
    public bool isFacingRight = true;
    public float combatMovementSpeed = 4.5f;
    private Animator enemyAnimator;

    private void Start()
    {
        //floorDetection = this.gameObject.transform.GetChild(0).GetComponent<>
        enemyController = GetComponent<Rigidbody>();
        enemyAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        enemyMovement = new Vector3(orientation, 0f, 0f);

        //when the enemy is struck, it deducts one health point and returns isHit to false
        if (isHit == true && beingDamaged == false)
        {
            StartCoroutine(Damage());
        }
    }

    private void FixedUpdate()
    {
        moveEnemy(enemyMovement);
    }

    void moveEnemy(Vector3 direction)
    {
        switch (playerDectected)
        {
            //if the player isn't near, then the enemy simply strolls around from right to left
            case false: enemyController.MovePosition(transform.position + (direction * enemySpeed * Time.deltaTime));

                break;

            //if the player is found, the enemy will attampt to close into the player
            case true:

                //flips the enemy towards the player always
                if (transform.position.x > GameObject.Find("xbot").transform.position.x && isFacingRight == true)
                {
                    Flip();
                }

                if (transform.position.x < GameObject.Find("xbot").transform.position.x && isFacingRight == false)
                {
                    Flip();
                }

                //if the enemy is facing the player and is up close, then it slows down its movement
                if (transform.position.x < GameObject.Find("xbot").transform.position.x && isFacingRight == true)
                {
                    //these debugs demonstrate how the math is done if need be
                    Debug.Log(GameObject.Find("xbot").transform.position.x);
                    Debug.Log(transform.position.x);

                    if (Mathf.Abs(GameObject.Find("xbot").transform.position.x - Mathf.Abs(transform.position.x)) <= 1.5f)
                    {
                        combatMovementSpeed = 0;

                        enemyAnimator.SetBool("isAttacking", true);
                    }
                    else
                    {
                        combatMovementSpeed = 4.5f;
                        enemyAnimator.SetBool("isAttacking", false);
                    }

                    enemyController.MovePosition(transform.position + (direction * enemySpeed * Time.deltaTime * combatMovementSpeed));
                }

                else if (transform.position.x > GameObject.Find("xbot").transform.position.x && isFacingRight == false)
                {
                    /*Debug.Log(transform.position.x - GameObject.Find("xbot").transform.position.x);
                    Debug.Log(Mathf.Abs(GameObject.Find("xbot").transform.position.x));
                    Debug.Log(Mathf.Abs(transform.position.x));*/

                    if (transform.position.x - GameObject.Find("xbot").transform.position.x <= 1.5f)
                    {
                        combatMovementSpeed = 0;

                        enemyAnimator.SetBool("isAttacking", true);
                    }
                    else
                    {
                        combatMovementSpeed = 4.5f;
                        enemyAnimator.SetBool("isAttacking", false);
                    }

                    enemyController.MovePosition(transform.position + (direction * enemySpeed * Time.deltaTime * combatMovementSpeed));
                }

                break;
        }
    }

    IEnumerator Damage()
    {
        //needs to immidetally become false again, otherwise the number goes does super fast
        enemyHealth--;
        isHit = false;

        //prevents the player from spamming and killing the enemy instantly
        beingDamaged = true;

        //enemy self destructs when health is 0
        if (enemyHealth == 0)
        {
            Destroy(this.gameObject);
        }

        //briefly changes the material so it flashes, then returns it to normal
        this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material = damagedShader;

        yield return new WaitForSeconds(hitDelay);

        beingDamaged = false;

        this.gameObject.transform.GetChild(0).GetComponent<Renderer>().material = regularShader;
    }

    public void Flip()
    {
        var currentRotation = GetComponent<Transform>().eulerAngles.y;

        GetComponent<Transform>().eulerAngles = new Vector3(0, currentRotation * -1, 0);
        orientation = orientation * -1;
        
        if (isFacingRight == true)
        {
            isFacingRight = false;
        }
        else
        {
            isFacingRight = true;
        }
    }
}
