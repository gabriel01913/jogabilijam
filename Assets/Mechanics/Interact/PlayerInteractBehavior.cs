using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInteractBehavior : MonoBehaviour
{

  [Header("Settings")]
  public Transform interactionPivot;
  public float interactionRadius = 1f;
  public LayerMask interactLayer;


  public static bool isMechanicEnabled = true;

  public void TryInteract(InputAction.CallbackContext context)
  {
    if (!context.performed)
      return;

    if (!isMechanicEnabled)
      return;

    Collider2D[] colliders = Physics2D.OverlapCircleAll(interactionPivot.position, interactionRadius, interactLayer);
    if (colliders.Length > 0)
    {
      InteractiveObject interactiveObject = colliders[0].gameObject.GetComponent<InteractiveObject>();
      interactiveObject.Interact();
    }
  }


  private void OnDrawGizmosSelected()
  {
    if (interactionPivot)
    {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireSphere(interactionPivot.position, interactionRadius);
    }
  }
}
