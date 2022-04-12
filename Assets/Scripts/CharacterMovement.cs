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
    private bool isResettingJump = true;

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



        //activate jump
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (floorCheckerBox.isGrounded == true)
            {
                characterAnimator.SetBool("isJumping", true);
                jump();
                StartCoroutine(jumpReset());
            }
        }
    }

    IEnumerator jumpReset()
    {
        //the character needs at least one frame to get off the floor for the animation to commence, hence I'm making it wait before resetting the jump bool to false 
        yield return new WaitForSeconds(jumpTimerReset);
        characterAnimator.SetBool("isJumping", false);
        characterAnimator.SetBool("isFallingIdleRight", true);
        characterAnimator.SetBool("isFallingIdleLeft", true);
    }

    void jump()
    {
        playerController.AddForce(0f, jumpStrenght, 0f);
    }

    private void FixedUpdate()
    {
        moveCharacter(movement);
    }

    void moveCharacter(Vector3 direction)
    {
        //movement horizontal
        playerController.MovePosition(transform.position + (direction * speed * Time.deltaTime));
    }
}
