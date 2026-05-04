using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public static class MapEvents
{
    public static readonly PlayerPointEvent PlayerPointEvent = new PlayerPointEvent();
    public static readonly PlayerActionEvent PlayerActionEvent = new PlayerActionEvent();
    public static readonly RoutRecordEndEvent RoutRecordEndEvent = new RoutRecordEndEvent();
    public static readonly RoutRecordStartEvent RoutRecordStartEvent = new RoutRecordStartEvent();
}

public class PlayerPointEvent : GameEvent
{
    public Vector3 PlayerPoint { get; private set; }
    public PlayerPointEvent Init(Vector3 playerPoint)
    {
        PlayerPoint = playerPoint;
        return this;
    }
}

public class PlayerActionEvent : GameEvent
{
    public string Action { get; private set; }
    public PlayerActionEvent Init(string action)
    {
        Action = action;
        return this;
    }
}

public class RoutRecordEndEvent : GameEvent
{
    public float RecordTime { get; private set; }
    public RoutRecordEndEvent Init(float recordTime)
    {
        RecordTime = recordTime;
        return this;
    }
}

public class RoutRecordStartEvent : GameEvent
{
}
