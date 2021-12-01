using UnityEngine;
using UnityEngine.Events;

public class InteractiveObject : MonoBehaviour
{
  [SerializeField] UnityEvent OnInteract;

  public void Interact()
  {
    OnInteract?.Invoke();
  }
}
