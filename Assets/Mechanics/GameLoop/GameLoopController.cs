using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameLoopController : MonoBehaviour
{

  [Header("References")]
  public Transform player;
  public Transform gameStartPoint;
  public ScreenTransition screenTransition;
  public Cinemachine.CinemachineConfiner cinemachineConfiner;

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
    if (playerHasStartedTheGame)
    {
      screenTransition.FadeIn();
    }
    PlacePlayerAtCorrectPosition();
  }

  bool playerHasStartedTheGame => (PlayerPrefs.GetString("LastFireShrine", null) != null);

  public void PlacePlayerAtCorrectPosition()
  {
    FireShrine lastFireShrine = GetLastShrine();
    if (lastFireShrine != null)
    {
      player.position = lastFireShrine.transform.position;
      cinemachineConfiner.m_BoundingShape2D = lastFireShrine.cineMachineConfiner;
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


  public void RestartScene()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public void RestartProgress(InputAction.CallbackContext context)
  {
    Debug.Log("Restarting Progress");
    if (context.performed)
    {
      PlayerPrefs.DeleteAll();
      RestartScene();
    }
  }

}
