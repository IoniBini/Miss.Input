using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBuildFix : MonoBehaviour
{
    private CharacterMovement characterMovementScript;
    private FloorChecker floorChecker;

    private void Start()
    {
        characterMovementScript = GetComponent<CharacterMovement>();
        floorChecker = GameObject.Find("FloorCollisionCheck").GetComponent<FloorChecker>();
    }

    public IEnumerator Jump()
    {
        floorChecker.OnJumpStart();
        characterMovementScript.jumpExtraFrame = true;
        characterMovementScript.GetComponent<Rigidbody>().AddForce(0f, characterMovementScript.jumpStrenght, 0f);
        //Debug.LogWarning("Jump Force Applied");
        yield return new WaitForSeconds(1f);
        characterMovementScript.jumpExtraFrame = false;
    }
}
