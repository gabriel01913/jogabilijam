using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour
{

  public UnityEvent[] animationEvents;
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void PlayAnimationEvent(int eventNumber)
  {
    animationEvents[eventNumber]?.Invoke();
  }

}
