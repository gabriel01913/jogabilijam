using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class torchTests : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] Vector3 _torchIniPosition;
    [SerializeField] Vector3 _boxSize;
    [SerializeField] Vector3 _boxOffset;
    [SerializeField] float _rotateSpeed = 10f;
    [SerializeField] float _force = 30f;
    [SerializeField] LayerMask _layerMask;
    [SerializeField] LayerMask _playerMask;
    public GameObject _player;
    CelesteMovement _celeste;
    [SerializeField] Vector2 _aimDir;
    [SerializeField] Vector2 _moveDir;
    Vector3 _torchPos;
    Vector3 _playerPos;
    [Header("Debug")]
    [SerializeField] bool _canThrow;
    [SerializeField] bool _hasThrow;
    [SerializeField] bool _moving;
    [SerializeField] bool _stoped;
    [SerializeField] bool _returning;
    Coroutine _coroutine;
    Vector2 _point;
    Vector2 _mousePos;
    PlayerInput _playerInput;
    InputHandler _Inputs;
    Rigidbody2D c_rigi2d;
    // Start is called before the first frame update
    void Awake()
    {
        _celeste = _player.GetComponent<CelesteMovement>();
        c_rigi2d = GetComponent<Rigidbody2D>();
        _Inputs = _player.GetComponent<InputHandler>();
        _playerInput = _player.GetComponent<PlayerInput>();
    }
    // Update is called once per frame
    void Update()
    {
        CheckCollision();
        CheckPoint();        
        _playerPos = _player.transform.position;        
        _torchPos = transform.position;
        if(_playerInput.currentControlScheme == "Keyboard")
        {
            _mousePos = _celeste._aimDirection;
            _aimDir = _mousePos - (Vector2)transform.position;
        }
        else
        {
            _aimDir = _celeste._aimDirection.normalized;
        }
        if (_Inputs.GetButtonDown("Torch") && !_hasThrow)
        {
            _hasThrow = true;            
        }

        if (_Inputs.GetButtonDown("Torch") && _hasThrow && _stoped)
        {
            _returning = true;
        }
    }
    private void FixedUpdate()
    {
        if (_hasThrow && !_stoped) TorchThrow(_aimDir);
        if (_returning) ReturningThrow();
    }
    void TorchThrow(Vector3 dir)
    {
        if (!_moving)
        {
            _moveDir = dir.normalized;
        }
        if(_moveDir == Vector2.zero)
        {
            if (_celeste._faceRight)
            {
                _moveDir.x = 1f;
            }
            else
            {
                _moveDir.x = -1f;
            }
        }
        transform.parent = null;
        _coroutine = StartCoroutine(Moving(_moveDir));
    }
    void ReturningThrow()
    {
        if (!_moving)
        {
            _moveDir = new Vector2(_playerPos.x - _torchPos.x, _playerPos.y - _torchPos.y).normalized;
        }
        if(transform.parent != null)
        {
            transform.parent = null;
        }
        _coroutine = StartCoroutine(Moving(_moveDir));
    }
    void GetThrow()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        c_rigi2d.velocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        _hasThrow = false;
        _returning = false;
        _moving = false;
        _stoped = false;
        transform.parent = _player.transform;
        transform.localPosition = _torchIniPosition;
        if (!_celeste._onGround)
        {
            _celeste._dashCounter = 1f;
        }
    }
    IEnumerator Moving(Vector2 dir)
    {
        _stoped = false;
        _moving = true;
        c_rigi2d.velocity = dir * _force;
        transform.Rotate(0,0,_rotateSpeed);
        yield return null;
    }
    private void Stop()
    {
        if(_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }        
        _moving = false;
        _returning = false;
        _stoped = true;
        c_rigi2d.velocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    void CheckCollision()
    {
        RaycastHit2D _hit = Physics2D.BoxCast(new Vector3(transform.position.x + _boxOffset.x, transform.position.y + _boxOffset.y, 0f),
                            _boxSize, 0f, Vector2.left, 0f, _playerMask);
        if(_hit.collider != null && (_stoped || _returning))
        {
            GetThrow();
            return;
        }
        _hit = Physics2D.BoxCast(new Vector3(transform.position.x + _boxOffset.x, transform.position.y + _boxOffset.y, 0f),
                            _boxSize, 0f, Vector2.left, 0f, _layerMask);
        if (_hit.collider != null && !_stoped && _hasThrow)
        {
            Stop();
            Vector3 newpos = new Vector3(transform.position.x - _hit.point.x, transform.position.y - _hit.point.y, 0f);
            Debug.Log(newpos);
            transform.position = new Vector3(transform.position.x + newpos.x, transform.position.y + newpos.y, 0f);
        }
    }
    #region QOL
    Vector3 CheckPoint()
    {
        Vector3 _newPosition = Vector3.zero;
        RaycastHit2D _hit = Physics2D.Raycast(transform.position, _aimDir, Mathf.Infinity, _layerMask);
        _point = _hit.point;
        Debug.DrawRay(transform.position, _aimDir, Color.red);
        return _newPosition;
    }
    private void OnDrawGizmos()
    {
        Vector3 pos = new Vector3(_aimDir.x * 10 + transform.position.x, _aimDir.y * 10 + transform.position.y, 0f);
        Gizmos.DrawLine(transform.position, pos);
        Gizmos.DrawWireSphere(_point, 1);
        Gizmos.DrawWireCube(new Vector3(transform.position.x + _boxOffset.x, transform.position.y + _boxOffset.y, 0f), _boxSize);
    }
    #endregion
}
