using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ZombieController : PlayerController
{



    public override void AnimateThePlayer(Vector3 desiredDirection)
    {
        Debug.Log("magnitude is " + moveInput.magnitude);
        if(moveInput.magnitude != 0)
        {
            playerAnimator.SetBool("IsMoving", true);


        }
        else
        {
            playerAnimator.SetBool("IsMoving", false);

        }

    }


    public override void OnAttack(InputAction.CallbackContext value)
    {
        base.OnAttack(value);

        if (value.canceled)
        {
            items[currentItemIndex].FinishedUse();

        }
    }



}
