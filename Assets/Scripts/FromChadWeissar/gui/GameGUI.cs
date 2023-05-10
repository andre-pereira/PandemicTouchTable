using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;

public class GameGUI : MonoBehaviour
{

    public static GameGUI theGameGUI = null;

    //public GameObject LoadOverlay;
    public GameObject GameCanvas;
    public GameObject AnimationCanvas;

    public List<PlayerGUI> PlayerPads;
    public RoleCard[] roleCards;
    public float playerUIOpacity;

    public static GameObject cloneOnCanvas(GameObject source)
    {
        GameObject movingResource = Instantiate(source);
        movingResource.SetActive(true);
        movingResource.transform.SetParent(GameGUI.theGameGUI.AnimationCanvas.transform, false);
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
        //LoadOverlay.SetActive(Game.theGame.CurrentGameState == Game.GameState.LOGIN);
        GameCanvas.SetActive(Game.theGame.CurrentGameState != Game.GameState.LOGIN);

        drawCenter();

        foreach (PlayerGUI pad in theGameGUI.PlayerPads)
        {
            pad.draw();
        }
    }
    public void drawCenter()
    {
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

}
