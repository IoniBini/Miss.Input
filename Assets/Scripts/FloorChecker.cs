using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorChecker : MonoBehaviour
{
    public bool isGrounded = false;

    private void OnTriggerStay(Collider other)
    {
        //while you are touching the floor, sets it to true every frame
        if (other.gameObject.layer == 6 && other.gameObject.tag != "Player")
        {
            isGrounded = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //disables upon leaving the floor
        if (other.gameObject.layer == 6 && other.gameObject.tag != "Player")
        {
            isGrounded = false;
        }
    }
}
