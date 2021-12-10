using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineDoor : MonoBehaviour
{
  public int numberOfFireShrineNeeded = 3;
  public bool isOpen = false;
  public GameObject door;
  public List<FireShrine> fireShrines;
  public string text;
  public Transform destination;
  public Cinemachine.CinemachineConfiner confiner;

  public PolygonCollider2D camConfinerCollider;

  bool wasActivated = false;
  // Start is called before the first frame update
  void Start()
  {
    fireShrines = new List<FireShrine>(FindObjectsOfType<FireShrine>());
    InvokeRepeating("CheckIfCanOpen", 0, 1);
  }

  void CheckIfCanOpen()
  {
    if (fireShrines.FindAll((FireShrine shrine) =>
    {
      return shrine.isLit;
    }).Count >= numberOfFireShrineNeeded)
    {
      if (!isOpen)
      {
        isOpen = true;
        door.SetActive(false);
      }
    }
  }


  public void Interact()
  {
    if (wasActivated)
    {
      return;
    }
    if (isOpen)
    {
      wasActivated = true;
      FindObjectOfType<ScreenTransition>().FadeOutCall(() =>
      {
        FindObjectOfType<CelesteMovement>().transform.position = destination.position;
        confiner.m_BoundingShape2D = camConfinerCollider;
        FindObjectOfType<ScreenTransition>().FadeInCall(() =>
        {
          wasActivated = false;
        });
      });
    }
    else
    {
      var dialogController = FindObjectOfType<DialogController>();
      if (dialogController)
      {
        dialogController.ShowSystemMessage(text);
      }
    }
  }
}