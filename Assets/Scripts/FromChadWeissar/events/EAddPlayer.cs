using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EAddPlayer : EngineEvent
{
    public int TablePositionId;
    public Player.Roles PlayerRole;
    public string PlayerName;

    GameGUI gui = GameGUI.theGameGUI;
    Game game = Game.theGame;

    EAddPlayer()
    {
        QUndoable = false;
    }
    public EAddPlayer(int tableId, Player.Roles role, string name)
    {
        TablePositionId = tableId;
        PlayerRole = role;
        PlayerName = name;
    }

    public override void Do(Timeline c)
    {
        Player p = new Player() { Position = TablePositionId, Role = PlayerRole, Name = PlayerName };
        PlayerList.Players.Add(p);
        gui.Cities[game.InitialCityID].GetComponent<City>().addPawn(PlayerRole);
    }

    public override float Act(bool qUndo = false)
    {
        GameObject currentPawn = gui.Pawns[(int)PlayerRole];
        currentPawn.SetActive(true);
        Image currentPawnImage = currentPawn.GetComponent<Image>();
        City initialCity = gui.Cities[game.InitialCityID].GetComponent<City>();
        Vector3 pawnPosition = initialCity.getPawnPosition(PlayerRole);
        Debug.Log(pawnPosition);

        CreateDebugRect(pawnPosition);

        //Sequence sequence = DOTween.Sequence();
        //sequence.Append(currentPawnImage.DOFade(0f, 0f));
        //sequence.Append(currentPawnImage.DOFade(1f, 1f));
        //sequence.Append(currentPawn.transform.DOMove(new Vector3(20,20,0),1f));
        //sequence.Play();
        
        return base.Act(qUndo);
    }

    public void CreateDebugRect(Vector3 position)
    {
        GameObject rectObject = new GameObject("DebugRect");
        rectObject.transform.SetParent(gui.TokenCanvas.transform, false);
        Canvas canvas = gui.TokenCanvas.GetComponent<Canvas>();

        RectTransform rectTransform = rectObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(10,10);
        rectTransform.position = position - new Vector3(canvas.pixelRect.width / 2, canvas.pixelRect.height / 2);

        Image image = rectObject.AddComponent<Image>();
        image.color = Color.white;
    }
}