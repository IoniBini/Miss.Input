using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour
{
    //movement variables
    protected Rigidbody playerController;
    protected FloorChecker floorCheckerBox;
    public float speed = 10f;
    public Vector3 movement;
    public float slowMoveWhileAttacking = 1f;

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

    private void Awake()
    {
        //allocating the correct rigidbodies 
        playerController = GetComponent<Rigidbody>();
        floorCheckerBox = GameObject.Find("FloorCollisionCheck").GetComponent<FloorChecker>();
        characterAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        //movement input
        movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);//Input.GetAxis("Vertical")

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
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            bufferTimeCounter = bufferTime;
        }
        else if (bufferTimeCounter >= 0f)
        {
            bufferTimeCounter = -Time.deltaTime;
        }


        //freezes the character in place if no input is being received, so that it doesn't slide down sloped surfaces.
        //I do want to say I have found a specific instance where if the player tries to walk both ways at once on a sloped surface, they will fall as they are no longer kinematic. I didn't have time to fix it though...
        if (floorCheckerBox.isGrounded == true && Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }

        //making it so that the animator bool is always the same as the code bool for being grounded
        if (floorCheckerBox.isGrounded == true)
        {
            characterAnimator.SetBool("isGrounded", true);
            characterAnimator.SetBool("isFallingIdleLeft", false);
            characterAnimator.SetBool("isFallingIdleRight", false);
        }
        else
        {
            characterAnimator.SetBool("isGrounded", false);
            characterAnimator.SetBool("isFallingIdleRight", true);
            characterAnimator.SetBool("isFallingIdleLeft", true);
        }

        //it checks to make sure the player can't walk both ways at the same time, making the animation loop between left and right endlessly
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
        {
            characterAnimator.SetBool("isRunningRight", false);
            characterAnimator.SetBool("isRunningLeft", false);
        }
        else if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            characterAnimator.SetBool("isRunningRight", false);
            characterAnimator.SetBool("isRunningLeft", false);
        }
        else
        {
            //trigger movement animation right 
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                characterAnimator.SetBool("isRunningRight", true);
            }
            else if (!Input.GetKey(KeyCode.D) || !Input.GetKey(KeyCode.RightArrow))
            {
                characterAnimator.SetBool("isRunningRight", false);
            }
            //trigger movement animation left
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                characterAnimator.SetBool("isRunningLeft", true);
            }
            else if (!Input.GetKey(KeyCode.A) || !Input.GetKey(KeyCode.LeftArrow))
            {
                characterAnimator.SetBool("isRunningLeft", false);
            }
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
                playerController.AddForce(0f, jumpStrenght, 0f);
                StartCoroutine(jumpReset());
            }
        }

        //activate attack
        if (Input.GetKeyDown(KeyCode.L))
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
        if (Input.GetKey(KeyCode.J) && floorCheckerBox.isGrounded == true)
        {
            //slow player movement down gradually while defending and moving
            if (slowMoveWhileAttacking >= 0.001f)
            {
                slowMoveWhileAttacking -= Time.deltaTime * 3f;
            }
            //once you reach the slowest speed possible, you freeze altogether, and become kinematic so that you don't slide down slopes when attacking
            else if (slowMoveWhileAttacking <= 0.01f)
            {
                slowMoveWhileAttacking = 0;
                GetComponent<Rigidbody>().isKinematic = true;
            }

            characterAnimator.SetBool("isDefending", true);
        }
        else
        {
            //slowly but a little faster build it back up when you stop attacking
            if (slowMoveWhileAttacking <= 1)
            {
                slowMoveWhileAttacking += Time.deltaTime;
            }

            characterAnimator.SetBool("isDefending", false);
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
        if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
        {

        }
        else if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
        {

        }
        else
        {
            //slow player movement down gradually while in attack, and also slowly but a little faster build it back up when you stop attacking
            if (isAttacking == true && slowMoveWhileAttacking >= 0.01f)
            {
                slowMoveWhileAttacking -= Time.deltaTime * 1.5f;
            }
            //also checks to see if you are not defending before returning movement speed to normal
            else if (slowMoveWhileAttacking <= 1 && !Input.GetKey(KeyCode.J))
            {
                slowMoveWhileAttacking += Time.deltaTime;
            }

            //once you reach the slowest speed possible, you freeze altogether, so that you don't slide down slopes when attacking
            if (isAttacking == true && slowMoveWhileAttacking <= 0.01f)
            {
                GetComponent<Rigidbody>().isKinematic = true;
            }

            playerController.MovePosition(transform.position + (direction * speed * Time.deltaTime * slowMoveWhileAttacking));
        }
    }
}
