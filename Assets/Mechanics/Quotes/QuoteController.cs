using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuoteController : MonoBehaviour
{
  [Header("Settings")]
  public TextAsset quotesCSV;

  [SerializeField] List<string> quoteList;

  List<QuoteObject> quoteObjects;
  HashSet<int> usedQuotes;

  // Dictionary<string, int> quoteObjectToQuote;  mais para o sistema de save no futuro

  private void Awake()
  {
    CSVToQuoteList();
  }

  void Start()
  {
    quoteObjects = new List<QuoteObject>(FindObjectsOfType<QuoteObject>());
    usedQuotes = new HashSet<int>();

    foreach (var quoteObject in quoteObjects)
    {
      int quoteNumber = GetAvaliableQuoteFromList();
      string quote = quoteList[quoteNumber];

      quoteObject.Setup(this, quoteNumber, quote);
    }
  }

  int GetAvaliableQuoteFromList()
  {
    int number = Random.Range(0, quoteList.Count);

    while (usedQuotes.Contains(number))
    {
      number = Random.Range(0, quoteList.Count);
    }

    usedQuotes.Add(number);

    return number;
  }

  void CSVToQuoteList()
  {
    string text = quotesCSV.text;
    quoteList = new List<string>(text.Split('\n'));
  }

}
