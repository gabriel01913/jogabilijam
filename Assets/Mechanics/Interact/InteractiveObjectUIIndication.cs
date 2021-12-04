using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InteractiveObjectUIIndication : MonoBehaviour
{
  public Canvas canvas;
  public Image buttonImage;

  public Sprite buttonSprite;
  public Sprite buttonSpritePressed;

  public TMP_Text text;


  private void Start()
  {
    InvokeRepeating("SwapButtonSprite", 0f, 0.5f);
    Hide();
  }

  void SwapButtonSprite()
  {
    if (buttonImage.sprite == buttonSprite)
    {
      buttonImage.sprite = buttonSpritePressed;
    }
    else
    {
      buttonImage.sprite = buttonSprite;
    }
  }

  public void Show(string text)
  {
    this.text.SetText(text);
    canvas.enabled = true;
  }


  public void Hide()
  {
    canvas.enabled = false;
    text.SetText("");
  }



}
