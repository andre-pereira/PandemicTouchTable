﻿using System.Collections.Generic;
using System.Linq;
using static Game;

internal class PResourcePlanning : PlayerEvent
{
    
    private List<int> topPlayerCards;

    public PResourcePlanning(Player playerModel) : base(playerModel){}

    public override float Act(bool qUndo = false)
    {
        _playerGui.ClearSelectedAction();
        _playerGui.draw();
        gameGUI.draw();
        return 0;
    }

    public override void Do(Timeline timeline)
    {
        topPlayerCards = _playerGui.ResourcePlanningEventCardsIDs.ToList();
        _playerGui.ResourcePlanningEventCardsIDs.Reverse();
        foreach (var item in _playerGui.ResourcePlanningEventCardsIDs)
        {
            theGame.PlayerCards.Add(item);
        }
        _playerGui.ResourcePlanningEventCardsCities[0].transform.parent.parent.gameObject.SetActive(false);
        _playerGui.ResourcePlanningEventCardsIDs.Clear();
        _playerGui.ResourcePlanningEventCardSelected = -1;
        _playerGui.ChangeToInEvent(EventState.NOTINEVENT, false);
    }
    
    public override string GetLogInfo()
    {
        string cardIds = string.Join(", ", topPlayerCards);
        return $@" ""topPlayerCards"" : [{cardIds}]
                ";
    }
}