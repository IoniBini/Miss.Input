using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterMovement : MonoBehaviour
{
    //movement variables
    protected Rigidbody playerController;
    protected FloorChecker floorCheckerBox;
    public float speed = 10f;
    public Vector3 movement;
    public float slowMoveWhileAttacking = 1f;
    public bool isWalkingBothWays = false;
    public int horizontalValue = 0;

    //animation variables
    protected Animator characterAnimator;

    //attack variables
    public float resetToIdle = 0.5f;
    private int attackPhaseCounter = 0;
    private float attackDelayTime = 0.4f;
    private float attackDelayCounter;
    public bool isAttacking = false;

    //jump variables
    public float jumpTimerReset = 0.5f;
    private bool avoidJumpSpam = false;
    public float jumpStrenght = 5f;
    public float coyoteTime = 0.2f;
    public float coyoteTimeCounter;
    public float bufferTime = 0.2f;
    public float bufferTimeCounter;
    public bool jumpExtraFrame = false;

    //health + defence variables
    public int healthPoints = 3;
    public bool isDead = false;
    public bool preventDamage = false;
    public bool isDefending = false;

    //key randomiser variables
    public KeyCode[] keyArray;

    [SerializeField]
    private int attackKeyInt;
    private TextMeshProUGUI attackKeyDisplay;

    [SerializeField]
    private int defendKeyInt;
    private TextMeshProUGUI defendDisplay;

    [SerializeField]
    private int jumpKeyInt;
    private TextMeshProUGUI jumpDisplay;

    [SerializeField]
    private int leftKeyInt;
    private TextMeshProUGUI leftKeyDisplay;

    [SerializeField]
    private int rightKeyInt;
    private TextMeshProUGUI rightKeyDisplay;

    private void Awake()
    {
        playerController = GetComponent<Rigidbody>();
        floorCheckerBox = GameObject.Find("FloorCollisionCheck").GetComponent<FloorChecker>();
        characterAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        //upon starting, all the keys for controlling the character are randomly picked from the array

        //attack
        attackKeyInt = Random.Range(0, keyArray.Length);
        attackKeyDisplay = GameObject.Find("AttackButton").GetComponent<TextMeshProUGUI>();
        attackKeyDisplay.text = "Attack: " + keyArray[attackKeyInt].ToString();

        //defend
        defendKeyInt = Random.Range(0, keyArray.Length);
        //prevents a two inputs to be bound to the same key
        do{defendKeyInt = Random.Range(0, keyArray.Length);}
        while (defendKeyInt == attackKeyInt);
        defendDisplay = GameObject.Find("DefendButton").GetComponent<TextMeshProUGUI>();
        defendDisplay.text = "Defend: " + keyArray[defendKeyInt].ToString();

        //jump
        jumpKeyInt = Random.Range(0, keyArray.Length);
        //prevents a two inputs to be bound to the same key
        do { jumpKeyInt = Random.Range(0, keyArray.Length); }
        while (jumpKeyInt == attackKeyInt || jumpKeyInt == defendKeyInt);
        jumpDisplay = GameObject.Find("JumpButton").GetComponent<TextMeshProUGUI>();
        jumpDisplay.text = "Jump: " + keyArray[jumpKeyInt].ToString();

        //right
        rightKeyInt = Random.Range(0, keyArray.Length);
        //prevents a two inputs to be bound to the same key
        do { rightKeyInt = Random.Range(0, keyArray.Length); }
        while (rightKeyInt == attackKeyInt || rightKeyInt == defendKeyInt || rightKeyInt == jumpKeyInt);
        rightKeyDisplay = GameObject.Find("RightButton").GetComponent<TextMeshProUGUI>();
        rightKeyDisplay.text = "Right: " + keyArray[rightKeyInt].ToString();

        //left
        leftKeyInt = Random.Range(0, keyArray.Length);
        //prevents a two inputs to be bound to the same key
        do { leftKeyInt = Random.Range(0, keyArray.Length); }
        while (leftKeyInt == attackKeyInt || leftKeyInt == defendKeyInt || leftKeyInt == jumpKeyInt || leftKeyInt == rightKeyInt);
        leftKeyDisplay = GameObject.Find("LeftButton").GetComponent<TextMeshProUGUI>();
        leftKeyDisplay.text = "Left: " + keyArray[leftKeyInt].ToString();
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }*/

        if (Input.GetKey(keyArray[rightKeyInt]))
        {
            horizontalValue = 1;
        }
        else if (Input.GetKey(keyArray[leftKeyInt]))
        {
            horizontalValue = -1;
        }
        else if (Input.GetKey(keyArray[leftKeyInt]) && Input.GetKey(keyArray[rightKeyInt]))
        {
            horizontalValue = 0;
        }
        else
        {
            horizontalValue = 0;
        }

        //movement input
        movement = new Vector3(horizontalValue, 0f, 0f);

        //any time you leave the floor, you have a grace period of 0.2 seconds to jump. This resets any time you reach the floor again.
        if (floorCheckerBox.isGrounded == true)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        //any time you jump, you have a grace period of 0.2 seconds before hitting the floor where if you press jump, it'll work regardless of not yet being grounded
        if (Input.GetKeyDown(keyArray[jumpKeyInt]))
        {
            bufferTimeCounter = bufferTime;
        }
        else if (bufferTimeCounter >= 0f)
        {
            bufferTimeCounter = -Time.deltaTime;
        }


        //freezes the character in place if no input is being received, so that it doesn't slide down sloped surfaces.
        //I do want to say I have found a specific instance where if the player tries to walk both ways at once on a sloped surface, they will fall as they are no longer kinematic. I didn't have time to fix it though...
        if (floorCheckerBox.isGrounded == true && isWalkingBothWays == false && Input.GetKeyDown(keyArray[jumpKeyInt]) == false)
        {
            if (jumpExtraFrame == true)
            {
                GetComponent<Rigidbody>().drag = 0;
            }
            else
            {
                GetComponent<Rigidbody>().drag = 30;
            }
        }
        else if (floorCheckerBox.isGrounded == false)
        {
            GetComponent<Rigidbody>().drag = 0;
        }

        //making it so that the animator bool is always the same as the code bool for being grounded
        if (floorCheckerBox.isGrounded == true)
        {
            //the reason I was turning gravity on and off was because if you run into a wall non stop while on an angle, unity interprets this as freefall, making it so that it builds up heaps of momentum,
            //consequentially, when you let go of the walk, you are pushed back with lots of force. Turning gravity off only helps but doesn't fully fix it, I'd need mroe time to reasearch a better solution
            //especially because doing it this way also makes you able to walk up certain walls
            //GetComponent<Rigidbody>().useGravity = false;

            characterAnimator.SetBool("isGrounded", true);
            characterAnimator.SetBool("isFallingIdleLeft", false);
            characterAnimator.SetBool("isFallingIdleRight", false);
        }
        else
        {
            //GetComponent<Rigidbody>().useGravity = true;

            characterAnimator.SetBool("isGrounded", false);
            characterAnimator.SetBool("isFallingIdleRight", true);
            characterAnimator.SetBool("isFallingIdleLeft", true);
        }

        //it checks to make sure the player can't walk both ways at the same time, making the animation loop between left and right endlessly
        if (isWalkingBothWays == true)
        {
            characterAnimator.SetBool("isRunningRight", false);
            characterAnimator.SetBool("isRunningLeft", false);
        }
        else
        {
            //trigger movement animation right 
            if (Input.GetKey(keyArray[rightKeyInt]))
            {
                characterAnimator.SetBool("isRunningRight", true);
            }
            else if (!Input.GetKey(keyArray[rightKeyInt]))
            {
                characterAnimator.SetBool("isRunningRight", false);
            }
            //trigger movement animation left
            if (Input.GetKey(keyArray[leftKeyInt]))
            {
                characterAnimator.SetBool("isRunningLeft", true);
            }
            else if (!Input.GetKey(keyArray[leftKeyInt]))
            {
                characterAnimator.SetBool("isRunningLeft", false);
            }
        }

        //activate attack using the randomly picked key
        if (Input.GetKeyDown(keyArray[attackKeyInt]))
        {
            isAttacking = true;

            //if player only attacks once, then only the first animation will play, but if twice, then the second will play. This loop can be repeated without going back to idle.
            if (attackPhaseCounter == 0)
            {
                //every time you are not attacking, the delay counter is returned to its original value
                attackDelayCounter = attackDelayTime;
                attackPhaseCounter = 1;
            }
            else if (attackPhaseCounter == 1)
            {
                attackDelayCounter = attackDelayTime;
                attackPhaseCounter = 2;
            }
            else if (attackPhaseCounter == 2)
            {
                attackDelayCounter = attackDelayTime;
                attackPhaseCounter = 1;
            }

            if (attackPhaseCounter == 1)
            {
                characterAnimator.SetBool("isAttacking1", true);
                characterAnimator.SetBool("isAttacking2", false);
            }
            else if (attackPhaseCounter == 2)
            {
                characterAnimator.SetBool("isAttacking1", false);
                characterAnimator.SetBool("isAttacking2", true);
            }
        }

        //after the animtaion and the counter are resolved, the game waits for a short second. If the next input is set, then the next animation plays.
        if (isAttacking == true)
        {
            attackDelayCounter -= Time.deltaTime;
        }

        if (attackDelayCounter <= 0f && isAttacking == true)
        {
            //But then if nothing is pressed, then it returns you to idle via the coroutine
            StartCoroutine(attackReset());
        }

        //activate shield, can't be activated midair
        if (Input.GetKey(keyArray[defendKeyInt]) && floorCheckerBox.isGrounded == true)
        {
            //originally, as you can see bellow, I intended to check if the player is facing the enemy with the shield in order to defend correctly. 
            //I abandonded the idea due to time constraint, because I realised it would take some time to figure out how to make this work with multiple instances of enemies.

            /*//it checks to see if the enemy is to your right or left, because the damage should ONLY be prevented if coming from the front, not the back
            if (transform.position.x > GameObject.Find("xbot").transform.position.x && facingRightPlayer == true)
            {
                facingRightPlayer = false;
            }
            else
            {
                facingRightPlayer = true;
            }

            //then if it is facing the correct direction, it applies the damage prevention
            if (isDefending == true)
            {
                preventDamage = true;
            }
            else if (isDefending == false)
            {
                preventDamage = false;
            }*/

            //slow player movement down gradually while defending and moving
            if (slowMoveWhileAttacking >= 0.001f)
            {
                slowMoveWhileAttacking -= Time.deltaTime * 3f;
            }
            //once you reach the slowest speed possible, you freeze altogether, and become kinematic so that you don't slide down slopes when attacking
            else if (slowMoveWhileAttacking <= 0.01f)
            {
                slowMoveWhileAttacking = 0;
                GetComponent<Rigidbody>().drag = 30;
            }

            isDefending = true;
            characterAnimator.SetBool("isDefending", true);
        }
        else
        {
            //slowly but a little faster build it back up when you stop attacking
            if (slowMoveWhileAttacking <= 1)
            {
                slowMoveWhileAttacking += Time.deltaTime;
            }

            isDefending = false;
            characterAnimator.SetBool("isDefending", false);
        }

        //activate jump
        if (bufferTimeCounter > 0f)
        {
            if (coyoteTimeCounter > 0f && avoidJumpSpam == false)
            {
                //both coyote and buffer time counter needs to be set to 0 every time you jumpt to avoid spamming it
                coyoteTimeCounter = 0f;
                bufferTimeCounter = 0f;
                characterAnimator.SetBool("isJumping", true);

                StartCoroutine(Jump());

                StartCoroutine(jumpReset());
            }
        }
    }

    IEnumerator jumpReset()
    {
        avoidJumpSpam = true;
        //to avoid frustration, I removed the majority of the slowing down from the attack once you jump to dodge
        slowMoveWhileAttacking = 0.7f;
        //the character needs at least one frame to get off the floor for the animation to commence, hence I'm making it wait before resetting the jump bool to false 
        yield return new WaitForSeconds(jumpTimerReset);
        avoidJumpSpam = false;
        characterAnimator.SetBool("isJumping", false);
        characterAnimator.SetBool("isFallingIdleRight", true);
        characterAnimator.SetBool("isFallingIdleLeft", true);
    }

    IEnumerator attackReset()
    {
        attackPhaseCounter = 0;
        //the animation needs to be reset when you stop pressing the attack
        yield return new WaitForSeconds(resetToIdle);
        isAttacking = false;
        characterAnimator.SetBool("isAttacking1", false);
        characterAnimator.SetBool("isAttacking2", false);
    }

    private void FixedUpdate()
    {
        moveCharacter(movement);
    }

    void moveCharacter(Vector3 direction)
    {
        //movement horizontal + it checks to make sure the player can't walk both ways at the same time
        if (Input.GetKey(keyArray[rightKeyInt]) && Input.GetKey(keyArray[leftKeyInt]))
        {
            isWalkingBothWays = true;
        }
        else
        {
            isWalkingBothWays = false;

            //slow player movement down gradually while in attack, and also slowly but a little faster build it back up when you stop attacking
            if (isAttacking == true && slowMoveWhileAttacking >= 0.01f)
            {
                slowMoveWhileAttacking -= Time.deltaTime * 3f;
            }
            //also checks to see if you are not defending before returning movement speed to normal
            else if (slowMoveWhileAttacking <= 1 && !Input.GetKey(KeyCode.J))
            {
                slowMoveWhileAttacking += Time.deltaTime;
            }

            //once you reach the slowest speed possible, you freeze altogether, so that you don't slide down slopes when attacking
            if (isAttacking == true && slowMoveWhileAttacking <= 0.01f)
            {
                GetComponent<Rigidbody>().drag = 30;
            }

            if (isDead == false)
            {
                playerController.MovePosition(transform.position + (direction * speed * Time.deltaTime * slowMoveWhileAttacking));
            }
        }
    }

    public IEnumerator beingDamaged()
    {
        healthPoints--;
        characterAnimator.SetBool("isBeingHit", true);

        if (healthPoints <= 0)
        {
            //the bool prevents the player from moving when dead and plays the death animation if your health is 0 or less
            isDead = true;
        }

        switch (isDead)
        {
            case false:

                yield return new WaitForSeconds(0.3f);
                characterAnimator.SetBool("isBeingHit", false);
                break;

            case true:

                characterAnimator.SetBool("isDead", true);
                yield return new WaitForSeconds(0.8f);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
        }    
    }

    IEnumerator Jump()
    {
        jumpExtraFrame = true;
        playerController.AddForce(0f, jumpStrenght, 0f);
        yield return new WaitForSeconds(0.1f);
        jumpExtraFrame = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //ignore collision between the player and the enemies, so you can walk through them. 
        //this code should be in the player as opposed to the enemy, since there are multiple instances of the skeleton whereas the player only one

        if (collision.gameObject.tag == "Enemy")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<CapsuleCollider>());
        }
    }
}
