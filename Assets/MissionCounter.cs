using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionCounter : MonoBehaviour
{
  public TMP_Text fireShrineCounter;
  public TMP_Text wisdomCounter;

  QuoteController quoteController;
  List<FireShrine> shrineList;

  public int totalOfShrinesLit => shrineList.FindAll(x => x.isLit).Count;


  void Start()
  {
    shrineList = new List<FireShrine>(FindObjectsOfType<FireShrine>());
    fireShrineCounter.SetText($"{totalOfShrinesLit}/{shrineList.Count}");

    quoteController = FindObjectOfType<QuoteController>();
    wisdomCounter.SetText($"{quoteController.totalOfDiscoverdQuotes}/{quoteController.totalOfQuotes}");
  }

  // Update is called once per frame
  void Update()
  {
    wisdomCounter.SetText($"{quoteController.totalOfDiscoverdQuotes}/{quoteController.totalOfQuotes}");
    fireShrineCounter.SetText($"{totalOfShrinesLit}/{shrineList.Count}");
  }
}
