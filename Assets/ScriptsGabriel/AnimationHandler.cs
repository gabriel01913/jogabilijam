using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{ 
    CelesteMovement _celeste;
    Animator _animator;
    SpriteRenderer _spriteRender;

    public Sprite jump1;
    public Sprite jump2;
    public Sprite jump3;
    // Start is called before the first frame update
    private void Awake()
    {
        _celeste = GetComponent<CelesteMovement>();
        _animator = GetComponent<Animator>();
        _spriteRender= GetComponent<SpriteRenderer>(); 
    }
    void Start()
    {
        _spriteRender.sprite = jump1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AnimationUpdate();
    }

    void AnimationUpdate()
    {
        if (_celeste._jumping)
        {
            Jumping();
        }else if(_celeste._onGround && _celeste._velocity.x != 0 && !_celeste._dashing)
        {
            _animator.Play("Run");
        }
        else
        {
            _animator.Play("New State");
        }
    }

    void Jumping()
    {
        if(_celeste._jumpTimer < 3f)
        {
            _spriteRender.sprite = jump1;
        }else if(_celeste._jumpTimer > 3f)
        {
            _spriteRender.sprite = jump2;
        }else if(_celeste._velocity.y < 0f)
        {
            _spriteRender.sprite = jump3;
        }
    }
}
