using System;
using UnityEngine;
using UnityEngine.InputSystem;

//animation controller
public class AnimatorController : MonoBehaviour
{
    private Animator animator;

    private bool _isJump;
    private bool _isWalking;
    private bool _isrunning;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void GetPlayerMoveInput(InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>();
        _isWalking = input.magnitude > 0; 
    }
    
    public void GetJumpInput(InputAction.CallbackContext ctx)
    {
        _isJump = ctx.ReadValueAsButton();
    }
    
    public void GetRunningInput(InputAction.CallbackContext ctx)
    {
        _isrunning = ctx.ReadValueAsButton();
    }
    private void Update()
    {
        animator.SetBool("isWalking",_isWalking);
        if(_isrunning) animator.SetFloat("speed",2);
        else animator.SetFloat("speed",1);
        
        animator.SetBool("isJumping",_isJump);
    }

}