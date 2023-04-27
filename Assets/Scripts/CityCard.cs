using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ENUMS;

[CreateAssetMenu(fileName = "New City Card", menuName = "Cards/City Card")]
public class CityCard : ScriptableObject
{
    public string cityName;
    public string population;
    public string countryName;
    public VirusColor virusColor;

    public Sprite mainArtwork;
    public Sprite flagArtwork;
}
