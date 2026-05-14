using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public static class SystemEvents
{
    public static readonly SavePrefEvent SavePrefEvent = new SavePrefEvent();
    public static readonly LoadPrefEvent LoadPrefEvent = new LoadPrefEvent();
    public static readonly OpenMenuEvent OpenMenuEvent = new OpenMenuEvent();
    public static readonly SaveFileEvent SaveFileEvent = new SaveFileEvent();
    public static readonly LoadFileEvent LoadFileEvent = new LoadFileEvent();
    public static readonly StartNewGameEvent StartNewGameEvent = new StartNewGameEvent();
}

public class SavePrefEvent : GameEvent { }

public class LoadPrefEvent : GameEvent { }

public class SaveFileEvent : GameEvent { }

public class LoadFileEvent : GameEvent
{ }

public class StartNewGameEvent : GameEvent
{ }

public class OpenMenuEvent : GameEvent
{
    public int uiTypeHash;

    public OpenMenuEvent Init(int uiHash)
    {
        uiTypeHash = uiHash;
        return this;
    }
}
