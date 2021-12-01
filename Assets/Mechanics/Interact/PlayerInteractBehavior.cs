using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractBehavior : MonoBehaviour
{

  [Header("Settings")]
  public Transform interactionPivot;
  public float interactionRadius = 1f;

  public LayerMask interactLayer;

  public string button = "";

  public static bool isMechanicEnabled = true;

  // Update is called once per frame
  void Update()
  {
    if (isMechanicEnabled)
    {
      if (Input.GetButtonDown(button))
      {
        TryInteract();
      }
    }
  }

  void TryInteract()
  {
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
