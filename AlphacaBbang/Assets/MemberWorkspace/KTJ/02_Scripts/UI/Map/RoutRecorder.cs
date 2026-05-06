using JJH._02_Scripts_Systems.EventSystems;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActionInfo
{
    public string Action { get; private set; }
    public ActionInfo(string action)
    {
        Action = action;
    }
}

public class RoutRecorder : MonoBehaviour
{
    [SerializeField] private EventChannelSO mapEventChannel;
    public Queue<(Vector3 point, List<ActionInfo> actions)> Routs { get; private set; } = new Queue<(Vector3 point, List<ActionInfo> actions)>();
    private List<ActionInfo> currentActions = new List<ActionInfo>();

    private void Awake()
    {
        Debug.Assert(mapEventChannel != null, "RoutRecorder: MapEventChannel is not assigned in the inspector.");
        mapEventChannel.AddListener<PlayerPointEvent>(OnPlayerPointEvent);
        mapEventChannel.AddListener<PlayerActionEvent>(OnPlayerActionEvent);
    }

    private void OnPlayerActionEvent(PlayerActionEvent @event)
    {
        currentActions.Add(new ActionInfo(@event.Action));
    }

    private void OnPlayerPointEvent(PlayerPointEvent @event)
    {
        Routs.Enqueue((@event.PlayerPoint, currentActions));
        currentActions = new List<ActionInfo>(); // √ ±‚»≠
    }
}
