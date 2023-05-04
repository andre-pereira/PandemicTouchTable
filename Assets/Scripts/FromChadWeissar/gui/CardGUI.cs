using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGUI : MonoBehaviour {

  public TMPro.TextMeshProUGUI CenterNumber;
  public TMPro.TextMeshProUGUI ULNumber;
  public TMPro.TextMeshProUGUI Score;

  public void draw(int number, bool showScore)
  {
    CenterNumber.text = ULNumber.text = number.ToString();
    Score.text = showScore ? "-" + number : "";
  }
}
