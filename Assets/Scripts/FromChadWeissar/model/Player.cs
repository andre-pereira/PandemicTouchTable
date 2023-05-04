using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class Player {
  public enum PlayerColors
  {
    RED, GREEN, BLUE, YELLOW, CYAN, PURPLE, ORANGE
  }

  public enum Statistic
  {
    NUM_TYPES
  }
  #region State
  #endregion

  #region Statistics
  public float ActingTime = 0f;
  public Dictionary<Statistic, int> Statistics = new Dictionary<Statistic, int>();
  #endregion

  public int Position;
  public int Place;
  public PlayerColors Color { get; set; }

  public int NumChips = -1;
  public List<int> AcceptedGifts = null;

  public Player() {
    NumChips = 0;
    AcceptedGifts = new List<int>();

    for (int i = 0; i < (int)Statistic.NUM_TYPES; ++i)
      Statistics[(Statistic)i] = 0;
  }

  public void TakeGift(int num)
  {
    AcceptedGifts.Add(num);
    AcceptedGifts.Sort();
  }
  public Color veryLightColor()
  {
    return GameGUI.VeryLightColors[(int)Color];
  }
  public Color lightColor()
  {
    return GameGUI.LightColors[(int)Color];
  }
  public Color solidColor()
  {
    return GameGUI.SolidColors[(int)Color];
  }

  public int totalScore()
  {
    int giftTotal = 0;
    int priorGift = -1;
    foreach ( int gift in AcceptedGifts )
    {
      if (gift != priorGift + 1)
        giftTotal += gift;
      priorGift = gift;
    }
    return NumChips - giftTotal;
  }
  public float totalScoreWithTieBreakers()
  {
    return totalScore() + 0;
  }
}
