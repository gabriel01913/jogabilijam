using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public GameObject _camera;
    Camera _mainCamera;
    PlayerInput _inputs;
    InputAction jump;
    InputAction dash;
    InputAction torch;
    bool _JumpDown;
    bool _JumpHeld;
    bool _JumpUp;
    bool _DashDown;
    bool _DashHeld;
    bool _DashUp;
    bool _TorchDown;
    bool _TorchHeld;
    bool _TorchUp;
    public float _horizontal;
    public float _vertical;
    Vector2 _mouseWolrd;
    public float _aimHorizontal;
    public float _aimVertical;
    private void Awake()
    {
        _inputs = GetComponent<PlayerInput>();
        _mainCamera = _camera.GetComponent<Camera>();
    }
    // Start is called before the first frame update
    void Start()
    {
        jump = _inputs.actions["Jump"];
        dash = _inputs.actions["Dash"];
        torch = _inputs.actions["Torch"];
    }
    private void Update()
    {
        var mousePosition = _inputs.actions["aim"].ReadValue<Vector2>();
        _mouseWolrd = _mainCamera.ScreenToWorldPoint(mousePosition);
        _aimHorizontal = _mouseWolrd.x;
        _aimVertical = _mouseWolrd.y;
    }
    #region Gambiara
    public void ButtonsStart()
    {
        _JumpDown = jump.triggered;
        _DashDown = dash.triggered;
        _TorchDown = torch.triggered;

    }
    public void ButtonsCleanUp()
    {
        _JumpUp = false;
        _DashUp = false;
        _TorchUp = false;
    }
    #endregion
    #region InputsCallbacks
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _JumpHeld = true;
            _JumpUp = false;
        }
        if (context.canceled)
        {
            _JumpHeld = false;
            _JumpUp = true;
        }
    }
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _DashHeld = true;
            _DashUp = false;
        }
        if (context.canceled)
        {
            _DashHeld = false;
            _DashUp = true;
        }
    }
    public void Torch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _TorchHeld = true;
            _TorchUp = false;
        }
        if (context.canceled)
        {
            _TorchHeld = false;
            _TorchUp = true;
        }
    }
    public void Movement(InputAction.CallbackContext context)
    {
        _horizontal = context.ReadValue<Vector2>().x;
        _vertical = context.ReadValue<Vector2>().y;
    }
    #endregion
    #region Acess Methods
    public bool GetButton(string action)
    {
        bool held = false;
        if (action == "Jump") held = _JumpHeld;
        if (action == "Dash") held = _DashHeld;
        if (action == "Torch") held = _TorchHeld;
        return held;
    }
    public bool GetButtonDown(string action)
    {
        bool held = true;
        if (action == "Jump") held = _JumpDown;
        if (action == "Dash") held = _DashDown;
        if (action == "Torch") held = _TorchDown;
        return held;
    }
    public bool GetButtonUp(string action)
    {
        bool held = true;
        if (action == "Jump") held = _JumpUp;
        if (action == "Dash") held = _DashUp;
        if (action == "Torch") held = _TorchUp;
        return held;
    }
    public float GetAxisRaw(string axis)
    {
        float axisF = 0f;
        if(axis == "Horizontal")
        {
            axisF = _horizontal;
        }
        if (axis == "Vertical")
        {
            axisF = _vertical;
        }
        if (axis == "AimHorizontal")
        {
            axisF = _aimHorizontal;
        }
        if (axis == "AimVertical")
        {
            axisF = _aimVertical;
        }
        return axisF;
    }
    #endregion
    #region Dont Delete
    private void OnEnable()
    {
        _inputs.enabled = true;
    }
    private void OnDisable()
    {
        _inputs.enabled = false;
    }
    #endregion
}
