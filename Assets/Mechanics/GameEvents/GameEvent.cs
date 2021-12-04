using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameEvent", menuName = "jogabilijam/GameEvent", order = 0)]
public class GameEvent : ScriptableObject
{

  public HashSet<GameEventListener> listeners = new HashSet<GameEventListener>();

  public void Raise()
  {
    foreach (var globalEventListener in listeners)
    {
      globalEventListener.OnEventRaised();
    }
  }

  public void Register(GameEventListener listener)
  {
    listeners.Add(listener);
  }

  public void Deregister(GameEventListener listener)
  {
    listeners.Remove(listener);
  }
}
