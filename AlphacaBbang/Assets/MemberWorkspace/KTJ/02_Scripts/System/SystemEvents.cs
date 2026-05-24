using JJH._02_Scripts_Systems.EventSystems;
using MemberWorkspace.CHG._02_Scripts;
using UnityEngine;

public static class SystemEvents
{
    public static readonly SavePrefEvent SavePrefEvent = new SavePrefEvent();
    public static readonly LoadPrefEvent LoadPrefEvent = new LoadPrefEvent();
    public static readonly OpenMenuEvent OpenMenuEvent = new OpenMenuEvent();
    public static readonly SaveFileEvent SaveFileEvent = new SaveFileEvent();
    public static readonly LoadFileEvent LoadFileEvent = new LoadFileEvent();
    public static readonly StartNewGameEvent StartNewGameEvent = new StartNewGameEvent();
    public static readonly OnGameENd OnGameEnd = new OnGameENd();
    public static readonly LootboxDataSendEvent LootboxDataSendEvent = new LootboxDataSendEvent();
    public static readonly SystemNotificationEvent SystemNotificationEvent = new SystemNotificationEvent();
}

public class LootboxDataSendEvent : GameEvent
{
    public LootBox LootBox { get; private set; }
    public LootboxDataSendEvent Init(LootBox lootbox)
    {
        LootBox = lootbox;
        return this;
    }
}
public class OnGameENd : GameEvent
{
    public bool IsPlayerAlive { get; private set; }
    public OnGameENd Init(bool isPlayerAlive)
    {
        IsPlayerAlive = isPlayerAlive;
        return this;
    }
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
