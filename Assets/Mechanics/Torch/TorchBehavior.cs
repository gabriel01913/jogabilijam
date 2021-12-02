using UnityEngine;
using UnityEngine.Events;

public class TorchBehavior : MonoBehaviour
{


  [SerializeField] float maxDuration = 5f;
  [SerializeField] float timeLeft = 5f;

  public float TimeLeft { get { return timeLeft; } }

  public UnityEvent OnExtinguish;

  bool isExtinguished = false;
  void Update()
  {
    timeLeft -= Time.deltaTime;
    if (!isExtinguished && timeLeft <= 0)
    {
      isExtinguished = true;
      OnExtinguish?.Invoke();
    }
  }

  public void Refill()
  {
    timeLeft = maxDuration;
    isExtinguished = false;
  }

}
