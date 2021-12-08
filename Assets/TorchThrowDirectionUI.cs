using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class TorchThrowDirectionUI : MonoBehaviour
{


  public PlayerInput playerInput;
  public Camera mainCamera;
  public Transform arrow;
  public Transform pivot;
  public Transform arrowDir;

  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    arrow.position = pivot.position;
  }


  public void HandleInput(InputAction.CallbackContext context)
  {

    Vector2 input = context.ReadValue<Vector2>();

    if (input.magnitude < 0.1f)
    {
      arrow.gameObject.SetActive(false);
      return;
    }

    arrow.gameObject.SetActive(true);

    string currentControlScheme = playerInput.currentControlScheme;
    if (currentControlScheme == "Keyboard")
    {
      RotateAimMouse(input);
    }
    else
    {
      RotateAimControll(input);
    }

  }

  public void RotateAimControll(Vector2 input)
  {
    float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
    arrow.rotation = Quaternion.Euler(0, 0, angle);
  }
  public void RotateAimMouse(Vector2 input)
  {
    Vector3 mousePosition = mainCamera.ScreenToWorldPoint(input);
    Vector2 dir1 = arrowDir.position - arrow.transform.position;
    Vector2 dir2 = mousePosition - arrow.transform.position;
    float angle = Vector3.SignedAngle(dir1, dir2, new Vector3(0, 0, 1));
    arrow.transform.Rotate(new Vector3(0, 0, angle), Space.Self);
  }
}
