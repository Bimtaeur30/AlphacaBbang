using JJH._02_Scripts.Agents;
using JJH._02_Scripts.Agents.Enemies;
using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Systems.ObjectPoolSystems;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public class TutorialDummySetup : MonoBehaviour, IPoolable
{
    [SerializeField] private EnemyDataSO enemyData;
    [SerializeField] private PoolManagerSO poolManager;
    [SerializeField] private PoolItemSO dummyPoolItem;

    public PoolItemSO PoolType { get => dummyPoolItem; set => dummyPoolItem = value; }
    public GameObject GameObject => gameObject;
    public event System.Action OnDummyDead;

    private Agent _agent;
    private EventChannelSO _agentEventChannel;

    public void ResetItem()
    {
        _agent = GetComponent<Agent>();
        _agent.HealthModule.InitHealth(enemyData.EnemyHealth);
        _agentEventChannel = _agent.AgentEventChannel;
        _agentEventChannel.AddListener<AgentDeadEvent>(OnAgentDead);
    }

    private void OnAgentDead(AgentDeadEvent evt)
    {
        if (evt.Agent != _agent) return;
        _agentEventChannel?.RemoveListener<AgentDeadEvent>(OnAgentDead);
        OnDummyDead?.Invoke();
        poolManager.Push(this);
    }
}