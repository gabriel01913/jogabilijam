using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GlobalLightBehavior : MonoBehaviour
{

  public Light2D globalLight;

  [SerializeField] float startValue = 1f;
  [SerializeField] float endValue = .5f;
  [SerializeField] float transitionDuration = 1f;
  float timeElapsed = 0f;

  bool timeElapsedReachedEndValue = false;

  void Start()
  {
    timeElapsedReachedEndValue = PlayerPrefs.GetInt("timeElapsedReachedEndValue", 0) == 1;
  }

  private void Update()
  {
    if (timeElapsedReachedEndValue)
    {
      return;
    }


    if (timeElapsed < transitionDuration)
    {
      timeElapsed += Time.deltaTime;
      float t = timeElapsed / transitionDuration;
      globalLight.intensity = Mathf.Lerp(startValue, endValue, t);
    }
    else
    {
      timeElapsedReachedEndValue = true;
      PlayerPrefs.SetInt("timeElapsedReachedEndValue", 0);
    }
  }
}
