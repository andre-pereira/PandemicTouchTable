using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static ENUMS;
using static GameGUI;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using static UnityEditor.ShaderData;

public class Game : MonoBehaviour
{

    public const int CALLTOMOBILIZEINDEX = 24;
    public const int FORECASTINDEX = 25;
    public const int MOBILEHOSPITALINDEX = 26;
    public const int RESOURCEPLANNINGINDEX = 27;

    public Player MobileHospitalPlayer = null;


    public enum PlayerPrefSettings
    {
        LAST_FILE_LOADED
    }


    public enum GameState
    {
        INVALID = -1,
        SETTINGBOARD,
        PLAYERACTIONS,
        DRAWPLAYERCARDS,
        EPIDEMIC,
        DRAWINFECTCARDS,
        OUTBREAK,
        GAME_OVER
    }

    public enum EventState
    {
        NOTINEVENT,
        CONFIRMINGFORECAST,
        CONFIRMINGRESOURCEPLANNING,
        CONFIRMINGCALLTOMOBILIZE,
        CONFIRMINGMOBILEHOSPITAL,
        FORECAST,
        RESOURCEPLANNING,
        CALLTOMOBILIZE,
        EXECUTINGMOBILEHOSPITAL //Only used for PlayerGUI. The game logic is controlled by the flag MobileHospitalInExecution.
    }

    public enum EpidemicGameState
    {

        EPIDEMICINCREASE,
        EPIDEMICINFECT,
        EPIDEMICINTENSIFY
    }

    public int PlayerCardsSeed = -1; // -1 randomizes the game
    public int InfectionCardsSeed = -1; // -1 randomizes the game
    
    public Random.State playerCardsRandomGeneratorState;
    public Random.State infectionCardsRandomGeneratorState;
    
    public int InfectionRate = 0;
    public int[] InfectionRateValues = new int[] { 2, 2, 3, 4 };
    public int NumberOfDrawnInfectCards = 0;

    public int OutbreakCounter = 0;
    public List<int> OutbreakTracker = new List<int>();

    public int InitialCityID = 13;

    public static Game theGame = null;

    public Player CurrentPlayer = null;

    public GameState CurrentGameState { get; private set; } = GameState.INVALID;
    public GameState previousGameState { get; private set; } = GameState.INVALID;
    public EpidemicGameState epidemicGameState = EpidemicGameState.EPIDEMICINCREASE;

    private bool actionsInitiated = false;
    public bool actionCompleted = false;

    public List<int> PlayerCards = null;
    public List<int> PlayerCardsDiscard = null;

    public List<int> InfectionCards = null;
    public List<int> InfectionCardsDiscard = null;
    public int PlayerCardsDrawn;
    public int RedCubesOnBoard = 16;
    public int YellowCubesOnBoard = 16;
    public int BlueCubesOnBoard = 16;

    public bool RedCure = false;
    public bool YellowCure = false;
    public bool BlueCure = false;
    private bool turnEnded = false;

    public EventState InEventCard = EventState.NOTINEVENT;

    public City[] Cities { get; internal set; }
    public bool MobileHospitalInExecution { get; set; }

    public void init()
    {
        PlayerCards = new List<int>();
        PlayerCardsDiscard = new List<int>();
        InfectionCards = new List<int>();
        InfectionCardsDiscard = new List<int>();

        PlayerCardsDrawn = 0;

        InfectionRate = 0;
        OutbreakCounter = 0;
        MobileHospitalInExecution = false;

    }

    public int GetCurrentInfectionRate()
    {
        return InfectionRateValues[InfectionRate];
    }

    public void test()
    {
        //if(CurrentPlayer.PlayerCardsInHand.Count < 7)
        //    Timeline.theTimeline.addEvent(new EDealCardToPlayer(CurrentPlayer));
        Timeline.theTimeline.addEvent(new EOutbreak(Cities[InitialCityID]));
    }

