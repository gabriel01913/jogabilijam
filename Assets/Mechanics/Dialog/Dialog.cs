using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : MonoBehaviour
{
  public string[] dialog;
  public bool isSystemMessage;

  public void Show()
  {
    var dialogController = FindObjectOfType<DialogController>();
    if (dialogController)
    {
      if (isSystemMessage)
        dialogController.ShowSystemMessage(dialog);
      else
        dialogController.ShowDialog(dialog);

    }
    else
    {
      Debug.LogError("No DialogController found");
    }
  }

}
