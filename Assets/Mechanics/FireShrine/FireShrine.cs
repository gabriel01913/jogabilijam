using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class FireShrine : MonoBehaviourID
{
  [Header("Settings")]
  public string shrineName;

  [Header("References")]
  public Animator fireAnimator;
  public AudioSource fireAudioSource;

  [Header("Events")]
  public UnityEvent onFireStart;
  public UnityEvent onSaveGame;

  public bool isLit = false;

  private void Awake()
  {
    isLit = PlayerPrefs.GetInt($"FS_{ID}", 0) == 1;
    if (isLit)
    {
      fireAnimator.SetTrigger("Lit");
      fireAudioSource.Play();
    }
  }

  public void LitAndSaveGame()
  {
    SaveGame();

    if (!isLit)
    {
      isLit = true;
      fireAnimator.SetTrigger("Lit");
      fireAudioSource.Play();
      onFireStart?.Invoke();
      PlayerPrefs.SetInt($"FS_{ID}", 1);

      return;
    }

    onSaveGame?.Invoke();

  }

  public void SaveGame()
  {
    PlayerPrefs.SetString("LastFireShrine", ID);
  }



  protected override void OnValidate()
  {
    base.OnValidate();
    this.name = shrineName;
  }

}
