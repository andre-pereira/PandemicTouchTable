using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using UnityEditor;
using static ENUMS;

public class GameGUI : MonoBehaviour
{
    private Game game = null;
    public static GameGUI theGameGUI = null;

    public Texture PlayerCardBack;
    public GameObject CityCardPrefab;
    public GameObject[] Cities;
    public Material lineMaterial;
    public GameObject EventCardPrefab;
    public EventCard[] Events;

    public GameObject InfectionCardPrefab;
    public Texture InfectionCardBack;
    public GameObject cubePrefab;

    public GameObject EpidemicCardPrefab;

    //public GameObject LoadOverlay;
    public GameObject BackgroundCanvas;
    public GameObject LinesCanvas;
    public GameObject CityCanvas;
    public GameObject TokenCanvas;
    public GameObject PlayerCanvas;
    public GameObject AnimationCanvas;

    public List<PlayerGUI> PlayerPads;
    public RoleCard[] roleCards;
    public float playerUIOpacity;

    public GameObject PlayerDeck;
    public GameObject PlayerDeckDiscard;
    public TextMeshProUGUI PlayerDeckCount;

    public GameObject InfectionDeck;
    public GameObject InfectionDiscard;
    public TextMeshProUGUI InfectionDeckCount;

    public GameObject OutbreakMarker;
    public Transform[] OutbreakMarkerTransforms;

    public GameObject InfectionRateMarker;
    public Transform[] InfectionRateMarkerTransforms;

    public GameObject[] VialTokens;
    public Transform[] VialTokensTransforms;

    public GameObject[] Pawns;
    public Vector2[] PawnPositionInCityOffset;

    public List<GameObject> RedCubes;
    public List<GameObject> YellowCubes;
    public List<GameObject> BlueCubes;

    public TextMeshProUGUI DebugText;

    private Vector3[] redCubePositions;
    private Vector3[] yellowCubePositions;
    private Vector3[] blueCubePositions;

    public static GameObject cloneOnCanvas(GameObject source, GameObject targetCanvas)
    {
        GameObject movingResource = Instantiate(source);
        movingResource.SetActive(true);
        movingResource.transform.SetParent(targetCanvas.transform, false);
        movingResource.transform.rotation = source.transform.rotation;
        movingResource.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        movingResource.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        movingResource.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        movingResource.transform.position = source.transform.position;
        movingResource.GetComponent<RectTransform>().sizeDelta = source.GetComponent<RectTransform>().rect.size;
        movingResource.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        return movingResource;
    }

    void Awake()
    {
        theGameGUI = this;
    }
    // Use this for initialization
    void Start()
    {
        game = Game.theGame;

        saveCubesInitialPositions();

        CreateNeighborLines();
    }

    private void saveCubesInitialPositions()
    {
        redCubePositions = new Vector3[RedCubes.Count];
        yellowCubePositions = new Vector3[YellowCubes.Count];
        blueCubePositions = new Vector3[BlueCubes.Count];
        for (int i = 0; i < RedCubes.Count; i++)
        {
            redCubePositions[i] = RedCubes[i].transform.position;
            yellowCubePositions[i] = YellowCubes[i].transform.position;
            blueCubePositions[i] = BlueCubes[i].transform.position;
        }
    }

    private void CreateNeighborLines()
    {
        //go through all Cities and connect them together graphically using a line based on their neighbors, don't repeat connections
        foreach (GameObject city in Cities)
        {
            City cityScript = city.GetComponent<City>();
            foreach (int neighbor in cityScript.city.neighbors)
            {
                if (neighbor > cityScript.city.cityID)
                {
                    GameObject line = new GameObject("Line - " + cityScript.transform.name + "_" + Cities[neighbor].transform.name);
                    line.transform.SetParent(LinesCanvas.transform, false);
                    line.transform.position = cityScript.transform.position;
                    line.AddComponent<LineRenderer>();
                    LineRenderer lr = line.GetComponent<LineRenderer>();
                    lr.sortingLayerName = "Lines";
                    lr.material = lineMaterial;
                    lr.startColor = Color.white;
                    lr.endColor = Color.white;
                    lr.startWidth = 0.025f;
                    lr.endWidth = 0.025f;
                    lr.SetPosition(0, cityScript.transform.position);
                    lr.SetPosition(1, Cities[neighbor].transform.position);
                }
            }
        }
    }

