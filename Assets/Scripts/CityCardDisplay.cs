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
    public Sprite virusRed;
    public Sprite virusYellow;
    public Sprite virusBlue;
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

        switch (cityCardData.virusColor)
        {
            case VirusColor.Red:
                virusTop.sprite = virusRed;
                virusBottom.sprite = virusRed;
                backgroundColor.color = new Color(1,0.2f,0.2f);
                break;
            case VirusColor.Yellow:
                virusTop.sprite = virusYellow;
                virusBottom.sprite = virusYellow;
                backgroundColor.color = new Color(1,0.8f,0.2f);
                break;
            case VirusColor.Blue:
                virusTop.sprite = virusBlue;
                virusBottom.sprite = virusBlue;
                backgroundColor.color = new Color(0,0.4f,1);
                break;
            default:
                break;
        }

    }
}
