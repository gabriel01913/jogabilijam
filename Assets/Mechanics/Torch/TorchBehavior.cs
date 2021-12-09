using UnityEngine;
using UnityEngine.Events;

public class TorchBehavior : MonoBehaviour
{


  [Header("Settings")]

  [SerializeField] float maxDuration = 5f;
  [SerializeField] float timeLeft = 5f;

  public float TimeLeft => timeLeft;
  public float Percentage => (timeLeft / maxDuration) * 100;
  [Space]
  [Header("Events")]
  public UnityEvent OnExtinguish;
  public UnityEvent OnRefill;
  public UnityEvent OnDamage;

  bool isExtinguished = false;
  void Update()
  {
    timeLeft -= Time.deltaTime;
    timeLeft = Mathf.Clamp(timeLeft, 0, maxDuration);
    if (!isExtinguished && timeLeft <= 0)
    {
      isExtinguished = true;
      OnExtinguish?.Invoke();
    }
  }

  public static void SetIntensity(int amountInPercentage)
  {
    TorchBehavior torch = FindObjectOfType<TorchBehavior>();
    if (torch == null)
    {
      Debug.LogError("TorchBehavior.Refill: TorchBehavior not found");
      return;
    }
    torch.timeLeft = Mathf.Clamp(torch.timeLeft + torch.maxDuration * (amountInPercentage / 100f), 0, torch.maxDuration);
  }

  private void OnValidate()
  {
    this.timeLeft = maxDuration;
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    Debug.Log("OnTriggerEnter2D");
  }

}
