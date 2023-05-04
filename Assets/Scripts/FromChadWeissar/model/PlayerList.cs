using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;

public class PlayerList : MonoBehaviour {

  static PlayerList thePlayerList = null;
  public static List<Player> Players { get { return thePlayerList._players; } set { thePlayerList._players = value; } }

  List<Player> _players = new List<Player>();

  public const int MIN_PLAYERS = 3;
  public const int MAX_PLAYERS = 7;
  
  void OnEnable()
  {
    thePlayerList = this;
  }
  void OnDestroy()
  {
    thePlayerList = null;
  }
  public static void shuffle()
  {
    Players.Shuffle();
  }
  public static Player playerAtPosition(int position)
  {
    return Players.Find(p => p.Position == position);
  }
  public static Player nextPlayer(Player player)
  {
    return playerLeftOfPlayer(player);
  }
  public static Player playerLeftOfPlayer(Player player)
  {
    int pIndex = Players.IndexOf(player);
    int index = (pIndex == Players.Count - 1) ? 0 : pIndex + 1;
    return Players[index];
  }
  public static Player playerRightOfPlayer(Player player)
  {
    int pIndex = Players.IndexOf(player);
    int index = (pIndex == 0 ? Players.Count - 1 : pIndex - 1);
    return Players[index];
  }
  public static void setOrderToClockwiseWithStartAt(Player player)
  {
    thePlayerList._players =
      thePlayerList._players.OrderBy(
        p => (p.Position - player.Position+MAX_PLAYERS) % MAX_PLAYERS).ToList();
  }
  public static void setScoreOrder()
  {
    float prevScore = float.MaxValue;
    int currPlace = 0;
    int realPlace = 0;
    foreach (Player p in thePlayerList._players
      .OrderByDescending(p => p.totalScoreWithTieBreakers()))
    {
      if (p.totalScoreWithTieBreakers() != prevScore)
      {
        // First player will always trigger this code.
        prevScore = p.totalScoreWithTieBreakers();
        currPlace = realPlace;
      }
      p.Place = currPlace;
      ++realPlace;
    }
  }
}
