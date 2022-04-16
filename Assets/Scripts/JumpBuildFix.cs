using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBuildFix : MonoBehaviour
{
    private CharacterMovement characterMovementScript;

    private void Start()
    {
        characterMovementScript = GetComponent<CharacterMovement>();
    }

    public IEnumerator Jump()
    {
        characterMovementScript.jumpExtraFrame = true;
        characterMovementScript.GetComponent<Rigidbody>().AddForce(0f, characterMovementScript.jumpStrenght, 0f);
        yield return new WaitForSeconds(0.1f);
        characterMovementScript.jumpExtraFrame = false;
    }
}
