using System.Linq;
using TMPro;
using UnityEngine;
using static GameGUI;
using static Game;

[System.Serializable]
public class EInitialize : EngineEvent
{
    
    private int PlayerCardsSeed;
    private int InfectionCardsSeed;

    
    public EInitialize()
    {
        QUndoable = false;
    }

    public override void Do(Timeline c)
    {
        initializeSeeds();
        initializeModel();
        initializeGUI();

        // Find all TextMeshProUGUI components in the scene
        TextMeshProUGUI[] allTMPTexts = Object.FindObjectsOfType<TextMeshProUGUI>();

        // Loop through each one and disable its raycastTarget property
        foreach (TextMeshProUGUI tmpText in allTMPTexts)
        {
            tmpText.raycastTarget = false;
        }

        c.addEvent(new EStartRound());
    }

    public void initializeSeeds()
    {
        int randomSeed = Mathf.Abs(System.DateTime.UtcNow.Ticks.GetHashCode());
        
        PlayerCardsSeed = theGame.PlayerCardsSeed == -1 ? randomSeed : theGame.PlayerCardsSeed;

        InfectionCardsSeed = theGame.InfectionCardsSeed == -1 ? randomSeed : theGame.InfectionCardsSeed;

        Random.InitState(PlayerCardsSeed);
        theGame.playerCardsRandomGeneratorState = Random.state;
        
        Random.InitState(InfectionCardsSeed);
        theGame.infectionCardsRandomGeneratorState = Random.state;
    }

    public void initializeModel()
    {
        theGame.setCurrentGameState(Game.GameState.SETTINGBOARD);

        //0 to 23 are city cards and 24 to 27 are event cards
        theGame.PlayerCards = Enumerable.Range(0, 28).ToList();
        theGame.PlayerCards.Shuffle(theGame.playerCardsRandomGeneratorState);
        theGame.playerCardsRandomGeneratorState = Random.state;

        ////TODO: remove this
        //for (int i = 0; i < 4; ++i)
        //    theGame.PlayerCards.Add(24+i);

        theGame.InfectionCards = Enumerable.Range(0, 24).ToList();
        theGame.InfectionCards.Shuffle(theGame.infectionCardsRandomGeneratorState);
        theGame.infectionCardsRandomGeneratorState = Random.state;

        int numCardsToDeal = PlayerList.Players.Count == 2 ? 3 : 2;
        foreach (Player player in PlayerList.Players)
        {
            for (int i = 0; i < numCardsToDeal; ++i)
            {
                Timeline.theTimeline.addEvent(new PDealCard(player));
            }
        }

        Timeline.theTimeline.addEvent(new EAddEpidemicCards());
        Timeline.theTimeline.addEvent(new EDrawInfectionCard(3, true));
        Timeline.theTimeline.addEvent(new EDrawInfectionCard(3, true));
        Timeline.theTimeline.addEvent(new EDrawInfectionCard(2, true));
        Timeline.theTimeline.addEvent(new EDrawInfectionCard(2, true));
        Timeline.theTimeline.addEvent(new EDrawInfectionCard(1, true));
        Timeline.theTimeline.addEvent(new EDrawInfectionCard(1, true));
        Timeline.theTimeline.addEvent(new EInitializeFirstPlayer());

        ////foreach city add a cube of each color
        //foreach (City city in theGame.Cities)
        //{
        //    city.incrementNumberOfCubes(VirusName.Yellow, 1);
        //    city.incrementNumberOfCubes(VirusName.Red, 1);
        //    city.incrementNumberOfCubes(VirusName.Blue, 1);
        //}
    }



    public void initializeGUI()
    {
        foreach (PlayerGUI playerGUI in gui.PlayerPads)
        {
            if (PlayerList.Players.Any(p => p.Position == playerGUI.Position))
            {
                playerGUI.init();
            }
            else
                playerGUI.gameObject.SetActive(false);
        }
       gui.PlayerPads = gui.PlayerPads.Where(p => p.gameObject.activeSelf).ToList();
       gui.draw();
    }

    public override float Act(bool qUndo)
    {
        AudioPlayer.PlayClip(AudioPlayer.AudioClipEnum.SHUFFLE);
        gui.draw();
        return 0;
    }
}
