using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ENUMS;

public class CityCardDisplay : MonoBehaviour
{
    public CityCard cityCardData;


    public Image backgroundColor;
    public Image backgroundWhite;
    public Image virusTop;
    public Image virusBottom;

    public Image artwork;
    public Image flag;
    public TextMeshProUGUI population;
    public TextMeshProUGUI cityName;
    public TextMeshProUGUI countryName;

    // Start is called before the first frame update
    void Start()
    {
        population.text = cityCardData.population;
        cityName.text = cityCardData.cityName;
        countryName.text = cityCardData.countryName;
        artwork.sprite = cityCardData.mainArtwork;
        flag.sprite = cityCardData.flagArtwork;
        virusTop.sprite = cityCardData.virusIcon.artwork;
        virusBottom.sprite = cityCardData.virusIcon.artwork;
        backgroundColor.color = cityCardData.virusIcon.virusColor;

    }
}
