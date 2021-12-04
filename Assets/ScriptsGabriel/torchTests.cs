using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class torchTests : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] Vector3 _torchIniPosition;
    [SerializeField] float _rotateSpeed = 10f;
    [SerializeField] float _force = 30f;
    [SerializeField] float _freezeReturning = 0.5f;
    public GameObject _player;
    CelesteMovement _celeste;
    Vector3 _playerDir;
    Vector3 _torchPos;
    Vector3 _playerPos;
    public LayerMask _layerMask;
    [Header("Debug")]
    [SerializeField] bool _onMove;
    [SerializeField] bool _moving;
    [SerializeField] bool _stoped;
    [SerializeField] bool _returning;
    float _returningTime;
   

    Vector2 _point;

    InputHandler _Inputs;
    Rigidbody2D c_rigi2d;
    // Start is called before the first frame update
    void Awake()
    {
        _celeste = _player.GetComponent<CelesteMovement>();
        c_rigi2d = GetComponent<Rigidbody2D>();
        _Inputs = _player.GetComponent<InputHandler>();
    }
    // Update is called once per frame
    void Update()
    {        
        _playerPos = _player.transform.position;
        _playerDir = _celeste._aimDirection;
        _torchPos = transform.position;        

        CheckPoint();
        if (_Inputs.GetButtonDown("Torch") && !_onMove)
        {
            TorchThrow(_playerDir);
        }        
    }

    private void FixedUpdate()
    {
        if (_stoped) StartCoroutine(Returning());
        if(_moving || _returning)
        {
            
        }
    }

    #region TorchMovement
    void TorchThrow(Vector2 dir)
    {
        _onMove = true;
        _moving = true;
        if (dir == Vector2.zero)
        {
            if (_celeste._faceRight)
            {
                dir = new Vector2(1f, 0f);
            }
            else
            {
                dir = new Vector2(-1f, 0f);
            }
        }
        c_rigi2d.isKinematic = false;
        c_rigi2d.gravityScale = 0;
        c_rigi2d.drag = 0;
        transform.parent = null;
        if (_moving)
        {
            c_rigi2d.AddForce(dir * _force, ForceMode2D.Impulse);
            transform.Rotate(0, 0, _rotateSpeed);
        }
    }

    IEnumerator Returning()
    {
        yield return new WaitForSeconds(_freezeReturning);
        transform.position = Vector3.Lerp(transform.position, _playerPos, _returningTime += Time.fixedDeltaTime);
        if (!_returning)
        {
            yield break;
        }
    }
    #endregion
    #region CheckCollision
    Vector3 CheckPoint()
    {
        Vector3 _newPosition = Vector3.zero;
        RaycastHit2D _hit = Physics2D.Raycast(transform.position, _playerDir, Mathf.Infinity, _layerMask);
        _point = _hit.point;
        Debug.DrawRay(transform.position, _playerDir, Color.red);
        return _newPosition;

    }
    #endregion
    #region TriggerCollision
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Saiu PLayer");
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _stoped = false;
            _returning = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StopAllCoroutines();
        if (collision.tag == "Player" && _returning)
        {
            _returning = false;
            _moving = false;
            _stoped = false;
            c_rigi2d.isKinematic = true;
            transform.parent = _player.transform;
            _onMove = false;

        }

        if (_moving && collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
            _moving = false;
            c_rigi2d.velocity = Vector2.zero;
            _stoped = true;
            _returningTime = 0f;
        }
        if(collision.tag == "Player" && !_onMove)
        {
            StopAllCoroutines();
            transform.rotation = Quaternion.Euler(Vector3.zero);
            transform.localPosition = _torchIniPosition;
        }        
    }
    #endregion
    #region QOL
    private void OnDrawGizmos()
    {
        Vector3 pos = _playerDir * 10 + transform.position;
        Gizmos.DrawLine(transform.position, pos);
        Gizmos.DrawWireSphere(_point, 1);
    }
    #endregion
}
