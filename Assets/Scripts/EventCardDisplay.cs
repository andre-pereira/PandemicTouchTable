using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ENUMS;

public class EventCardDisplay : MonoBehaviour
{
    public EventCard eventCardData;

    public Image artwork;
    public TextMeshProUGUI eventName;
    public TextMeshProUGUI eventText;


    // Start is called before the first frame update
    void Start()
    {
        eventName.text = eventCardData.eventName;
        eventText.text = eventCardData.eventText;
        artwork.sprite = eventCardData.mainArtwork;

    }
}
