using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
  CelesteMovement _celeste;
  Animator _animator;
  TorchThrowBehavior torchThrowBehavior;

  bool jump = false;
  bool throwTorch = false;

  // Start is called before the first frame update
  private void Awake()
  {
    _celeste = GetComponent<CelesteMovement>();
    _animator = GetComponent<Animator>();
    torchThrowBehavior = GetComponentInChildren<TorchThrowBehavior>();
  }

  // Update is called once per frame
  void FixedUpdate()
  {
    AnimationUpdate();
  }

  void AnimationUpdate()
  {

    _animator.SetFloat("isWithTorch", torchThrowBehavior.HasThrow ? 0 : 1);
    _animator.SetFloat("velocityX", _celeste._velocity.x);
    _animator.SetFloat("velocityY", _celeste._velocity.y);
    _animator.SetBool("isDashing", _celeste._dashing);
    _animator.SetBool("isInGround", _celeste._onGround);



    if (torchThrowBehavior.HasThrow)
    {
      if (!throwTorch)
      {
        //on throw
        _animator.SetTrigger("onTorchThrow");
        throwTorch = true;
      }
    }
    else
    {
      throwTorch = false;
    }

    if (_celeste._jumping)
    {
      if (!jump)
      {
        jump = true;
        _animator.SetTrigger("onJump");
      }
    }
    else
    {
      jump = false;
    }
  }

}
