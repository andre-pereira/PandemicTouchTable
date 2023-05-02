using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ENUMS;

public class InfectionCardDisplay : MonoBehaviour
{
    public CityCard cityCardData;

    public Image background;
    public Image virus;

    public Image artwork;

    public TextMeshProUGUI cityName;

    // Start is called before the first frame update
    void Start()
    {
        cityName.text = cityCardData.cityName;
        artwork.sprite = cityCardData.mainArtwork;
        virus.sprite = cityCardData.virusIcon.artwork;
        background.color = cityCardData.virusIcon.virusColor;

    }
}