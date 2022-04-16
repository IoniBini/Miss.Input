using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDetector : MonoBehaviour
{
    private BoxCollider shieldCollider;
    public CharacterMovement characterMovement;

    private void Awake()
    {
        shieldCollider = GetComponent<BoxCollider>();
        characterMovement = GameObject.Find("xbot").GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        //prevent damage while defending
        if (characterMovement.isDefending == true)
        {
            characterMovement.preventDamage = true;
        }
        else if (characterMovement.isDefending == false)
        {
            characterMovement.preventDamage = false;
        }
    }
} 
