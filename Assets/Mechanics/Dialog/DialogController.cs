using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class DialogController : MonoBehaviour
{
  [Header("References")]
  public Canvas dialogCanvas;
  public TMPro.TMP_Text dialogText;
  public Image textBox;

  [Header("Settings")]
  [SerializeField] [Range(0, 2)] float openDialogAnimationDuration = 0.5f;
  [SerializeField] [Range(0, 2)] float closeDialogAnimationDuration = 0.5f;

  [Header("Events")]
  public UnityEvent onDialogOpen;
  public UnityEvent onDialogClose;


  float textBoxHeight;

  [SerializeField] List<string> dialogLines = new List<string>();
  int currentLine = 0;
  private void Awake()
  {
    textBoxHeight = textBox.rectTransform.rect.height;
    textBox.rectTransform.sizeDelta = new Vector2(textBox.rectTransform.rect.width, 0);
    dialogCanvas.enabled = false;
  }

  private void Start()
  {
    // ShowDialog("Hello World!", "This is a test dialog.");
  }


  void ShowText(string text, TMPro.TextAlignmentOptions alignment = TMPro.TextAlignmentOptions.Left)
  {
    dialogText.SetText("");
    dialogText.alignment = alignment;

    dialogCanvas.enabled = true;

    textBox.rectTransform.DOSizeDelta(
      new Vector2(textBox.rectTransform.rect.width, textBoxHeight),
    openDialogAnimationDuration)
    .OnComplete(() =>
    {
      dialogText.SetText(text);
    });
  }

  public void ShowDialog(params string[] textLines)
  {
    onDialogOpen?.Invoke();
    dialogLines.Clear();
    dialogLines.AddRange(textLines);
    currentLine = 0;
    ShowText(dialogLines[currentLine]);
  }

  public void ShowSystemMessage(params string[] textLines)
  {
    onDialogOpen?.Invoke();
    dialogLines.Clear();
    dialogLines.AddRange(textLines);
    currentLine = 0;
    ShowText(dialogLines[currentLine], TMPro.TextAlignmentOptions.Center);
  }

  public void NextDialog(InputAction.CallbackContext context)
  {
    if (!context.performed)
      return;

    if (!dialogCanvas.enabled) return;

    textBox.rectTransform.sizeDelta = new Vector2(textBox.rectTransform.rect.width, textBoxHeight);
    if (currentLine < dialogLines.Count - 1)
    {
      currentLine++;
      dialogText.SetText(dialogLines[currentLine]);
    }
    else
    {
      HideDialog();
    }
  }

  public void HideDialog()
  {
    dialogText.SetText("");
    textBox.rectTransform.DOSizeDelta(
      new Vector2(textBox.rectTransform.rect.width, 0),
    closeDialogAnimationDuration)
    .OnComplete(() =>
    {
      dialogCanvas.enabled = false;
      onDialogClose?.Invoke();
    });
  }


}
