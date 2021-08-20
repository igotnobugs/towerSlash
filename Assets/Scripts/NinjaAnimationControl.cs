using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaAnimationControl : MonoBehaviour 
{

    private Animator animator;

    private void Awake() 
	{
        animator = GetComponent<Animator>();
    }

    public void SetIdle() {
        animator.SetBool("isMoving", false);
    }

    public void SetRunning() {
        animator.SetBool("isSlashAttack", false);
        animator.SetBool("isMoving", true);
    }

    public void SetSlashing() {
        animator.SetBool("isSlashAttack", true);
    }

    public void SetJumpSlash() {
        //.SetBool("isJumpAttack", true);
        //.GetCurrentAnimatorStateInfo(0).
    }
}
