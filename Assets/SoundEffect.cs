using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffect : MonoBehaviour
{
  AudioSource audioSource;

  [Header("Settings")]
  public AudioClip[] clips;

  [Header("Pitch")]

  public bool ramdomPitch = true;
  public float minPitch = 0.9f;
  public float maxPitch = 1.1f;


  void Start()
  {
    audioSource = GetComponent<AudioSource>();
  }


  public void PlayRandomClip()
  {
    if (clips.Length == 0)
      return;
    audioSource.pitch = ramdomPitch ? Random.Range(minPitch, maxPitch) : 1;
    audioSource.PlayOneShot(clips[Random.Range(0, clips.Length - 1)]);
  }

}
