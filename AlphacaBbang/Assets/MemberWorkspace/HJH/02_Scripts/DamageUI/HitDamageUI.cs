using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Systems.ObjectPoolSystems;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

public class HitDamageUI : MonoBehaviour
{
    [SerializeField] private PoolManagerSO poolManager;
    [SerializeField] private PoolItemSO damagetext;
    [SerializeField] private EventChannelSO agentEventChannel;
    [SerializeField] private Transform spawnPos;

    private Agent Agent;

    private void OnEnable()
    {
        Agent = GetComponentInParent<Agent>();
        agentEventChannel.AddListener<AgentHealthChangeEvent>(OnDamageReceived);
    }

    private void OnDisable()
    {
        agentEventChannel.RemoveListener<AgentHealthChangeEvent>(OnDamageReceived);
    }

    private void OnDamageReceived(AgentHealthChangeEvent evt)
    {
        if (evt.Agent == Agent)
            ShowDamage(evt.Damage);
    }

    private void ShowDamage(float damage)
    {
        DamageTextItem item = poolManager.Pop<DamageTextItem>(damagetext);
        item.Play(damage, spawnPos.position);
    }
}