using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    protected Rigidbody playerController;
    protected FloorChecker floorCheckerBox;
    public float speed = 10f;
    public Vector3 movement;

    protected Animator characterAnimator;
    public float jumpTimerReset = 0.5f;

    public float coyoteTime = 0.2f;
    public float coyoteTimeCounter;

    public float bufferTime = 0.2f;
    public float bufferTimeCounter;

    private bool avoidJumpSpam = false;

    public float jumpStrenght = 5f;

    private void Awake()
    {
        //allocating the correct rigidbodies 
        playerController = GetComponent<Rigidbody>();
        floorCheckerBox = GameObject.Find("FloorCollisionCheck").GetComponent<FloorChecker>();
        characterAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
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
        else
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
    }

    IEnumerator jumpReset()
    {
        avoidJumpSpam = true;
        //the character needs at least one frame to get off the floor for the animation to commence, hence I'm making it wait before resetting the jump bool to false 
        yield return new WaitForSeconds(jumpTimerReset);
        avoidJumpSpam = false
            ;
        characterAnimator.SetBool("isJumping", false);
        characterAnimator.SetBool("isFallingIdleRight", true);
        characterAnimator.SetBool("isFallingIdleLeft", true);
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
            playerController.MovePosition(transform.position + (direction * speed * Time.deltaTime));
        }
    }
}