    public void Update()
    {
        
        if (InEventCard == EventState.CALLTOMOBILIZE)
        {
            //for all players check if they are done with this event
            if (PlayerList.getAllPlayers().All(player => player.playerGui.eventExecuted == true))
            {
                InEventCard = EventState.NOTINEVENT;
                foreach (Player player in PlayerList.getAllPlayers())
                {
                    player.playerGui.ChangeToInEvent(EventState.NOTINEVENT);
                }
            }
        }
        else if (InEventCard != EventState.NOTINEVENT) return;

        if (PlayerList.getAllPlayers().Any(player => player.PlayerCardsInHand.Count > 6)) return;
        
        if (CurrentGameState == GameState.DRAWPLAYERCARDS)
        {
            //Debug.Log("No more cards in the deck !");
            //Debug.Log("Cards drawn = " + PlayerCardsDrawn);
            if (PlayerCardsDrawn == 0 && PlayerCards.Count < 2) 
            {
                setCurrentGameState(GameState.GAME_OVER);
            }
            else {
                if (!actionsInitiated)
                {
                    actionsInitiated = true;
                    Debug.Log("Draw Player Card: " + CurrentGameState + " Cards available: " + PlayerCards.Count);
                    Timeline.theTimeline.addEvent(new PDealCard(CurrentPlayer));
                }
                if(actionCompleted)
                {
                    actionCompleted = false;
                    if (PlayerCardsDrawn < 2)
                    {
                        actionsInitiated = false;
                    }
                    else if (CurrentPlayer.PlayerCardsInHand.Count < 7 && PlayerCardsDrawn == 2)
                    {
                        if (CurrentGameState != GameState.EPIDEMIC)
                            setCurrentGameState(GameState.DRAWINFECTCARDS);
                    }
                }
            }   
        }
        else if (CurrentGameState == GameState.EPIDEMIC)
        {
            if (epidemicGameState == EpidemicGameState.EPIDEMICINCREASE)
            {
                Timeline.theTimeline.addEvent(new EIncreaseInfectionRate());
                epidemicGameState = EpidemicGameState.EPIDEMICINFECT;
                Timeline.theTimeline.addEvent(new EDrawInfectionCard(3, true));
            }
            else if (epidemicGameState == EpidemicGameState.EPIDEMICINFECT)
            {
                if (actionCompleted)
                {
                    epidemicGameState = EpidemicGameState.EPIDEMICINTENSIFY;
                    Timeline.theTimeline.addEvent(new EIntensify());
                    setCurrentGameState(previousGameState);
                }
            }
        }
        else if (CurrentGameState == GameState.OUTBREAK)
        {
            if (actionCompleted)
            {
                OutbreakTracker.Clear();
                setCurrentGameState(GameState.DRAWINFECTCARDS);
            }
        }

            else if (CurrentGameState == GameState.DRAWINFECTCARDS)
        {
            if (NumberOfDrawnInfectCards < InfectionRateValues[InfectionRate] && !turnEnded)
            {
                if (!actionsInitiated)
                {
                    actionsInitiated = true;
                    Timeline.theTimeline.addEvent(new EDrawInfectionCard(1, true));
                }
                if (actionCompleted)
                {
                    actionsInitiated = false;
                    actionCompleted = false;
                }
            }
            else
            {
                if (!turnEnded)
                {
                    turnEnded = true;
                    Timeline.theTimeline.addEvent(new PEndTurn());
                }
            }
        }
    }

    public void Awake()
    {
        theGame = this;
    }

    public void OnDestroy()
    {
        if (theGame == this) theGame = null;
    }

    internal void setCurrentGameState(GameState state)
    {
        turnEnded = false;
        previousGameState = CurrentGameState;
        CurrentGameState = state;
        actionsInitiated = false;
        actionCompleted = false;
        if (state == GameState.PLAYERACTIONS)
        {
            PlayerCardsDrawn = 0;
            NumberOfDrawnInfectCards = 0;
        }

        if (state == GameState.EPIDEMIC)
            epidemicGameState = EpidemicGameState.EPIDEMICINCREASE;
    }

    public void incrementNumberOfCubesOnBoard(VirusName virus, int increment)
    {
        switch (virus)
        {
            case VirusName.Red:
                RedCubesOnBoard += increment;
                break;
            case VirusName.Yellow:
                YellowCubesOnBoard += increment;
                break;
            case VirusName.Blue:
                BlueCubesOnBoard += increment;
                break;
        }
    }

    public int DistanceFromCity(int cityID1, int cityID2)
    {
        if(cityID1 == cityID2)
            return 0;

        int distance = 0;
        HashSet<int> citiesToVisit = new HashSet<int>();
        HashSet<int> citiesVisited = new HashSet<int>();
        bool foundConnection = false;

        HashSet<int> newCitiesToVisit = new HashSet<int>();

        newCitiesToVisit.UnionWith(Cities[cityID2].city.neighbors);

        for (int i = 0; i < Cities.Length; i++)
        {
            distance++;
            citiesToVisit = new HashSet<int>(newCitiesToVisit);
            newCitiesToVisit.RemoveWhere(citiesVisited.Contains);
            citiesVisited.UnionWith(citiesToVisit);

            if (citiesVisited.Contains(cityID1))
            {
                foundConnection = true;
                break;
            }

            foreach (int city in citiesToVisit)
            {
                foreach (int neighbor in Game.theGame.Cities[city].city.neighbors)
                {
                    newCitiesToVisit.Add(neighbor);
                }
            }
        }

        if (foundConnection)
        {
            return distance;
        }
        else return -1;
    }

