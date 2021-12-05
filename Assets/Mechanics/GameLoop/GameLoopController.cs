using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoopController : MonoBehaviour
{

  [Header("References")]
  public Transform player;

  public Transform gameStartPoint;

  private void Start()
  {
    if (player == null)
    {
      Debug.LogError("Game Loop Controller: Player is null");
    }

    GameStart();
  }

  public void GameStart()
  {
    PlacePlayerAtCorrectPosition();
  }

  public void PlacePlayerAtCorrectPosition()
  {
    FireShrine lastFireShrine = GetLastShrine();
    if (lastFireShrine != null)
    {
      player.position = lastFireShrine.transform.position;
      return;
    }

    player.position = gameStartPoint.position;

  }

  public FireShrine GetLastShrine()
  {
    string lastShrineID = PlayerPrefs.GetString("LastFireShrine", null);
    Debug.Log("Last Shrine ID: " + lastShrineID);
    if (lastShrineID != null)
    {
      var shrines = FindObjectsOfType<FireShrine>();
      foreach (FireShrine shrine in shrines)
      {
        if (shrine.ID == lastShrineID)
        {
          return shrine;
        }
      }
    }

    return null;
  }
}
