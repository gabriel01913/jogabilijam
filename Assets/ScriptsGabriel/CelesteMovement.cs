using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelesteMovement : MonoBehaviour
{
  #region Variables/Constants
  [Header("Run / Jump")]
  [Header("Constants")]
  [SerializeField] private float _runSpeed = 7f;
  [SerializeField] private float _jumpDuration = 8f;
  [SerializeField] private float _jumpMinSpeed = 15f;
  [SerializeField] private float _jumpMaxSpeed = 18f;
  [SerializeField] private float _jumpAcell = 0f;
  private float _jumpSpeed;
  public float _jumpTimes = 2f;
  float _jumpCounter;
  [HideInInspector] public float _jumpTimer;
  [Header("Dash")]
  [SerializeField] private float _dashSpeed = 5f;
  [SerializeField] float _dashDuration = 7f;
  [SerializeField] public float _dashCounter = 1f;
  [SerializeField] public float _dashFrezeTime = 6f;
  private float _dashTimer;
  [Header("Air Control")]
  [SerializeField] private float _fallMultiplier = 12f; //multiply by gravity
  [SerializeField] private float _airDecel = 5f;
  [Header("Ground Collision")]
  [SerializeField] LayerMask _groundLayer;
  [SerializeField] Vector2 _groundOffset = new Vector2(0f, -1.83f);
  [SerializeField] Vector2 _groundSize = new Vector2(1f, 0.3f);
  [SerializeField] private float _groundDistance = 0f;
  [Header("Wall Collision")]
  [SerializeField] private float _wallSlideVelocity = 4f;
  [SerializeField] LayerMask _wallLayer;
  [SerializeField] Vector2 _wallLefOffset = new Vector2(-0.64f, -0.35f);
  [SerializeField] private Vector2 _wallLeftSize = new Vector2(0.2f, 1.96f);
  [SerializeField] private float _wallLeftDistance = 0f;
  [SerializeField] Vector2 _wallRightOffset = new Vector2(0.62f, -0.35f);
  [SerializeField] private Vector2 _wallRightSize = new Vector2(0.2f, 1.96f);
  [SerializeField] private float _wallRightDistance = 0f;
  [Header("Corner Correction")]
  [Space]
  [Header("QOF Parameters")]
  [SerializeField] private LayerMask _ccLayers;
  [SerializeField] private float _ccLenghtRayCast = 1.2f;
  [SerializeField] private Vector3 _ccEdegeRayCast = new Vector3(0.53f, 0f, 0f);
  [SerializeField] private Vector3 _ccInerRayCast = new Vector3(0.28f, 0f, 0f);
  [Header("Coyte/Wall/Jump Timer")]
  [SerializeField] private float _coyoteTimer = .1f;// Or Hang Timer, in seconds
  private float _coyoteTimerCounter;
  [SerializeField] private float _jumpBufferTimer = .1f;// A little help to jump close to the ground, in seconds
  private float _jumpBufferCounter;
  [SerializeField] private float _wallJumpBufferTimer = .1f;// A little help to jump close to the wall, in seconds
  private float _wallJumpBufferCounter;
  [SerializeField] private float _graceTime = 0.1f;
  [Header("Debug")]
  [Space]
  [SerializeField] bool _canMove = true;
  [SerializeField] bool _canJump = true;
  public bool _onGround;
  public bool _jumping;
  public bool _dashing;
  public bool _wallSlide;
  [SerializeField] bool _wallJumping;
  public bool _faceRight;
  [SerializeField] private bool _cornerCorrection;
  float _horizontal;
  float _vertical;
  float _aimHorizontal;
  float _aimVertical;
  Vector2 _wallDir;
  [SerializeField] bool _wallLeft;
  [SerializeField] bool _wallRight;
  public Vector2 _velocity;
  public Vector3 _direction;
  public Vector2 _aimDirection;
  Vector3 _mousePos;
  #endregion

  //Components
  InputHandler _Inputs;
  Rigidbody2D c_rigi2d;
  SpriteRenderer c_sprite;
  // Start is called before the first frame update
  private void Awake()
  {
    c_rigi2d = GetComponent<Rigidbody2D>();
    c_sprite = GetComponentInChildren<SpriteRenderer>();
    _Inputs = GetComponent<InputHandler>();
    _faceRight = true;
  }
  private void Update()
  {
    _Inputs.ButtonsStart();
    CheckInputs();
  }
  private void FixedUpdate()
  {
    CheckColission();
    PhysicManipulation();
    if (_cornerCorrection) CornerCorrection(c_rigi2d.velocity.y);
    //if (_dashCorrection && (_dashing || _hasDashedCounter > 0) && _velocity.y == 0f) DashCorrection(_velocity.x);
    if (_canMove) Running(_direction);
    if (_jumping) Jump(Vector2.up);
    if (_wallJumping) WallJump(_wallDir);
    if (_dashing) Dash(_direction);
    if (_wallSlide) WallSlide();

  }
  private void LateUpdate()
  {
    _Inputs.ButtonsCleanUp();
  }
  void CheckInputs()
  {
    //get velocity in the start of frame
    _velocity = c_rigi2d.velocity;

    if (_onGround || _jumpCounter > 0)
    {
      _coyoteTimerCounter = _coyoteTimer;
    }
    else
    {
      _coyoteTimerCounter -= Time.fixedDeltaTime;
    }

    //running
    _horizontal = _Inputs.GetAxisRaw("Horizontal");
    _vertical = _Inputs.GetAxisRaw("Vertical");
    _direction = new Vector2(_horizontal, _vertical);
    _aimHorizontal = _Inputs.GetAxisRaw("AimHorizontal");
    _aimVertical = _Inputs.GetAxisRaw("AimVertical");
    _aimDirection = new Vector2(_aimHorizontal, _aimVertical);

    //jump Input.GetButtonDown("Jump")
    if (_Inputs.GetButtonDown("Jump"))
    {
      _jumpBufferCounter = _jumpBufferTimer;
      _wallJumpBufferCounter = _wallJumpBufferTimer;
      _jumpCounter -= 1f;
    }
    else
    {
      _jumpBufferCounter -= Time.deltaTime;
      _wallJumpBufferCounter -= Time.deltaTime;
    }

    //Walljump
    if (!_onGround && (_wallLeft || _wallRight) && !_jumping && _wallJumpBufferCounter > 0)
    {
      _jumpTimer = 0f;
      _jumpCounter = 1;
      _wallJumping = true;
      _wallDir = _wallLeft ? Vector2.right : Vector2.left;
    }

    if (_jumpBufferCounter > 0 && _coyoteTimerCounter > 0)
    {
      _jumpTimer = 0f;
      _jumping = true;
    }


    //dash Input.GetButtonDown("Dash")
    if (_Inputs.GetButtonDown("Dash") && !_dashing && _dashCounter > 0f)
    {
      _dashCounter = 0f;
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
    if ((_wallLeft && _velocity.y <= 0 && _horizontal < 0 || _wallRight && _velocity.y <= 0 && _horizontal > 0) && !_wallJumping && !_onGround)
    {
      _jumping = false;
      _wallSlide = true;
      _jumpCounter = _jumpTimes;
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
      if (_Inputs.GetButton("Jump")) _jumpTimer += 1f;
      float jumpforce = Mathf.Clamp(_jumpSpeed + _jumpTimer, _jumpMinSpeed, _jumpMaxSpeed);
      c_rigi2d.velocity = new Vector2(_velocity.x, jumpforce * dir.y + _jumpAcell);
      if (_jumpTimer >= _jumpDuration || !_Inputs.GetButton("Jump"))
      {
        _jumping = false;
        StartCoroutine(GraceTime(_graceTime));

      }
    }
    else
    {
      if (_Inputs.GetButton("Jump")) _jumpTimer += 1f;
      float jumpforce = Mathf.Clamp(_jumpSpeed + _jumpTimer, _jumpMinSpeed, _jumpMaxSpeed);
      c_rigi2d.velocity = new Vector2(_velocity.x, 0);
      dir = (dir + Vector2.up) * (jumpforce + _jumpAcell);
      c_rigi2d.velocity = dir;
      if (_jumpTimer >= _jumpDuration || !_Inputs.GetButton("Jump"))
      {
        _wallJumping = false;
        StartCoroutine(GraceTime(_graceTime));
      }
    }
  }

  IEnumerator GraceTime(float time)
  {
    c_rigi2d.velocity = new Vector2(c_rigi2d.velocity.x, 0f);
    yield return new WaitForSeconds(time);
  }
  #endregion
  #region Wall Interactions
  private void WallSlide()
  {
    c_rigi2d.velocity = new Vector2(_velocity.x, _wallSlideVelocity * -1);
  }

  void WallJump(Vector2 dir)
  {
    StartCoroutine(DisableMove(0.2f));
    Jump(dir);
  }
  #endregion
  #region Dash Methods
  void Dash(Vector2 dir)
  {
    if (_faceRight)
      dir.x = 1;
    else
      dir.x = -1;

    StartCoroutine(ApplyDash(dir));

  }
  IEnumerator DisableMove(float time)
  {
    _canMove = false;
    yield return new WaitForSeconds(time);
    _canMove = true;
    yield break;
  }

  IEnumerator ApplyDash(Vector2 dir)
  {
    _canMove = false;
    c_rigi2d.gravityScale = 0f;
    _dashTimer += 1;
    if (_dashTimer < _dashFrezeTime)
    {
      c_rigi2d.velocity = Vector2.zero;
      yield return null;
    }
    else if (_dashTimer < (_dashDuration + _dashFrezeTime))
    {
      c_rigi2d.velocity += new Vector2(dir.normalized.x, 0) * _dashSpeed;
      yield return null;
    }
    else
    {
      _canMove = true;
      _dashing = false;
      yield break;
    }
    yield return null;
  }
  #endregion
  #region Physic Stuffs
  void CheckColission()
  {
    _onGround = Physics2D.BoxCast((Vector2)transform.position + _groundOffset, _groundSize, 0f, Vector2.down, _groundDistance, _groundLayer);
    _wallLeft = Physics2D.BoxCast((Vector2)transform.position + _wallLefOffset, _wallLeftSize, 0f, Vector2.left, _wallLeftDistance, _wallLayer);
    _wallRight = Physics2D.BoxCast((Vector2)transform.position + _wallRightOffset, _wallRightSize, 0f, Vector2.right, _wallRightDistance, _wallLayer);
    _cornerCorrection = Physics2D.Raycast(transform.position + _ccEdegeRayCast, Vector2.up, _ccLenghtRayCast, _ccLayers) &&
                        !Physics2D.Raycast(transform.position + _ccInerRayCast, Vector2.up, _ccLenghtRayCast, _ccLayers) ||
                        Physics2D.Raycast(transform.position - _ccEdegeRayCast, Vector2.up, _ccLenghtRayCast, _ccLayers) &&
                        !Physics2D.Raycast(transform.position - _ccInerRayCast, Vector2.up, _ccLenghtRayCast, _ccLayers);
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

    if (_onGround && !_dashing && !_jumping)
    {
      _dashCounter = 1f;
      _jumpCounter = _jumpTimes;
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
      transform.position = new Vector3(transform.position.x + _newPosition + 0.01f, transform.position.y, transform.position.z);
      c_rigi2d.velocity = new Vector2(c_rigi2d.velocity.x, velocityY);
      return;
    }

    _hit = Physics2D.Raycast(transform.position + _ccInerRayCast + Vector3.up * _ccLenghtRayCast, Vector3.right, _ccLenghtRayCast, _ccLayers);
    if (_hit.collider != null)
    {
      float _newPosition = Vector3.Distance(new Vector3(_hit.point.x, transform.position.y, 0f) + Vector3.up * _ccLenghtRayCast,
          transform.position + _ccEdegeRayCast + Vector3.up * _ccLenghtRayCast);
      c_sprite.enabled = false;
      transform.position = new Vector3(transform.position.x - _newPosition - 0.01f, transform.position.y, transform.position.z);
      c_sprite.enabled = true;
      c_rigi2d.velocity = new Vector2(c_rigi2d.velocity.x, velocityY);
    }
  }
  private void Flip()
  {
    Vector3 trans = transform.localScale;
    if (_faceRight)
    {
      transform.localScale = new Vector3(trans.x * -1, trans.y, trans.z);
      _faceRight = false;
    }
    else
    {
      transform.localScale = new Vector3(trans.x * -1, trans.y, trans.z);
      _faceRight = true;
    }
  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.red;
    Gizmos.DrawWireCube((Vector2)transform.position + _groundOffset, _groundSize);
    Gizmos.DrawWireCube((Vector2)transform.position + _wallLefOffset, _wallLeftSize);
    Gizmos.DrawWireCube((Vector2)transform.position + _wallRightOffset, _wallRightSize);

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
