using Newtonsoft.Json;
using System;
using static AnimationTemplates;

[System.Serializable]
public abstract class TimelineEvent
{
    public float ANIMATIONDURATION = 1f / GameGUI.gui.AnimationTimingMultiplier;

    // These default values are great for Initialize and AddPlayer Events.
    [Flags]
    public enum Attribute
    {
        None = 0,
        Undoable = 1,
        ContinueUndo = 2
    }
    [JsonIgnore]
    public bool QUndoable
    {
        get { return (Flags & Attribute.Undoable) == Attribute.Undoable; }
        set { if (value) Flags |= Attribute.Undoable; else Flags &= ~Attribute.Undoable; }
    }
    [JsonIgnore]
    public bool QContinueUndo
    {
        get { return (Flags & Attribute.ContinueUndo) == Attribute.ContinueUndo; }
        set { if (value) Flags |= Attribute.ContinueUndo; else Flags &= ~Attribute.ContinueUndo; }
    }

    public Attribute Flags = Attribute.None;
    abstract public void Do(Timeline timeline);
    public virtual float Act(bool qUndo = false) { return 0; }
}

[System.Serializable]
public abstract class EngineEvent : TimelineEvent
{
  public EngineEvent()
  {
    QUndoable = true;
    QContinueUndo = true;
  }
}

[System.Serializable]
public abstract class PlayerEvent : TimelineEvent
{
    public GameGUI gameGUI = GameGUI.gui;
    public Game game = Game.theGame;
    public int PlayerPosition = -1;

    [JsonIgnore] protected Player _player { get { return PlayerList.playerAtPosition(PlayerPosition); } }
    [JsonIgnore] protected PlayerGUI _playerGui { get { return GameGUI.playerPadForPosition(PlayerPosition); } }
    public PlayerEvent() { }
    public PlayerEvent(Player player)
    {
        PlayerPosition = player != null ? player.Position : -1;
        QUndoable = true;
        QContinueUndo = false;
    }
}

[System.Serializable]
public class EDelay : EngineEvent
{
  float myDelay;

  public EDelay(float delay)
  {
    myDelay = delay;
  }

  public override void Do(Timeline timeline) { }
  public override float Act(bool qUndo) { return qUndo ? 0 : myDelay; }
}


