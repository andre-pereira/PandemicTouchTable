public class GActionButtonClicked : GuiEvent
{
    private ActionTypes _actionSelected;

    public GActionButtonClicked(ActionTypes actionSelected)
    {
        _actionSelected = actionSelected;
    }
    
    public override void Do(Timeline timeline)
    {
        
    }
    
    public override string GetLogInfo()
    {
        return $@"""actionSelected"" : ""{_actionSelected}""
                ";
    }
}
