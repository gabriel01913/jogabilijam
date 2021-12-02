using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelesteMovement : MonoBehaviour
{
    #region Variables/Constants
    [Header("Run / Jump")]
    [Header("Constants")]
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _jumpDuration = 10f;
    [SerializeField] private float _jumpMinSpeed = 1f;
    [SerializeField] private float _jumpMaxSpeed = 15f;
    [SerializeField] private float _jumpAcell = 8f;
    private float _jumpSpeed;
    private float _jumpTimer;    
    [Header("Dash")]
    [SerializeField] private float _dashSpeed = 10f;
    [SerializeField] float _dashDuration = 5f;
    private float _dashTimer;        
    [Header("Air Control")]
    [SerializeField] private float _fallMultiplier = 12f; //multiply by gravity
    [SerializeField] private float _airDecel = 5f;
    [Header("Ground Collision")]
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Vector2 _groundOffset = new Vector2(0f, -0.82f);
    [SerializeField] private float _groundRadius = 0.2f;
    [Header("Wall Collision")]
    [SerializeField] private float _wallSlideVelocity = 4f;
    [SerializeField] LayerMask _wallLayer;
    [SerializeField] Vector2 _wallLefOffset = new Vector2(-0.5f, -0.4f);
    [SerializeField] private float _wallLefRadius = 0.2f;
    [SerializeField] Vector2 _wallRightOffset = new Vector2(0.5f, -0.4f);
    [SerializeField] private float _wallRighRadius = 0.2f;
    [Header("Corner Correction")]
    [Space]
    [Header("QOF Parameters")]
    [SerializeField] private LayerMask _ccLayers;
    [SerializeField] private float _ccLenghtRayCast = 1.2f;
    [SerializeField] private Vector3 _ccEdegeRayCast = new Vector3(0.5f,0f,0f);
    [SerializeField] private Vector3 _ccInerRayCast = new Vector3(0.25f, 0f, 0f);
    private bool _cornerCorrection;
    [Header("Dash Correction Left")]
    [SerializeField] private LayerMask _dsLayers;
    [SerializeField] private float _dsLenghtRayCast = 1.5f;
    [SerializeField] private Vector3 _dsEdegeRayCast = new Vector3(0f, 0.75f, 0f);
    [SerializeField] private Vector3 _dsInerRayCast = new Vector3(0f, 0.55f, 0f);    
    [Header("Dash Correction Right")]
    [SerializeField] private float _dsRLenghtRayCast = 1.5f;
    [SerializeField] private Vector3 _dsREdegeRayCast = new Vector3(0f, 0.75f, 0f);
    [SerializeField] private Vector3 _dsRInerRayCast = new Vector3(0f, 0.55f, 0f);
    [Header("Coyte/Wall/Jump Timer")]
    [SerializeField] private float _coyoteTimer = .1f;// Or Hang Timer, in seconds
    private float _coyoteTimerCounter;
    [SerializeField] private float _jumpBufferTimer = .1f;// A little help to jump close to the ground, in seconds
    private float _jumpBufferCounter;
    [SerializeField] private float _wallJumpBufferTimer = .1f;// A little help to jump close to the wall, in seconds
    private float _wallJumpBufferCounter;
    [Header("Debug")]
    [Space]    
    [SerializeField] private Vector2 _velocity;
    public Vector3 _direction { get; private set; }
    [SerializeField] bool _onGround = true;
    [SerializeField] bool _canMove = true;
    [SerializeField] bool _canJump = true;
    [SerializeField] bool _jumping;
    [SerializeField] bool _dashing;
    [SerializeField] bool _wallSlide;
    [SerializeField] bool _wallJumping;
    [SerializeField] public bool _faceRight { get; private set; }
    [SerializeField] private bool _dashCorrection;
    [SerializeField] private bool _dashRCorrection;
    private float _horizontal;
    private float _vertical;
    Vector2 _wallDir;
    [SerializeField] bool _wallLeft;
    [SerializeField] bool _wallRight;
    #endregion

    //Components
    Rigidbody2D c_rigi2d;
    SpriteRenderer c_sprite;
    // Start is called before the first frame update
    private void Awake()
    {
        c_rigi2d = GetComponent<Rigidbody2D>();
        c_sprite = GetComponentInChildren<SpriteRenderer>();
        _faceRight = true;
    }

    private void Update()
    {        
        CheckInputs();        
    }

    private void FixedUpdate()
    {        
        CheckColission();
        PhysicManipulation();
        if (_cornerCorrection) CornerCorrection(_velocity.y);        
        if (_canMove) Running(_direction);
        if (_jumping) Jump(Vector2.up);
        if (_wallJumping) WallJump(_wallDir);
        if (_dashing) Dash(_direction);
        if (_wallSlide) WallSlide();
        
    }

    void CheckInputs()
    {
        if (_onGround)
        {
            _coyoteTimerCounter = _coyoteTimer;
        }
        else
        {
            _coyoteTimerCounter -= Time.fixedDeltaTime;
        }

        //running
        _velocity = c_rigi2d.velocity;
        _horizontal = Input.GetAxisRaw("Horizontal");
        if (_horizontal > -0.02f && _horizontal < 0.02f)
        {
            _horizontal = 0f;
        }
        _vertical = Input.GetAxisRaw("Vertical");
        _direction = new Vector2(_horizontal, _vertical);

        //jump
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

        if (_jumpBufferCounter > 0 && _coyoteTimerCounter > 0)
        {
            _jumpTimer = 0f;
            _jumping = true;
        }

        //Walljump
        if (!_onGround && (_wallLeft || _wallRight) && !_jumping && _wallJumpBufferCounter > 0)
        {
            _jumpTimer = 0f;
            _wallJumping = true;
            _wallDir = _wallLeft ? Vector2.right : Vector2.left;
        }

        //dash
        if (Input.GetButtonDown("Dash") && !_dashing)
        {
            _direction = new Vector2(_horizontal, _vertical);
            _dashTimer = 0f;
            _dashing = true;
        }


        //Flip
        if (_canMove && _horizontal > 0 && !_faceRight)
        {
            Flip();
        }
        else if (_canMove && _horizontal < 0 && _faceRight)
        {
            Flip();
        }

        //wallslide
        if((_wallLeft && _velocity.y < 0 && _horizontal < 0 || _wallRight && _velocity.y < 0 && _horizontal > 0) && !_wallJumping)
        {
            _wallSlide = true;
        }
        else
        {
            _wallSlide = false;
        }

    }
    #region Running/Jumping
    void Running(Vector2 dir)
    {
        if (!_wallJumping)
        {
            c_rigi2d.velocity = new Vector2(dir.x * _runSpeed, _velocity.y);
        }
        else
        {
            c_rigi2d.velocity = Vector2.Lerp(c_rigi2d.velocity, (new Vector2(dir.x * _runSpeed, _velocity.y)), 10f * Time.deltaTime);
        }
      
    }

    void Jump(Vector2 dir)
    {
        if (!_canJump)
        {
            return;
        }
        if (!_wallJumping)
        {
            if (Input.GetButton("Jump")) _jumpTimer += 1f;
            float jumpforce = Mathf.Clamp(_jumpSpeed + _jumpTimer, _jumpMinSpeed, _jumpMaxSpeed);
            c_rigi2d.velocity = new Vector2(_velocity.x, jumpforce * dir.y + _jumpAcell);
            if (_jumpTimer >= _jumpDuration || !Input.GetButton("Jump"))
            {
                _jumping = false;
            }
        }
        else
        {
            if (Input.GetButton("Jump")) _jumpTimer += 1f;
            float jumpforce = Mathf.Clamp(_jumpSpeed + _jumpTimer, _jumpMinSpeed, _jumpMaxSpeed);
            c_rigi2d.velocity = new Vector2(_velocity.x, 0);
            dir = (dir + Vector2.up) * (jumpforce + _jumpAcell);            
            c_rigi2d.velocity = dir;
            if (_jumpTimer >= _jumpDuration || !Input.GetButton("Jump"))
            {
                _wallJumping = false;
            }
        }        
    }
    #endregion
    #region Wall Interactions
    private void WallSlide()
    {
        c_rigi2d.velocity = new Vector2(_velocity.x, _wallSlideVelocity * -1);       
    }

    void WallJump(Vector2 dir)
    {
        StartCoroutine(DisableMove(0.3f));               
        Jump(dir);
    }
    #endregion
    #region Dash Methods
    void Dash(Vector2 dir)
    {
        if(dir.x == 0 && dir.y == 0 && _faceRight)
        {
            dir.x = 1;
        }else if(dir.x == 0 && dir.y == 0 && !_faceRight)
        {
            dir.x = -1;
        }        
        StartCoroutine(DisableMove(0.2f));
        ApplyDash(dir, 0.5f);
        if (_dashing && (_dashCorrection || _dashRCorrection) && dir.y == 0) DashCorrection(_velocity.x);

    }
    IEnumerator DisableMove(float time)
    {
        _canMove = false;
        yield return new WaitForSeconds(time);
        _canMove = true;
        yield break;
    }

    void ApplyDash(Vector2 dir, float time)
    {
        c_rigi2d.gravityScale = 0f;
        _dashTimer += 1;
        if(_dashTimer < 2f)
        {
            c_rigi2d.velocity = Vector2.zero;
        }
        else if(_dashTimer < _dashDuration )
        {
            c_rigi2d.velocity += dir * _dashSpeed;
        }
        else
        {           
            _dashing = false;
        }       
    }
    #endregion
    #region Physic Stuffs
    void CheckColission()
    {
        _onGround = Physics2D.OverlapCircle((Vector2)transform.position + _groundOffset, _groundRadius, _groundLayer);
        _wallLeft = Physics2D.OverlapCircle((Vector2)transform.position + _wallLefOffset, _wallLefRadius, _wallLayer);
        _wallRight = Physics2D.OverlapCircle((Vector2)transform.position + _wallRightOffset, _wallRighRadius, _wallLayer);
        _cornerCorrection = Physics2D.Raycast(transform.position + _ccEdegeRayCast, Vector2.up, _ccLenghtRayCast, _ccLayers) &&
                            !Physics2D.Raycast(transform.position + _ccInerRayCast, Vector2.up, _ccLenghtRayCast, _ccLayers) ||
                            Physics2D.Raycast(transform.position - _ccEdegeRayCast, Vector2.up, _ccLenghtRayCast, _ccLayers) &&
                            !Physics2D.Raycast(transform.position - _ccInerRayCast, Vector2.up, _ccLenghtRayCast, _ccLayers);
        _dashCorrection = Physics2D.Raycast(transform.position + _dsEdegeRayCast, Vector2.left, _dsLenghtRayCast, _dsLayers) &&
                         !Physics2D.Raycast(transform.position + _dsInerRayCast, Vector2.left, _dsLenghtRayCast, _dsLayers) ||
                          Physics2D.Raycast(transform.position - _dsEdegeRayCast, Vector2.left, _dsLenghtRayCast, _dsLayers) &&
                         !Physics2D.Raycast(transform.position - _dsInerRayCast, Vector2.left, _dsLenghtRayCast, _dsLayers);
        _dashRCorrection = Physics2D.Raycast(transform.position + _dsREdegeRayCast, Vector2.right, _dsRLenghtRayCast, _dsLayers) &&
                          !Physics2D.Raycast(transform.position + _dsRInerRayCast, Vector2.right, _dsRLenghtRayCast, _dsLayers) ||
                           Physics2D.Raycast(transform.position - _dsREdegeRayCast, Vector2.right, _dsRLenghtRayCast, _dsLayers) &&
                          !Physics2D.Raycast(transform.position - _dsRInerRayCast, Vector2.right, _dsRLenghtRayCast, _dsLayers);
    }    

    void PhysicManipulation()
    {
        if (!_onGround)
        {
            c_rigi2d.gravityScale = 1f * _fallMultiplier;
            c_rigi2d.drag = _airDecel;
        }
        else
        {
            c_rigi2d.gravityScale = 1f;
            c_rigi2d.drag = 0f;
        }

        if (_jumping || _dashing || _wallSlide)
        {
            c_rigi2d.gravityScale = 0f;
        }

        if (_dashing)
        {
            c_rigi2d.drag = 0f;
        } 
    }
    #endregion
    #region QOL Stuffs
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
    private void DashCorrection(float velocityX)
    {
        RaycastHit2D _hit = Physics2D.Raycast(transform.position - _dsInerRayCast + Vector3.left * _dsLenghtRayCast, Vector3.down, _dsLenghtRayCast, _dsLayers);
        if (_hit.collider != null)
        {
            float _newPosition = Vector3.Distance(new Vector3(_hit.point.x, transform.position.y, 0f) + Vector3.left * _dsLenghtRayCast,
                transform.position - _dsEdegeRayCast + Vector3.left * _dsLenghtRayCast);
            Debug.Log(_newPosition);
            transform.position = new Vector3(transform.position.x, transform.position.y + _newPosition - 2f, transform.position.z);
            c_rigi2d.velocity = new Vector2(velocityX, c_rigi2d.velocity.y);
            return;
        }

        _hit = Physics2D.Raycast(transform.position + _dsInerRayCast + Vector3.left * _dsLenghtRayCast, Vector3.up, _dsLenghtRayCast, _dsLayers);
        if (_hit.collider != null)
        {
            float _newPosition = Vector3.Distance(new Vector3(_hit.point.x, transform.position.y, 0f) + Vector3.left * _dsLenghtRayCast,
                transform.position + _dsEdegeRayCast + Vector3.left * _dsLenghtRayCast);
            Debug.Log(_newPosition);
            transform.position = new Vector3(transform.position.x , transform.position.y - _newPosition - 2f, transform.position.z);
            c_rigi2d.velocity = new Vector2(velocityX, c_rigi2d.velocity.y);
        }

        _hit = Physics2D.Raycast(transform.position - _dsRInerRayCast + Vector3.right * _dsRLenghtRayCast, Vector3.up, _dsRLenghtRayCast, _dsLayers);
        if (_hit.collider != null)
        {
            float _newPosition = Vector3.Distance(new Vector3(_hit.point.x, transform.position.y, 0f) + Vector3.right * _dsRLenghtRayCast,
                transform.position - _dsREdegeRayCast + Vector3.right * _dsRLenghtRayCast);
            Debug.Log(_newPosition);
            transform.position = new Vector3(transform.position.x, transform.position.y + _newPosition - 2f, transform.position.z);
            c_rigi2d.velocity = new Vector2(velocityX, c_rigi2d.velocity.y);
            return;
        }

        _hit = Physics2D.Raycast(transform.position + _dsInerRayCast + Vector3.right * _dsLenghtRayCast, Vector3.down, _dsRLenghtRayCast, _dsLayers);
        if (_hit.collider != null)
        {
            float _newPosition = Vector3.Distance(new Vector3(_hit.point.x, transform.position.y, 0f) + Vector3.left * _dsRLenghtRayCast,
                transform.position + _dsREdegeRayCast + Vector3.left * _dsRLenghtRayCast);
            Debug.Log(_newPosition);
            transform.position = new Vector3(transform.position.x, transform.position.y - _newPosition - 2f, transform.position.z);
            c_rigi2d.velocity = new Vector2(velocityX, c_rigi2d.velocity.y);
        }
    }
    private void Flip()
    {
        if (_faceRight)
        {
            c_sprite.flipX = true;
            _faceRight = false;
        }
        else
        {
            c_sprite.flipX = false;
            _faceRight = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + _groundOffset, _groundRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + _wallLefOffset, _wallLefRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + _wallRightOffset, _wallRighRadius);

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


        //dash check left
        Gizmos.DrawLine(transform.position + _dsEdegeRayCast, transform.position + _dsEdegeRayCast + Vector3.left * _dsLenghtRayCast);
        Gizmos.DrawLine(transform.position - _dsEdegeRayCast, transform.position - _dsEdegeRayCast + Vector3.left * _dsLenghtRayCast);
        Gizmos.DrawLine(transform.position + _dsInerRayCast, transform.position + _dsInerRayCast + Vector3.left * _dsLenghtRayCast);
        Gizmos.DrawLine(transform.position - _dsInerRayCast, transform.position - _dsInerRayCast + Vector3.left * _dsLenghtRayCast);

        //dash distance check left
        Gizmos.DrawLine(transform.position - _dsInerRayCast + Vector3.left * _dsLenghtRayCast,
                        transform.position - _dsInerRayCast + Vector3.left * _dsLenghtRayCast + Vector3.down * _dsLenghtRayCast);
        Gizmos.DrawLine(transform.position + _dsInerRayCast + Vector3.left * _dsLenghtRayCast,
                        transform.position + _dsInerRayCast + Vector3.left * _dsLenghtRayCast + Vector3.up * _dsLenghtRayCast);

        //dash check right
        Gizmos.DrawLine(transform.position + _dsREdegeRayCast, transform.position + _dsREdegeRayCast + Vector3.right * _dsRLenghtRayCast);
        Gizmos.DrawLine(transform.position - _dsREdegeRayCast, transform.position - _dsREdegeRayCast + Vector3.right * _dsRLenghtRayCast);
        Gizmos.DrawLine(transform.position + _dsRInerRayCast, transform.position + _dsRInerRayCast + Vector3.right * _dsRLenghtRayCast);
        Gizmos.DrawLine(transform.position - _dsRInerRayCast, transform.position - _dsRInerRayCast + Vector3.right * _dsRLenghtRayCast);

        //dash distance check right
        Gizmos.DrawLine(transform.position - _dsRInerRayCast + Vector3.right * _dsRLenghtRayCast,
                        transform.position - _dsRInerRayCast + Vector3.right * _dsRLenghtRayCast + Vector3.down * _dsRLenghtRayCast);
        Gizmos.DrawLine(transform.position + _dsRInerRayCast + Vector3.right * _dsRLenghtRayCast,
                        transform.position + _dsRInerRayCast + Vector3.right * _dsRLenghtRayCast + Vector3.up * _dsRLenghtRayCast);
    }
    #endregion
}
