using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class PlayerInteractBehavior : MonoBehaviour
{

  [Header("Settings")]
  public Transform interactionPivot;
  public float interactionRadius = 1f;
  public LayerMask interactLayer;

  public bool isMechanicEnabled = true;

  [Header("Events")]
  public UnityEvent<string> onNewInteractiveObjectInRange;
  public UnityEvent onInteractiveObjectLeftRange;

  [SerializeField] InteractiveObject currentInteractiveObject;



  private void Update()
  {
    if (isMechanicEnabled)
    {
      checkCanInteract();
    }
  }

  void checkCanInteract()
  {
    Collider2D[] colliders = Physics2D.OverlapCircleAll(interactionPivot.position, interactionRadius, interactLayer);

    if (colliders.Length > 0 && colliders[0].TryGetComponent(out InteractiveObject interactiveObject))
    {
      if (currentInteractiveObject != interactiveObject)
      {
        currentInteractiveObject = interactiveObject;
        onNewInteractiveObjectInRange?.Invoke(currentInteractiveObject.name);
      }
    }
    else
    {
      if (currentInteractiveObject != null)
      {
        onInteractiveObjectLeftRange?.Invoke();
      }
      currentInteractiveObject = null;
    }

  }

  public void TryInteract(InputAction.CallbackContext context)
  {
    if (!context.performed)
      return;

    if (!isMechanicEnabled)
      return;

    currentInteractiveObject?.Interact();
  }


  private void OnDrawGizmosSelected()
  {
    if (interactionPivot)
    {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireSphere(interactionPivot.position, interactionRadius);
    }
  }

  public void SetIsMechanicEnabled(bool isEnabled)
  {
    isMechanicEnabled = isEnabled;
  }
}
