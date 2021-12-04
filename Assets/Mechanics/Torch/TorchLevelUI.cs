using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TorchLevelUI : MonoBehaviour
{
  public TMP_Text percentageText;
  public Image fireImage;
  TorchBehavior torch;

  private void Start()
  {
    torch = FindObjectOfType<TorchBehavior>();
    if (torch == null)
    {
      Debug.LogError("TorchLevelUI: No TorchBehavior found");
    }
  }
  void Update()
  {
    UpdatePercentage();
    UpdateFireOpacity();
  }

  public void UpdatePercentage()
  {
    percentageText.text = torch.Percentage.ToString("0.0") + "%";
  }

  public void UpdateFireOpacity()
  {
    fireImage.color = new Color(1, 1, 1, torch.Percentage / 100f);
  }
}
