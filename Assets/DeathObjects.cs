using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeathObjects : MonoBehaviour
{

  public UnityEvent OnCollidePlayer;
  public float timeToRestart = 1f;
  public UnityEvent OnRestart;

  private void OnCollisionEnter2D(Collision2D other)
  {
    if (other.gameObject.tag == "Player")
    {
      OnCollidePlayer?.Invoke();
      StartCoroutine(Restart());
    }
  }

  IEnumerator Restart()
  {
    yield return new WaitForSeconds(timeToRestart);
    OnRestart?.Invoke();
  }
}
