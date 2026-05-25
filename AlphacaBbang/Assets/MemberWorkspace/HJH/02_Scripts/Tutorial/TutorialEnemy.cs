using JJH._02_Scripts.Agents;
using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts_Systems.EventSystems;
using System.Collections;
using UnityEngine;

public class TutorialEnemy : MonoBehaviour
{
    [SerializeField] private Agent agent;
    [SerializeField] private Animator animator;
    private static readonly int HitTrigger = Animator.StringToHash("Hit");

    private EventChannelSO _agentEventChannel;

    private void Start()
    {
        _agentEventChannel = agent.AgentEventChannel;
        _agentEventChannel.AddListener<AgentDeadEvent>(OnAgentDead);
        _agentEventChannel.AddListener<AgentHealthChangeEvent>(OnHealthChanged);
    }

    private void OnDestroy()
    {
        _agentEventChannel.RemoveListener<AgentDeadEvent>(OnAgentDead);
        _agentEventChannel.RemoveListener<AgentHealthChangeEvent>(OnHealthChanged);
    }

    private void OnAgentDead(AgentDeadEvent evt)
    {
        StartCoroutine(RespawnAfterDelay(3f));
    }

    private void OnHealthChanged(AgentHealthChangeEvent evt)
    {
        if (evt.Damage > 0)
            animator.SetTrigger(HitTrigger);
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        var healthModule = agent.HealthModule as AgentHealthModule;
        healthModule.InitHealth(healthModule.MaxHealth);
    }
}
