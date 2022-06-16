using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator
{
    public void OnPunch(Animator animator)
    {
        animator.SetBool("IsAttack", true);
    }
}
