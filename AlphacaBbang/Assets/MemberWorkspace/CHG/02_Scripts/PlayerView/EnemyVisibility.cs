using JJH._02_Scripts.Agents;
using JJH._02_Scripts.Agents.Enemies;
using JJH._02_Scripts.Systems.EventSystems;
using JJH._02_Scripts_Systems.EventSystems;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts.PlayerView
{
    public class EnemyVisibility : MonoBehaviour, IVisible
    {
        private AbstractEnemy enemy;
        private Renderer[] renderers;
        private EventChannelSO agentEventChannel;

        private bool isDead;

        private void Awake()
        {
            enemy = GetComponent<AbstractEnemy>();
            CacheRenderers();

            ApplyRenderers(false);
        }

        private void OnEnable()
        {
            isDead = false;
            ApplyRenderers(false);
        }

        private void Start()
        {
            CacheRenderers();

            if (enemy != null)
            {
                agentEventChannel = enemy.AgentEventChannel;
                if (agentEventChannel != null)
                    agentEventChannel.AddListener<AgentDeadEvent>(HandleAgentDeadEvent);
            }

            Hide();
        }

        private void OnDestroy()
        {
            if (agentEventChannel != null)
                agentEventChannel.RemoveListener<AgentDeadEvent>(HandleAgentDeadEvent);
        }

        public void Show() => SetVisible(true);

        public void Hide() => SetVisible(false);

        public void RefreshRenderers()
        {
            CacheRenderers();
        }

        private void SetVisible(bool visible)
        {
            ApplyRenderers(visible);

            if (enemy == null) return;

            if (isDead) return;

            IHealth healthModule = enemy.HealthModule;
            if (healthModule == null) return;

            healthModule.SetHealthBarVisible(visible);
        }

        private void ApplyRenderers(bool visible)
        {
            if (renderers == null) return;

            foreach (var r in renderers)
            {
                if (r != null)
                    r.enabled = visible;
            }
        }

        private void CacheRenderers()
        {
            renderers = GetComponentsInChildren<Renderer>(true);
        }

        private void HandleAgentDeadEvent(AgentDeadEvent evt)
        {
            if (evt.Agent != enemy) return;

            isDead = true;
        }
    }
}
