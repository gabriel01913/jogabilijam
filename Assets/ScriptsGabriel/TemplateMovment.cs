using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TemplateMovemente : MonoBehaviour
{
    #region Variables
    [Header("Running Settings")]
    [Header("---Variables---")]   
    [SerializeField] private float _accel = 50f; //multiply velocity
    [SerializeField] private float _maxSpeed = 6f; //clamp
    [SerializeField] private float _decel = 12f; // set lineardrag
    [SerializeField] private float _decelThreshold = 0.4f;
    [Header("Jumping Settings")]
    [Range(0f, 10f)]
    [SerializeField] private float _jumpAccel = 2f; //Addtive
    [SerializeField] private float _airTimeMax = 12f; //Frames
    [SerializeField] private float _airTimeMin = 1f; //Frames
    [SerializeField] private float _velociTyMax = 10f; //Clamp
    [SerializeField] private float _velociTyMin = 1f; //Clamp
    private float _jumpForce;
    private float _jumpingTime;
    private float _airTime;
    [Header("Dashing Settings")]
    [SerializeField] private float _dashVelocity = 15f; //Addtive
    [SerializeField] private float _dashVelocityAir = 10f; //Addtive    
    [SerializeField] private float _dashDuration = 25f;
    [SerializeField] private float _dashDurationAir = 15f;
    [SerializeField] private short _dashAirCounter = 1;
    private float _dashDurationCounter;
    private float _dashingTime;
    [Header("Air Control")]
    [SerializeField] private float _fallMultiplier = 12f; //multiply by gravity
    [SerializeField] private float _decelAir = 5f; //set lineardrag    
    [Header("Wall Settings")]
    [SerializeField] private float _wallSlideVelocity = 4f; //set velocity
    [SerializeField] private float _wallJumpTimeMax = 10f; //frames
    [SerializeField] private float _wallJumpVelocity = 12f; //set velocity
    [SerializeField] private float _wallJumpRecoyDuration = 4f; //frame
    private float _wallJumpRecoyCounter;
    private float _wallJumpRecoy;
    [Header("QOL Setings")] 
    [SerializeField] private float _coyoteTimer = .1f;// Or Hang Timer, in seconds
    private float _coyoteTimerCounter;  
    [SerializeField] private float _jumpBufferTimer = .1f;// A little help to jump close to the ground, in seconds
    private float _jumpBufferCounter;
    [SerializeField] private float _wallJumpBufferTimer = .1f;// A little help to jump close to the wall, in seconds
    private float _wallJumpBufferCounter;
    [Header("Layers Check")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;
    [Range(0f, 2f)]
    [SerializeField] private float _groundLenght = 1f;    
    [SerializeField] private Vector2 _wallSize = new Vector2(1f, 1f);
    [SerializeField] private Vector3 _wallPosition = new Vector3(0f, 0f, 0f);
    [SerializeField] private float _wallDistance = 0.5f;
    [Header("Corner Correction")]
    [SerializeField] private LayerMask _ccLayers;
    [SerializeField] private float _ccLenghtRayCast;   
    [SerializeField] private Vector3 _ccEdegeRayCast;
    [SerializeField] private Vector3 _ccInerRayCast;
    private bool _cornerCorrection;
    [Header("States")]
    [Header("---Debug---")]    
    [SerializeField] private bool _facinRight = true;
    [SerializeField] private bool _onGround = true;
    [SerializeField] private bool _onAir = false;
    [SerializeField] private bool _canMove = true;
    [SerializeField] private bool _canJump = true;
    [SerializeField] private bool _canDash = true;
    [SerializeField] private bool _jumping = false;
    [SerializeField] private bool _wallLeft = false;
    [SerializeField] private bool _wallRight = false;
    [SerializeField] private bool _wallSlide = false;
    [SerializeField] private bool _wallJumping = false;
    [SerializeField] private bool _dashing = false;
    [SerializeField] private bool _changeDirection => (c_rigi2d.velocity.x > 0f && _horizontal < 0f || c_rigi2d.velocity.x < 0f && _horizontal > 0f); 
    [Header("Variables")]
    [SerializeField] private float _horizontal;
    [SerializeField] private Vector2 velocity;
    #endregion
    #region Components
    //Components
    Rigidbody2D c_rigi2d;
    SpriteRenderer c_sprite;
    #endregion

    // Start is called before the first frame update
    private void Awake()
    {
        c_rigi2d = GetComponent<Rigidbody2D>();
        c_sprite = GetComponentInChildren<SpriteRenderer>();
    }
    // Update is called once per frame
    private void Update()
    {        
        velocity = c_rigi2d.velocity;
        CheckCollision();
        CheckStates();
        CheckInputs();
    }
    //Fixed update is caleed 50 times per second
    private void FixedUpdate()
    {
        GravityManipulation();
        if (_cornerCorrection) CornerCorrection(velocity.y);
        if(_canMove) Running();                
        ApplyLinearDrag();
        if (_jumping) Jumping();
        if (_wallSlide) WallSlide();
        if (_wallJumping) StartCoroutine(WallJump());
        if (_dashing) StartCoroutine(Dash());
    }
    //Check Player Inputs
    private void CheckInputs()
    {
        //Jump inputs
        if (Input.GetButtonDown("Jump"))
        {
            _jumpBufferCounter = _jumpBufferTimer;
            _wallJumpBufferCounter = _wallJumpBufferTimer;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
            _wallJumpBufferCounter -= Time.deltaTime;
        }

        if (_jumpBufferCounter > 0 && _canJump && _coyoteTimerCounter > 0){
            _dashing = false;
            _jumping = true;            
        }

        //running inputs
        _horizontal = Input.GetAxisRaw("Horizontal");
        if(_horizontal > -0.02f && _horizontal < 0.02f)
        {
            _horizontal = 0f;
        }        

        //wallslide inputs
        if (!_onGround && velocity.y < 0 && (_wallLeft || _wallRight) && _horizontal != 0)
        {
            _wallSlide = true;
        }
        else
        {
            _wallSlide = false;
        }

        //walljump inputs
        if (!_jumping && !_onGround && (_wallLeft || _wallRight) && _wallJumpBufferCounter > 0 && !_dashing)
        {          
            _wallJumping = true;            
        }

        //dash
        if (Input.GetButtonDown("Dash") && _canDash && _dashAirCounter > 0 && !_dashing)
        {
            _dashing = true;
            _dashingTime = 0f;
            _dashDurationCounter = 1f;
            _dashAirCounter = 0;
        }
        //Flip sprite
        if (_canMove && _horizontal > 0 && !_facinRight)
        {
            Flip() ;
        }else if (_canMove && _horizontal < 0 && _facinRight)
        {
            Flip();
        }
    }
    #region Running/Jumping
    //Running method
    private void Running()
    {
        c_rigi2d.AddForce(new Vector2(_horizontal, 0f) * _accel);        
        if(Mathf.Abs(velocity.x) > _maxSpeed)
        {
            c_rigi2d.velocity = new Vector2(Mathf.Sign(velocity.x) * _maxSpeed, velocity.y);
        }
    }    
    //Jumping method
    private void Jumping()
    {
        if (_jumping)
        {
            c_rigi2d.gravityScale = 0f;
            _coyoteTimerCounter = 0f;
            _jumpBufferCounter = 0f;
            //holding button increase airTime per frame
            if (Input.GetButton("Jump"))
            {
                _jumpingTime += 1f;
            }
            if (_jumpingTime < _airTimeMin)
            {
                _jumpingTime = _airTimeMin;
            }
            //Force of jump is incresed by frames and accel, this make rising fast, but clamp at max speed
            _jumpForce = Mathf.Clamp(_jumpAccel + _jumpingTime, _velociTyMin, _velociTyMax);
            c_rigi2d.velocity = new Vector2(c_rigi2d.velocity.x, _jumpForce);
            // cut jump if button is realsed
            _airTime += 1f;
            if (_airTime > _jumpingTime || _airTime > _airTimeMax)
            {
                _jumping = false;
            }
        }
    }
    #endregion
    #region Wall Interaction
    //WallSlide
    private void WallSlide()
    {
        _dashAirCounter = 1;
        if (_wallLeft && _wallLeft && _horizontal < 0)
        {            
            c_rigi2d.velocity = new Vector2(velocity.x, _wallSlideVelocity * -1);
            
        }
        else if(!_wallLeft && _wallRight && _horizontal > 0)
        {
            c_rigi2d.velocity = new Vector2(velocity.x, _wallSlideVelocity * -1);
        }
    }
    //WallJumping
    IEnumerator WallJump()
    {
        if (_wallJumping)
        {
            c_rigi2d.gravityScale = 0f;
            c_rigi2d.velocity = new Vector2(0f, 0f);
            _wallJumpRecoy = _maxSpeed;            
            if(_wallJumpRecoyCounter < _wallJumpRecoyDuration && _facinRight)
            {
                c_rigi2d.velocity = new Vector2(-1 * _wallJumpRecoy, _wallJumpVelocity);
            }
            else if(_wallJumpRecoyCounter < _wallJumpRecoyDuration && !_facinRight)
            {
                c_rigi2d.velocity = new Vector2(_wallJumpRecoy, _wallJumpVelocity);
            }else
            {                
                _canMove = true;
                _canDash = true;
                _dashAirCounter = 1;
                if (_dashing)
                {
                    _wallJumping = false;
                    yield break;
                }
                c_rigi2d.velocity = new Vector2(c_rigi2d.velocity.x, _wallJumpVelocity);
            }
            _wallJumpRecoyCounter += 1f;            
            // cut jump if button is realsed
            if (_wallJumpRecoyCounter >= _wallJumpTimeMax)
            {
                _wallJumping = false;
            }
        }
        yield return null;
    }
    #endregion
    #region Dashing
    IEnumerator Dash()
    {
        if (_facinRight && (_horizontal < 0 || _wallRight))
        {
            _dashing = false;
            yield break;
        }else if(!_facinRight && (_horizontal > 0 || _wallLeft))
        {
            _dashing = false;
            yield break;
        }        
        if (_dashing)
        {
            c_rigi2d.gravityScale = 0f;
            c_rigi2d.velocity = Vector2.zero;
            if (_onGround)
            {
                if (Input.GetButton("Dash"))
                {
                    _dashDurationCounter += 1f;
                }
                if (_facinRight && _dashDurationCounter > 0f)
                {
                    c_rigi2d.velocity = new Vector2(_dashVelocity, 0f);
                }
                else
                {
                    c_rigi2d.velocity = new Vector2(-1 * _dashVelocity, 0f);
                }
                if (_dashingTime > _dashDurationCounter || _dashingTime > _dashDuration)
                {
                    _dashing = false;
                }

            }
            else
            {
                if (_dashingTime < 2f)
                {
                    c_rigi2d.velocity = Vector2.zero;
                }
                else if (_facinRight && _dashDurationCounter > 0f)
                {
                    c_rigi2d.velocity = new Vector2(_dashVelocityAir, 0f);
                }
                else
                {
                    c_rigi2d.velocity = new Vector2(-1 * _dashVelocityAir, 0f);
                }                
                if (_dashingTime > _dashDurationAir)
                {
                    _dashing = false;
                }
            }           
        }
        _dashingTime += 1f;
        yield return null;
    }
    #endregion
    #region Physic Manipulation
    //Gravity Manipulation
    private void GravityManipulation()
    {
        if (_onAir && velocity.y < 0 && !_wallSlide)
        {
            c_rigi2d.gravityScale = 1f * _fallMultiplier;
        }else
        {
            c_rigi2d.gravityScale = 1f;
        }
    }
    //LinearDrag Control
    private void ApplyLinearDrag()
    {
        //linear drag to decelerate a little the character
        if (_onGround && Mathf.Abs(_horizontal) < _decelThreshold || _changeDirection && !_dashing)
        {
            c_rigi2d.drag = _decel;
        }
        else if (!_onGround && !_dashing)
        {
            c_rigi2d.drag = _decelAir;
        }else
        {
            c_rigi2d.drag = 0f;
        }
    }
    #endregion
    #region Control States
    //Controle of states
    private void CheckStates()
    {
        if (_wallSlide || (_facinRight && _wallRight) || (!_facinRight && _wallLeft) && !_onGround && !_canMove)
        {
            _canDash = false;
        }
        else
        {
            _canDash = true;
        }

        if (_onGround)
        {
            _airTime = 0;
            _jumpingTime = 0;
            _canJump = true;
            _coyoteTimerCounter = _coyoteTimer;
            _wallSlide = false;
            _dashAirCounter = 1;
        }
        else
        {
            _coyoteTimerCounter -= Time.deltaTime;
        }

        if (!_onGround && velocity.y != 0)
        {
            _onAir = true;
        }
        else
        {
            _onAir = false;
        }
    }
    //Collision
    private void CheckCollision()
    {
        _onGround = Physics2D.Raycast(transform.position * _groundLenght, Vector2.down, _groundLenght, _groundLayer);

        _cornerCorrection = Physics2D.Raycast(transform.position + _ccEdegeRayCast, Vector2.up, _ccLenghtRayCast, _ccLayers) &&
                            !Physics2D.Raycast(transform.position + _ccInerRayCast, Vector2.up, _ccLenghtRayCast, _ccLayers) ||
                            Physics2D.Raycast(transform.position - _ccEdegeRayCast, Vector2.up, _ccLenghtRayCast, _ccLayers) &&
                            !Physics2D.Raycast(transform.position - _ccInerRayCast, Vector2.up, _ccLenghtRayCast, _ccLayers);
        _wallLeft = Physics2D.BoxCast(transform.position + new Vector3(-1 *_wallPosition.x, _wallPosition.y, _wallPosition.z), _wallSize, 0f, Vector3.left, _wallDistance ,_wallLayer);
        _wallRight = Physics2D.BoxCast(transform.position + _wallPosition, _wallSize, 0f, Vector3.right, _wallDistance, _wallLayer);
    }
    #endregion
    #region QOL Methods
    //No bonk head method
    private void CornerCorrection(float velocityY)
    {
        RaycastHit2D _hit = Physics2D.Raycast(transform.position - _ccInerRayCast + Vector3.up * _ccLenghtRayCast, Vector3.left, _ccLenghtRayCast, _ccLayers);
        if (_hit.collider != null)
        {
            float _newPosition = Vector3.Distance(new Vector3(_hit.point.x, transform.position.y, 0f) + Vector3.up * _ccLenghtRayCast,
                transform.position - _ccEdegeRayCast + Vector3.up * _ccLenghtRayCast);
            transform.position = new Vector3(transform.position.x + _newPosition, transform.position.y, transform.position.z);
            c_rigi2d.velocity = new Vector2(c_rigi2d.velocity.x, velocityY);
            return;
        }

        _hit = Physics2D.Raycast(transform.position + _ccInerRayCast + Vector3.up * _ccLenghtRayCast, Vector3.right, _ccLenghtRayCast, _ccLayers);
        if (_hit.collider != null)
        {
            float _newPosition = Vector3.Distance(new Vector3(_hit.point.x, transform.position.y, 0f) + Vector3.up * _ccLenghtRayCast,
                transform.position + _ccEdegeRayCast + Vector3.up * _ccLenghtRayCast);
            transform.position = new Vector3(transform.position.x - _newPosition, transform.position.y, transform.position.z);
            c_rigi2d.velocity = new Vector2(c_rigi2d.velocity.x, velocityY);
        }
    }
    private void Flip()
    {
        if (_facinRight)
        {
            c_sprite.flipX = true;
            _facinRight = false;
        }else
        {
            c_sprite.flipX = false;
            _facinRight = true;
        }
    }
    private void OnDrawGizmos()
    {
        //ground check
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _groundLenght);

        //wall check
        Gizmos.DrawCube(transform.position + new Vector3(-1 * _wallPosition.x, _wallPosition.y, _wallPosition.z), _wallSize);
        Gizmos.DrawCube(transform.position + _wallPosition, _wallSize);

        //corner check
        Gizmos.DrawLine(transform.position + _ccEdegeRayCast, transform.position + _ccEdegeRayCast + Vector3.up * _ccLenghtRayCast);
        Gizmos.DrawLine(transform.position - _ccEdegeRayCast, transform.position - _ccEdegeRayCast + Vector3.up * _ccLenghtRayCast);
        Gizmos.DrawLine(transform.position + _ccInerRayCast, transform.position + _ccInerRayCast + Vector3.up * _ccLenghtRayCast);
        Gizmos.DrawLine(transform.position - _ccInerRayCast, transform.position - _ccInerRayCast + Vector3.up * _ccLenghtRayCast);

        //corner distance check
        Gizmos.DrawLine(transform.position - _ccInerRayCast + Vector3.up * _ccLenghtRayCast,
                        transform.position - _ccInerRayCast + Vector3.up * _ccLenghtRayCast + Vector3.left * _ccLenghtRayCast);
        Gizmos.DrawLine(transform.position + _ccInerRayCast + Vector3.up * _ccLenghtRayCast,
                        transform.position + _ccInerRayCast + Vector3.up * _ccLenghtRayCast + Vector3.right * _ccLenghtRayCast);
    }
    #endregion
}
