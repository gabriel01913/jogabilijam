using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuoteObject : MonoBehaviourID
{

  [SerializeField]
  private int index;

  [SerializeField]
  private string text;

  private QuoteController quoteController;


  public bool wasDiscovered = false;

  private void Awake()
  {
    index = 0;
    text = "";

    wasDiscovered = PlayerPrefs.GetInt($"QO_{ID}_wasDiscovered", 0) == 1;
  }

  public string Setup(QuoteController quoteController, int index, string text)
  {
    this.index = index;
    this.text = text;
    this.quoteController = quoteController;

    return ID;
  }


  public void ShowQuote()
  {
    var dialogController = FindObjectOfType<DialogController>();
    if (dialogController)
    {
      dialogController.ShowDialog(FormatedText());
      if (!wasDiscovered)
      {
        wasDiscovered = true;
        PlayerPrefs.SetInt($"QO_{ID}_wasDiscovered", 1);
      }
    }
    else
    {
      Debug.LogError("No DialogController found");
    }
  }

  string FormatedText()
  {
    return text.Replace("\"\"", "\n");
  }
}