    private void Update()
    {
        DebugText.text = "Pending event?: " + Timeline.theTimeline.hasPendingEvent() + "\n";
        if (Game.theGame.CurrentPlayer != null)
            DebugText.text += "Current Player: " + Game.theGame.CurrentPlayer.Role + "\n";
        //add debug text to check if an animation is running
        //DebugText.text += "Animation running?: " + Timeline.theTimeline.isAnimationRunning() + "\n";
    }

    void OnDestroy()
    {
        theGameGUI = null;
    }

    public static PlayerGUI currentPlayerPad()
    {
        if (Game.theGame.CurrentPlayer == null)
        {
            Debug.LogError("Requesting playerGUI for the current player which hasn't been set");
            return null;
        }
        return playerPadForPlayer(Game.theGame.CurrentPlayer);
    }

    public static PlayerGUI playerPadForPlayer(Player player)
    {
        return playerPadForPosition(player.Position);
    }

    public static PlayerGUI playerPadForPosition(int position)
    {
        PlayerGUI retVal = theGameGUI.PlayerPads.FirstOrDefault(p => p.Position == position);
        if (retVal == null)
            Debug.LogError("Requesting playerGUI for player at position " + position + " which doesn't exist.");
        return retVal;
    }


    public void draw()
    {
        drawBoard();
        drawPlayerAreas();
    }

    public void drawBoard()
    {
        PlayerDeckCount.text = game.PlayerCards.Count.ToString();
        InfectionDeckCount.text = game.InfectionCards.Count.ToString();
    }

    public void drawPlayerAreas()
    {
        foreach (PlayerGUI pad in theGameGUI.PlayerPads)
        {
            pad.draw();
        }
    }

    public void saveAndExit()
    {
        AudioPlayer.PlayClip(AudioPlayer.AudioClipEnum.CLICK);

        Timeline.theTimeline.save(PlayerPrefs.GetString(Game.PlayerPrefSettings.LAST_FILE_LOADED.ToString()));
        Timeline.theTimeline.saveScreenshot(PlayerPrefs.GetString(Game.PlayerPrefSettings.LAST_FILE_LOADED.ToString()));
        SceneManager.LoadScene(0);
    }
    public void undoAction()
    {
        AudioPlayer.Stop();
        StopAllCoroutines();
        AudioPlayer.PlayClip(AudioPlayer.AudioClipEnum.CLICK);
        DOTween.CompleteAll(true);
        Dictionary<int, float> times = new Dictionary<int, float>();
        foreach (Player p in PlayerList.Players)
            times[p.Position] = p.ActingTime;
        Timeline.theTimeline.undo();
        foreach (var kvp in times)
            PlayerList.playerAtPosition(kvp.Key).ActingTime = kvp.Value;
    }

    // During development, I'll often add a save/load button to the screen so that I can quickly
    // see that the save file looks correct and load a save. In a final game, these buttons go away
    // and are replaced with a "Save and Exit" button
    public void save()
    {
        Timeline.theTimeline.save(PlayerPrefs.GetString(Game.PlayerPrefSettings.LAST_FILE_LOADED.ToString()));
    }
    public void load()
    {
        /*
        PlayerPrefs.SetString(Game.PlayerPrefSettings.FILE_TO_LOAD.ToString(), "test");
        Game.loadLevel();
        */
    }

    public List<GameObject> GetCubesList(VirusInfo virusInfo)
    {
        switch (virusInfo.virusName)
        {
            case VirusName.Red:
                return RedCubes;
            case VirusName.Yellow:
                return YellowCubes;
            case VirusName.Blue:
                return BlueCubes;
            default:
                Debug.LogError("Unknown virus name: " + virusInfo.virusName);
                return null;
        }
    }
}
