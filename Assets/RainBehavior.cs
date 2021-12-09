using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RainBehavior : MonoBehaviour
{

  public float minActiveDistance;
  public int rainDamagePercent = 1;

  public UnityEvent OnRainCollideTorch;
  Transform player;
  TorchThrowBehavior torchThrowBehavior;

  ParticleSystem rainParticles;

  bool wasActive = false;

  private void Start()
  {
    player = GameObject.FindGameObjectWithTag("Player").transform;
    torchThrowBehavior = FindObjectOfType<TorchThrowBehavior>();
    rainParticles = GetComponent<ParticleSystem>();
  }

  private void Update()
  {
    bool isInRange = Vector3.Distance(transform.position, player.position) < minActiveDistance;
    if (isInRange && !wasActive)
    {
      rainParticles.Play();
      wasActive = true;
    }
    else if (!isInRange && wasActive)
    {
      rainParticles.Stop();
      wasActive = false;
    }
  }

  private void OnParticleCollision(GameObject other)
  {
    if (other.CompareTag("Player") && !torchThrowBehavior.HasThrow || other.CompareTag("Torch") && torchThrowBehavior.HasThrow)
    {
      Debug.Log("Torch Damage " + other.name);
      TorchBehavior.SetIntensity(-rainDamagePercent);
      OnRainCollideTorch?.Invoke();
    }
  }


  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.blue;
    Gizmos.DrawWireSphere(transform.position, minActiveDistance);
  }

}
