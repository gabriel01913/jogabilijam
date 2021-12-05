using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuoteController : MonoBehaviour
{
  [Header("Settings")]
  public TextAsset quotesCSV;

  [SerializeField] List<string> quoteList;
  public int totalOfQuotes => quoteObjects.Count;
  public int totalOfDiscoverdQuotes => quoteObjects.FindAll(q => q.wasDiscovered).Count;

  List<QuoteObject> quoteObjects;
  HashSet<int> usedQuotes;


  private void Awake()
  {
    CSVToQuoteList();
    int seed = PlayerPrefs.GetInt("seed", -1);
    if (seed == -1)
    {
      seed = Random.Range(0, 1000);
      PlayerPrefs.SetInt("seed", seed);
    }

    Random.InitState(seed);
  }

  void Start()
  {
    quoteObjects = new List<QuoteObject>(FindObjectsOfType<QuoteObject>());
    usedQuotes = new HashSet<int>();

    foreach (var quoteObject in quoteObjects)
    {
      int quoteNumber = GetAvaliableQuoteFromList();
      string quote = quoteList[quoteNumber];

      string objectId = quoteObject.Setup(this, quoteNumber, quote);
    }

    Debug.Log("Total of quotes: " + totalOfQuotes);
    Debug.Log("Total of discovered quotes: " + totalOfDiscoverdQuotes);
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
