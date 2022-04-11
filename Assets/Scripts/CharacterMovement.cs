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

        //trigger movement animation
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            characterAnimator.SetBool("isRunning", true);
        }
        else if (!Input.GetKey(KeyCode.D) || !Input.GetKey(KeyCode.RightArrow))
        {
            characterAnimator.SetBool("isRunning", false);
        }

        //clamping axis rotation
        playerController.constraints = RigidbodyConstraints.FreezeRotation;

        //activate jump
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (floorCheckerBox.isGrounded == true)
            {
                jump();
            }
        }
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
