using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;
public class ScreenTransition : MonoBehaviour
{

  [Header("Settings")]
  public float duration = 1f;
  public Image image;

  [Header("Events")]
  public UnityEvent onFadeInEnd;
  public UnityEvent onFadeOutEnd;

  [ContextMenu("Fade In")]
  public void FadeIn()
  {
    image.enabled = true;
    SetAlpha(1);

    image.DOFade(0, duration).SetEase(Ease.InOutQuad).OnComplete(() =>
    {
      image.enabled = false;
      onFadeInEnd?.Invoke();
    });
  }


  [ContextMenu("Fade Out")]
  public void FadeOut()
  {
    image.enabled = true;
    SetAlpha(0);
    image.DOFade(1, duration).SetEase(Ease.InOutQuad).OnComplete(() =>
    {
      // image.enabled = false;
      onFadeOutEnd?.Invoke();
    });
  }

  public void FadeInCall(Action onComplete)
  {
    image.enabled = true;
    SetAlpha(1);

    image.DOFade(0, duration).SetEase(Ease.InOutQuad).OnComplete(() =>
    {
      image.enabled = false;
      onComplete?.Invoke();
    });
  }

  public void FadeOutCall(Action onComplete)
  {
    image.enabled = true;
    SetAlpha(0);
    image.DOFade(1, duration).SetEase(Ease.InOutQuad).OnComplete(() =>
    {
      // image.enabled = false;
      onComplete?.Invoke();
    });
  }

  void SetAlpha(float alpha)
  {
    image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
  }

}
