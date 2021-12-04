using UnityEngine;
using UnityEngine.Events;

public class TorchBehavior : MonoBehaviour
{


  [SerializeField] float maxDuration = 5f;
  [SerializeField] float timeLeft = 5f;

  public float TimeLeft => timeLeft;

  public UnityEvent OnExtinguish;
  public UnityEvent OnRefill;
  public UnityEvent OnDamage;

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

  public void Refill(int amountInPercentage)
  {
    timeLeft = maxDuration * (amountInPercentage / 100f);
  }

  public void Decrease(int amountInPercentage)
  {
    timeLeft -= maxDuration * (amountInPercentage / 100f);
  }

}
