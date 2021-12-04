using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
  [SerializeField] GameEvent gameEvent;
  [SerializeField] UnityEvent unityEvent;

  void Awake() => gameEvent.Register(this);

  void OnDestroy() => gameEvent.Register(this);

  public void OnEventRaised() => unityEvent?.Invoke();
}
