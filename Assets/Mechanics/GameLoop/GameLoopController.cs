using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameLoopController : MonoBehaviour
{

  [Header("References")]
  public Transform player;
  public Transform gameStartPoint;
  public ScreenTransition screenTransition;
  public Cinemachine.CinemachineConfiner cinemachineConfiner;

  public PolygonCollider2D startConfiner;

  public Image gameTitle;
  public Image gameTitle2;

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
    cinemachineConfiner.m_BoundingShape2D = startConfiner;

    if (PlayerPrefs.GetInt("GameStarted", 0) == 0)
    {
      ShowGameTitle();
    }
    else
    {
      player.position = gameStartPoint.position;
    }

    PlayerPrefs.SetInt("GameStarted", 1);

    screenTransition.FadeIn();
    PlacePlayerAtCorrectPosition();
  }

  public void ShowGameTitle()
  {
    gameTitle.enabled = true;
    gameTitle2.enabled = true;
    StartCoroutine(IntroWait());
  }

  float introWaitTime = 2f;
  IEnumerator IntroWait()
  {
    yield return new WaitForSeconds(introWaitTime);
    gameTitle2.DOFade(1, 1f).SetEase(Ease.InOutQuad);
    gameTitle.DOFade(0, 2f).SetEase(Ease.InOutQuad).OnComplete(() =>
    {
      gameTitle.enabled = false;
      StartCoroutine(IntroWait2());

    });
  }

  IEnumerator IntroWait2()
  {
    yield return new WaitForSeconds(introWaitTime * 7);
    gameTitle2.DOFade(0, 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
    {
      gameTitle2.enabled = false;
    });
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
