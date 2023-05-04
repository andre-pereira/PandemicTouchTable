using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MainMenu : MonoBehaviour {

  public GameObject LoadCanvas;
  public GameObject MainMenuCanvas;

  public Button PlayButton;
  public Button ResumeButton;

  public GameObject CreditsDialog;
  public GameObject SavedGameEntryPrefab;

  // Use this for initialization
  public PlayerLoginArea[] PlayerLoginAreas;
  HashSet<Player.PlayerColors> _freeColors = new HashSet<Player.PlayerColors>();

  void Start() {
    init();
  }
  public void init()
  {
    // This code will only execute during development. If the MainMenuGUI is turned off, just skip all this
    // and let the GameGUI load the last game.
    if (!MainMenuCanvas.activeInHierarchy)
      return;

    LoadCanvas.SetActive(false);
    setPlayButtonState();

    if (PlayerPrefs.GetString(Game.PlayerPrefSettings.LAST_FILE_LOADED.ToString(), "NONE") == "NONE")
      ResumeButton.interactable = false;

    foreach (Player.PlayerColors color in Enum.GetValues(typeof(Player.PlayerColors)))
      _freeColors.Add(color);

    foreach (PlayerLoginArea area in PlayerLoginAreas)
      area.reset();

    AudioPlayer.PlayClip(AudioPlayer.AudioClipEnum.INTRO, 1);
  }

  public void OnPlayButton()
  {
    // Stop playing the intro music
    AudioPlayer.Stop();
    AudioPlayer.PlayClip(AudioPlayer.AudioClipEnum.CLICK);

    // Turn off the main menu and start the game
    MainMenuCanvas.SetActive(false);
    GameGUI.theGameGUI.startOrLoadGame();

    // Init the timeline with initial events
    Timeline.theTimeline.reset();
    Timeline.theTimeline.addEvent(new EResetGame());
    foreach (PlayerLoginArea area in PlayerLoginAreas)
    {
      if (area.isPlaying())
        Timeline.theTimeline.addEvent(new EAddPlayer(area.Position, area.Color.Value));
    }

    EInitialize initEvt = new EInitialize();

    Timeline.theTimeline.addEvent(initEvt);

    string saveName = Timeline.theTimeline.save(null);
    PlayerPrefs.SetString(Game.PlayerPrefSettings.LAST_FILE_LOADED.ToString(),
      saveName);
  }

  public void OnLoadButton()
  {
    AudioPlayer.PlayClip(AudioPlayer.AudioClipEnum.CLICK);
    // Bring up (or hide) the Load screen
    MainMenuCanvas.SetActive(!MainMenuCanvas.activeInHierarchy);
    LoadCanvas.SetActive(!LoadCanvas.activeInHierarchy);

    if (LoadCanvas.activeInHierarchy)
      StartCoroutine(populateSavedGameList());
    else
      setPlayButtonState();
  }
  public void OnResumeButton()
  {
    loadGame(PlayerPrefs.GetString(Game.PlayerPrefSettings.LAST_FILE_LOADED.ToString()));
  }
  public void OnQuitButton()
  {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
  }

  IEnumerator populateSavedGameList()
  {
    Transform savedGameList = LoadCanvas.transform.Find("Panel/Scroll View/Viewport/Content");
    savedGameList.gameObject.DestroyChildrenImmediate();

    Directory.CreateDirectory(Application.persistentDataPath + "/savedGames");
    foreach (string dirName in Directory.GetDirectories(Application.persistentDataPath + "/savedGames").Reverse() )
    {
      DateTime gameStart = DateTime.MinValue;
      string timeString;
      string strippedDirName;
      try
      {
        strippedDirName = dirName.Split('\\').Last();
        string[] dateParts = strippedDirName.Split('_'); // yyyy_MM_dd_HH_mm_ss
        int year = int.Parse(dateParts[0]);
        int month = int.Parse(dateParts[1]);
        int day = int.Parse(dateParts[2]);
        int hour = int.Parse(dateParts[3]);
        int minute = int.Parse(dateParts[4]);
        int second = int.Parse(dateParts[5]);
        gameStart = new DateTime(year, month, day, hour, minute, second);
        timeString = " @ ";
        if (hour < 10) timeString += "0" + hour; else timeString += hour;
        timeString += ":";
        if (minute < 10) timeString += "0" + ((minute / 5) * 5); else timeString += ((minute / 5) * 5);
      }
      catch (Exception)
      {
        Debug.Log("Skipping directory: " + dirName);
        //Debug.LogException(e);
        continue;
      }
      DateTime midnightToday = DateTime.Today;
      GameObject saveEntry = Instantiate(SavedGameEntryPrefab);
      saveEntry.transform.SetParent(savedGameList);
      saveEntry.transform.localScale = Vector3.one;

      RawImage screenCapture = saveEntry.transform.Find("Screen Capture").GetComponent<RawImage>();
      Text dateText = saveEntry.transform.Find("Date Text").GetComponent<Text>();
      Button loadButton = saveEntry.GetComponent<Button>();

      if (gameStart >= midnightToday)
        dateText.text = "Today" + timeString;
      else if (gameStart >= midnightToday.AddDays(-1))
        dateText.text = "Yesterday" + timeString;
      else if (gameStart >= midnightToday.AddDays(-6))
        dateText.text = gameStart.DayOfWeek.ToString() + timeString;
      else
        dateText.text = gameStart.ToString("d MMM yy") + timeString;

      loadButton.onClick.AddListener(() => loadGame(strippedDirName));

      if (File.Exists(dirName + "/Snapshot.png"))
      {
        string url = "file:///" + dirName.Replace("\\", "/").Replace(" ", "%20") + "/Snapshot.png";
        //var www = new WWW(url);
        //yield return www;
        //Texture2D tex = new Texture2D(www.texture.width, www.texture.height);
        //www.LoadImageIntoTexture(tex);
        //screenCapture.texture = tex;

        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                // Get downloaded texture once the request has completed
                Texture2D tex = DownloadHandlerTexture.GetContent(webRequest);
                screenCapture.texture = tex;
            }
        }
      }
    }
  }

    public void loadGame(string name)
  {
    // Stop the intro music
    AudioPlayer.Stop();
    PlayerPrefs.SetString(Game.PlayerPrefSettings.LAST_FILE_LOADED.ToString(), name);
    // Startup the game
    MainMenuCanvas.SetActive(false);
    LoadCanvas.SetActive(false);
    GameGUI.theGameGUI.startOrLoadGame();
    // Init the timeline with the saved game
    Timeline.theTimeline.reset();
    Timeline.theTimeline.reprocessEvents(Timeline.load(name));
  }
  public void onCreditsClick()
  {
    CreditsDialog.SetActive(!CreditsDialog.activeInHierarchy);
  }
  public void assignColorToPlayer(int position, Player.PlayerColors color)
  {

    if (_freeColors.Contains(color))
    {
      _freeColors.Remove(color);
      if (PlayerLoginAreas[position].Color.HasValue)
        _freeColors.Add(PlayerLoginAreas[position].Color.Value);
    }
    else
    {
      Player.PlayerColors replacementColor;
      PlayerLoginArea otherArea = PlayerLoginAreas.First(a => a.Color == color);
      if (PlayerLoginAreas[position].Color.HasValue)
        replacementColor = PlayerLoginAreas[position].Color.Value;
      else
      {
        replacementColor = _freeColors.First();
        _freeColors.Remove(replacementColor);
      }

      otherArea.Color = replacementColor;
    }
    PlayerLoginAreas[position].Color = color;
    setPlayButtonState();
  }
  public void assignFreeColorToPlayer(int position)
  {
    PlayerLoginAreas[position].Color = _freeColors.First();
    _freeColors.Remove(PlayerLoginAreas[position].Color.Value);
    setPlayButtonState();
  }
  public void releaseColorFromPlayer(int position)
  {
    _freeColors.Add(PlayerLoginAreas[position].Color.Value);
    PlayerLoginAreas[position].Color = null;
    setPlayButtonState();
  }
  void setPlayButtonState()
  {
    int count = PlayerLoginAreas.Count(o => o.isPlaying());
    PlayButton.interactable = count >= PlayerList.MIN_PLAYERS &&
      count <= PlayerList.MAX_PLAYERS;
  }
}
