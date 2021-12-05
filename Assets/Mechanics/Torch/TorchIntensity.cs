using UnityEngine;

public class TorchIntensity : MonoBehaviour
{
  public void SetIntensityByPercentage(int percentage)
  {
    TorchBehavior.SetIntensity(percentage);
  }
}
