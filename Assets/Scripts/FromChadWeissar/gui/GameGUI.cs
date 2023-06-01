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
using static GameGUI;
using static Game;
using UnityEngine.EventSystems;

public class GameGUI : MonoBehaviour
{

    public static GameGUI gui = null;

    public TextMeshProUGUI BigTextMessage;

    public Sprite[] ContextButtonTextures;

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
    public Image EpidemicCardBoard;

    public GameObject CureVialPrefab;

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
    public GameObject InfectionCardBackPrefab;
    public TextMeshProUGUI InfectionDeckCount;

    public GameObject OutbreakMarkerPrefab;
    public Transform[] OutbreakMarkerTransforms;

    public GameObject InfectionRateMarkerPrefab;
    public Transform[] InfectionRateMarkerTransforms;

    public GameObject[] VialTokens;
    public Transform[] VialTokensTransforms;

    public GameObject Pawns;
    public GameObject PawnPrefab;

    public List<GameObject> RedCubes;
    public List<GameObject> YellowCubes;
    public List<GameObject> BlueCubes;
    public VirusInfo[] VirusInfos;

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
        gui = this;
    }
    // Use this for initialization
    void Start()
    {
        moveCityInfoToGame();

        saveCubesInitialPositions();

        CreateNeighborLines();

    }

    private void moveCityInfoToGame()
    {
        theGame.Cities = new City[Cities.Length];
        for (int i = 0; i < Cities.Length; i++)
        {
            theGame.Cities[i] = Cities[i].GetComponent<City>();
        }
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


    void OnDestroy()
    {
        gui = null;
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
        PlayerGUI retVal = gui.PlayerPads.FirstOrDefault(p => p.Position == position);
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
        PlayerDeckCount.text = theGame.PlayerCards.Count.ToString();
        InfectionDeckCount.text = theGame.InfectionCards.Count.ToString();

        foreach (Transform item in OutbreakMarkerTransforms)
        {
            item.gameObject.DestroyChildrenImmediate();
        }

        foreach (Transform item in InfectionRateMarkerTransforms)
        {
            item.gameObject.DestroyChildrenImmediate();
        }

        Instantiate(InfectionRateMarkerPrefab, InfectionRateMarkerTransforms[theGame.InfectionRate].position, InfectionRateMarkerTransforms[theGame.InfectionRate].rotation, InfectionRateMarkerTransforms[theGame.InfectionRate]);
        Instantiate(OutbreakMarkerPrefab, OutbreakMarkerTransforms[theGame.OutbreakCounter].position, OutbreakMarkerTransforms[theGame.OutbreakCounter].rotation, OutbreakMarkerTransforms[theGame.OutbreakCounter]);

        drawCureVialsOnBoard();

        drawBigContextText();

    }

    private void drawCureVialsOnBoard()
    {
        for (int i = 0; i < VialTokensTransforms.Length; i++)
        {
            VialTokensTransforms[i].gameObject.DestroyChildrenImmediate();
        }

        if (theGame.RedCure)
        {
            GameObject vial = Instantiate(CureVialPrefab, VialTokensTransforms[0].position, VialTokensTransforms[0].rotation, VialTokensTransforms[0]);
            vial.GetComponent<Image>().color = gui.VirusInfos[0].virusColor; 
        }

        if (theGame.YellowCure)
        {
            GameObject vial = Instantiate(CureVialPrefab, VialTokensTransforms[1].position, VialTokensTransforms[1].rotation, VialTokensTransforms[1]);
            vial.GetComponent<Image>().color = gui.VirusInfos[1].virusColor;
        }

        if (theGame.BlueCure)
        { 
            GameObject vial = Instantiate(CureVialPrefab, VialTokensTransforms[2].position, VialTokensTransforms[2].rotation, VialTokensTransforms[2]);
            vial.GetComponent<Image>().color = gui.VirusInfos[2].virusColor;
        }

    }

    public void drawPlayerAreas()
    {
        foreach (PlayerGUI pad in gui.PlayerPads)
        {
            pad.draw();
        }
    }

    public void drawCurrentPlayerArea()
    {
        foreach (PlayerGUI pad in gui.PlayerPads)
        {
            if (theGame.CurrentPlayer == pad.PlayerModel)
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
        Timeline.theTimeline.undo();
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

    public GameObject getInfectionRateMarker(int targetInfectionRate)
    {
        return InfectionRateMarkerTransforms[targetInfectionRate].gameObject;
    }

    public GameObject getOutbreakMarker(int targetOutbreak)
    {
        return OutbreakMarkerTransforms[targetOutbreak].gameObject;
    }

    public void drawBigContextText()
    {
        switch (theGame.CurrentGameState)
        {
            case GameState.SETTINGBOARD:
                GameGUI.gui.BigTextMessage.text = "Set up phase";
                break;
            case GameState.PLAYERACTIONS:
                GameGUI.gui.BigTextMessage.text = theGame.CurrentPlayer.Name.ToString() + "'s turn";
                break;
            case GameState.DRAW1STPLAYERCARD:
                GameGUI.gui.BigTextMessage.text = "Drawing First Player Card";
                break;
            case GameState.DRAW2NDPLAYERCARD:
                GameGUI.gui.BigTextMessage.text = "Drawing Second Player Card";
                break;
            case GameState.EPIDEMIC:
                switch (theGame.epidemicGameState)
                {
                    case EpidemicGameState.EPIDEMICINCREASE:
                        GameGUI.gui.BigTextMessage.text = "Epidemic: Increase";
                        break;
                    case EpidemicGameState.EPIDEMICINFECT:
                        GameGUI.gui.BigTextMessage.text = "Epidemic: Infect";
                        break;
                    case EpidemicGameState.EPIDEMICINTENSIFY:
                        GameGUI.gui.BigTextMessage.text = "Epidemic: Intensify";
                        break;
                }
                break;
            case GameState.DRAWINFECTCARDS:
                GameGUI.gui.BigTextMessage.text = "Drawing Infection Cards";
                break;
            case GameState.OUTBREAK:
                GameGUI.gui.BigTextMessage.text = "Outbreak!";
                break;
            case GameState.GAME_OVER:
                GameGUI.gui.BigTextMessage.text = "Game Over!";
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        DebugText.text = "State: " + theGame.CurrentGameState + "\n";
        DebugText.text += "Previous: " + theGame.previousGameState + "\n";
        if (theGame.CurrentGameState == GameState.EPIDEMIC)
            DebugText.text += "Epidemic State: " + theGame.epidemicGameState + "\n";
        if (theGame.CurrentPlayer != null)
        {
            DebugText.text += "Current Player: " + theGame.CurrentPlayer.Role + "\n";
            DebugText.text += "Action Selected: " + currentPlayerPad().ActionSelected + "\n";
            DebugText.text += "Actions remaining: " + currentPlayerPad().PlayerModel.ActionsRemaining + "\n";
            DebugText.text += "Cards State: " + currentPlayerPad().cardsState + "\n";
            DebugText.text += "Cards in Hand: " + string.Join(", ", currentPlayerPad().PlayerModel.PlayerCardsInHand) + "\n";
        }
        //add debug text to check if an animation is running
        //DebugText.text += "Animation running?: " + Timeline.theTimeline.isAnimationRunning() + "\n";
    }

}