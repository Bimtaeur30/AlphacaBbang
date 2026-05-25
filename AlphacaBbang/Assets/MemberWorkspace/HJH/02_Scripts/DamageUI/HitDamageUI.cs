using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts.Systems.ObjectPoolSystems;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitDamageUI : MonoBehaviour
{
    [SerializeField] private PoolManagerSO poolManager;
    [SerializeField] private PoolItemSO damagetext;
    [SerializeField] private EventChannelSO agentEventChannel;
    [SerializeField] private Transform spawnPos;

    private Agent Agent;

    private void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            ShowDamage(Random.Range(10f, 999f));
        }
    }
    private void OnEnable()
    {
        Agent = GetComponentInParent<Agent>();
        agentEventChannel.AddListener<AgentHealthChangeEvent>(OnDamageReceived);
    }

    private void OnDisable()
    {
        agentEventChannel.RemoveListener<AgentHealthChangeEvent>(OnDamageReceived);
    }

    private void OnDamageReceived(AgentHealthChangeEvent e)
    {
        if (e.Agent == Agent)
            ShowDamage(e.Damage);
    }

    private void ShowDamage(float damage)
    {
        DamageTextItem item = poolManager.Pop<DamageTextItem>(damagetext);
        item.Play(damage, spawnPos.position);
    }
}