    internal void ChangeToInEvent(EventState state, Player callToMobilizePendingPlayer = null)
    // callToMobilizePendingPlayer is an optional parameter only used when a mobile hospital event occurs during
    // an ongoing call to mobilize event. This caused the MobileHospitalPlayer to remove a cube thus interrupting
    // the call to mobilize event for that specific player.
    {
        InEventCard = state;
        if(state == EventState.CALLTOMOBILIZE)
        {
            if (callToMobilizePendingPlayer != null)
            {
                callToMobilizePendingPlayer.playerGui.DestroyMovingPawn();
                //callToMobilizePendingPlayer.GetCurrentCityScript().addPawn(callToMobilizePendingPlayer); // The player is added back to the city after being removed by the first call to CreateMovingPawn
                callToMobilizePendingPlayer.playerGui.ChangeToInEvent(state);
                int pawnNumber = 0;
                foreach (var item in callToMobilizePendingPlayer.GetCurrentCityScript().PawnsInCity)
                {
                    if (item != null)
                    {
                        if (item.PlayerModel.Role == callToMobilizePendingPlayer.Role)
                            break;
                        pawnNumber++;
                    }
                }
                Vector3[] MovingPawnTranslations = new Vector3 [4] { new Vector3(0, 0, 0), new Vector3(0, 0.5f, 0), new Vector3(0, -0.5f, 0), new Vector3(1, 0, 0) }; 
                callToMobilizePendingPlayer.playerGui.CreateMovingPawn(MovingPawnTranslations[pawnNumber]);        
            }
            else
            {
                foreach (Player player in PlayerList.getAllPlayers())
                {
                    player.playerGui.ChangeToInEvent(state);
                    int pawnNumber = 0;
                    foreach (var item in player.GetCurrentCityScript().PawnsInCity)
                    {
                        if (item != null)
                        {
                            if (item.PlayerModel.Role == player.Role)
                                break;
                            pawnNumber++;
                        }
                    }
                    Vector3[] MovingPawnTranslations = new Vector3 [4] { new Vector3(0, 0, 0), new Vector3(0, 0.5f, 0), new Vector3(0, -0.5f, 0), new Vector3(1, 0, 0) }; 
                    player.playerGui.CreateMovingPawn(MovingPawnTranslations[pawnNumber]);            
                } 
            }
           
        }
    }

    internal void MakePlayersWait()
    {
        foreach (Player player in PlayerList.getAllPlayers())
        {
            player.playerGui.Waiting = true;
        }
    }

    internal void RemovePlayersWait()
    {
        foreach (Player player in PlayerList.getAllPlayers())
        {
            player.playerGui.Waiting = false;
            player.playerGui.draw();
        }
        CurrentPlayer.playerGui.ClearSelectedAction();
    }

    public GameObject AddPlayerCardToTransform(int cardToAdd, Transform transform, bool withButtonComponent, PlayerGUI pGUI = null, Transform adjustTransform = null)
    {
        GameObject cardToAddObject;
        if (cardToAdd > 23)
        {
            cardToAddObject = Instantiate(gui.EventCardPrefab, transform);
            cardToAddObject.GetComponent<EventCardDisplay>().EventCardData = gui.Events[cardToAdd - 24];
            if (pGUI != null)
            {
                if (pGUI.selectedCards.Contains(cardToAdd))
                {
                    cardToAddObject.GetComponent<EventCardDisplay>().border.gameObject.SetActive(true);
                }
                else
                {
                    cardToAddObject.GetComponent<EventCardDisplay>().border.gameObject.SetActive(false);
                }
            }

        }
        else
        {
            cardToAddObject = Instantiate(gui.CityCardPrefab, transform);
            cardToAddObject.GetComponent<CityCardDisplay>().CityCardData = Cities[cardToAdd].city;

        }
        if (withButtonComponent)
        {
            var buttonComponent = cardToAddObject.AddComponent<Button>();
            buttonComponent.onClick.AddListener(() => pGUI.CardInHandClicked(cardToAdd));
        }

        if (adjustTransform != null)
        {
            cardToAddObject.transform.rotation = adjustTransform.rotation;
            cardToAddObject.transform.position = adjustTransform.position;
        }

        return cardToAddObject;
    }

    internal void CubeClicked(City city, VirusName virusName)
    {
        if(MobileHospitalPlayer != null && MobileHospitalInExecution)
        {
            Timeline.theTimeline.addEvent(new PMobileHospitalEvent(MobileHospitalPlayer, city, virusName));
            if (CurrentPlayer.ActionsRemaining == 0)
            {
                theGame.setCurrentGameState(GameState.DRAWPLAYERCARDS);
            }
        }    
        else 
            currentPlayerPad().CubeClicked(city, virusName);
    }
}
