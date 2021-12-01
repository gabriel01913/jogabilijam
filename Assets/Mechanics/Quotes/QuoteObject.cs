using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuoteObject : MonoBehaviour
{

  [SerializeField]
  private string _id = Guid.NewGuid().ToString();

  public string ID { get => _id; }


  [SerializeField]
  private int index;

  [SerializeField]
  private string text;

  private QuoteController quoteController;

  private void Awake()
  {
    index = 0;
    text = "";
  }

  public string Setup(QuoteController quoteController, int index, string text)
  {
    this.index = index;
    this.text = text;
    this.quoteController = quoteController;

    return _id;
  }


  public void ShowQuote()
  {
    Debug.Log(FormatedText());
  }

  string FormatedText()
  {
    return text.Replace("\"\"", "\n");
  }
}
