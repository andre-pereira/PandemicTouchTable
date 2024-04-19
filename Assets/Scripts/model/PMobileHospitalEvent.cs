using UnityEngine;
using static ENUMS;
using static Game;

public class PMobileHospitalEvent : PlayerEvent
{
    private City city;
    private VirusName virusName;

    public PMobileHospitalEvent(Player player, City city, ENUMS.VirusName virusName) : base(player)
    {
        this.city = city;
        this.virusName = virusName;

    }

    public override float Act(bool qUndo = false)
    {
        city.draw();
        gameGUI.drawBoard();
        foreach (Player player in PlayerList.getAllPlayers())
        {
            player.playerGui.draw();
        }
        return 0;
    }

    public override void Do(Timeline timeline)
    {
        /*Debug.Log("Player of Mobile Hospital = " + game.CurrentPlayer.Name);
        Debug.Log("In the city :" + game.CurrentPlayer.GetCurrentCity() + " cityID=" + city.city.cityID + " theGame.InEventCard = " + theGame.InEventCard);*/

        if (city.city.cityID == game.CurrentPlayer.GetCurrentCity() && theGame.InEventCard == EventState.EXECUTINGMOBILEHOSPITAL)
        {
            //Debug.Log("In the city :" + game.CurrentPlayer.GetCurrentCity() + " city.getInstanceID=" + city.GetInstanceID());
            city.incrementNumberOfCubes((VirusName)virusName, -1);
            game.incrementNumberOfCubesOnBoard((VirusName)virusName, 1);

            theGame.MobileHospitalPlayer.playerGui.ChangeToInEvent(EventState.NOTINEVENT);
            theGame.ChangeToInEvent(EventState.NOTINEVENT);
            theGame.RemovePlayersWait();
            theGame.CubeClicked(city, virusName);

        }

    }
}