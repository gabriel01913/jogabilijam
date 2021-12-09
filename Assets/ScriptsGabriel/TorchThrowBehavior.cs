using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.Events;

public class TorchThrowBehavior : MonoBehaviour
{
  [Header("Variables")]
  [SerializeField] Transform torchPivot;
  [SerializeField] float throwSpeed = 30f;
  [SerializeField] LayerMask _layerMask;
  public GameObject _player;
  CelesteMovement _celeste;
  [SerializeField] Vector2 _aimDir;

  [Header("Events")]
  public UnityEvent OnThrow;
  public UnityEvent OnCallTorch;
  public UnityEvent OnTorchCollide;
  public UnityEvent OnGetTorch;

  [Header("Debug")]
  [SerializeField] bool _hasThrow;
  public bool HasThrow => _hasThrow;
  [SerializeField] bool _returning;
  Vector2 _mousePos;
  PlayerInput _playerInput;
  InputHandler _Inputs;
  Rigidbody2D c_rigi2d;
  Animator animator;
  bool isThrowing = false;
  DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> throwTween;
  DG.Tweening.Core.TweenerCore<Quaternion, Vector3, DG.Tweening.Plugins.Options.QuaternionOptions> rotateTween;
  // Start is called before the first frame update
  void Awake()
  {
    _celeste = _player.GetComponent<CelesteMovement>();
    c_rigi2d = GetComponent<Rigidbody2D>();
    _Inputs = _player.GetComponent<InputHandler>();
    _playerInput = _player.GetComponent<PlayerInput>();
    animator = GetComponent<Animator>();
  }
  // Update is called once per frame
  void Update()
  {
    UpdateAimDirection();
    UpdatePlayerGet();

    if (!_hasThrow)
    {
      transform.localPosition = torchPivot.localPosition;
    }

    UpdatePlayerInput();

  }

  void UpdatePlayerInput()
  {
    if (_Inputs.GetButtonDown("Torch") && !_hasThrow)
    {
      // on throw
      TorchThrow();

    }
    else if (_Inputs.GetButtonDown("Torch") && _hasThrow)
    {
      if (!_returning)
      {
        // on push back
        PullTorchBack();
      }
    }
  }

  void UpdateAimDirection()
  {
    if (_playerInput.currentControlScheme == "Keyboard")
    {
      _mousePos = _celeste._aimDirection;
      _aimDir = _mousePos - (Vector2)transform.position;
    }
    else
    {
      _aimDir = _celeste._aimDirection.normalized;
    }
  }

  void TorchThrow()
  {
    _hasThrow = true;
    transform.parent = null;
    animator.SetBool("show", true);
    isThrowing = true;
    OnThrow?.Invoke();

    Vector2 direction = _aimDir.normalized;

    if (direction == Vector2.zero)
    {
      if (_celeste._faceRight)
      {
        direction.x = 1f;
      }
      else
      {
        direction.x = -1f;
      }
    }

    Vector2 endPosition = GetDesiredEndPosition(torchPivot.position, direction);

    float distance = Vector2.Distance(torchPivot.position, endPosition);

    float duration = distance / throwSpeed;

    rotateTween = transform.DORotate(new Vector3(0, 0, 360), .3f, RotateMode.WorldAxisAdd).SetLoops(-1, LoopType.Restart);

    throwTween = transform.DOMove(endPosition, duration)
    .OnComplete(() =>
    {
      isThrowing = false;
      rotateTween?.Kill();
      OnTorchCollide?.Invoke();
    });

  }

  Vector2 GetDesiredEndPosition(Vector2 startPosition, Vector2 direction)
  {
    Vector2 point = Vector2.zero;
    RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, float.PositiveInfinity, _layerMask);
    if (hit.collider != null)
    {
      point = hit.point;
    }
    else
    {
      point = startPosition + direction * 10f;
    }

    return Vector2.Lerp(startPosition, point, 0.99f);
  }

  void PullTorchBack()
  {
    _returning = true;
    isThrowing = false;
    rotateTween?.Kill();
    throwTween?.Kill();

    OnCallTorch?.Invoke();

    Vector2 direction = torchPivot.position - transform.position;

    Vector2 endPosition = GetDesiredEndPosition(transform.position, direction);

    float distance = Vector2.Distance(torchPivot.position, transform.position);

    float duration = (distance / throwSpeed);

    rotateTween = transform.DORotate(new Vector3(0, 0, 360), .3f, RotateMode.WorldAxisAdd).SetLoops(-1, LoopType.Restart);
    throwTween = transform.DOMove(endPosition, duration)
    .OnComplete(() =>
    {
      _returning = false;
      rotateTween?.Kill();
    });

  }

  void UpdatePlayerGet()
  {
    if (!isThrowing && HasThrow)
    {
      if (Vector2.Distance(transform.position, torchPivot.position) < 2f)
      {
        GetTorch();
      }
    }
  }

  public void GetTorch()
  {
    _hasThrow = false;
    _returning = false;
    rotateTween?.Kill();
    throwTween?.Kill();
    transform.parent = _player.transform;
    transform.localPosition = Vector3.zero;
    transform.localRotation = Quaternion.identity;
    animator.SetBool("show", false);
    OnGetTorch?.Invoke();
    _player.transform.DOShakePosition(.2f, .1f, 20, 90, false, true);
  }


  #region QOL
  private void OnValidate()
  {
    if (torchPivot)
    {
      transform.localPosition = torchPivot.localPosition;
    }
  }

  #endregion
}
