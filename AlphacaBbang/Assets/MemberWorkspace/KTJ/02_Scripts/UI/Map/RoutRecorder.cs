using JJH._02_Scripts_Systems.EventSystems;
using System.Collections.Generic;
using UnityEngine;

public class ActionInfo
{
    public string Action { get; private set; }
    public ActionInfo(string action) { Action = action; }
}

public class RoutRecorder : MonoBehaviour
{
    [SerializeField] private EventChannelSO mapEventChannel;

    public List<(Vector3 point, List<ActionInfo> actions)> Routs { get; private set; }
        = new List<(Vector3, List<ActionInfo>)>();

    private List<ActionInfo> _currentActions = new List<ActionInfo>();

    private void Awake()
    {
        Debug.Assert(mapEventChannel != null, "RoutRecorder: MapEventChannel is not assigned in the inspector.");
        mapEventChannel.AddListener<PlayerPointEvent>(OnPlayerPointEvent);
        mapEventChannel.AddListener<PlayerActionEvent>(OnPlayerActionEvent);
    }

    private void OnDestroy()
    {
        mapEventChannel.RemoveListener<PlayerPointEvent>(OnPlayerPointEvent);
        mapEventChannel.RemoveListener<PlayerActionEvent>(OnPlayerActionEvent);
    }

    // 외부에서 명시적으로 호출해서 초기화
    public void Clear()
    {
        Routs.Clear();
        _currentActions = new List<ActionInfo>();
        //Debug.Log($"[RoutRecorder] Clear 호출 → Routs 초기화 완료");
    }

    private void OnPlayerActionEvent(PlayerActionEvent @event)
    {
        _currentActions.Add(new ActionInfo(@event.Action));
        //Debug.Log($"[RoutRecorder] 액션 추가: '{@event.Action}' | 현재 누적 액션 수: {_currentActions.Count}");
    }

    private void OnPlayerPointEvent(PlayerPointEvent @event)
    {
        Routs.Add((@event.PlayerPoint, _currentActions));
        //Debug.Log($"[RoutRecorder] 포인트 추가: {_currentActions.Count}개 액션 포함 | 총 Routs 수: {Routs.Count}");
        _currentActions = new List<ActionInfo>();
    }
}