using JJH._02_Scripts.Agents.Enemies;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts.PlayerView
{
    public class EnemyVisibility : MonoBehaviour, IVisible
    {
        private AbstractEnemy enemy;
        private Renderer[] renderers;

        private void Awake()
        {
            enemy = GetComponent<AbstractEnemy>();
            renderers = GetComponentsInChildren<Renderer>();
            Hide();
        }

        private void OnEnable()
        {
            Hide();
        }

        public void Show()
        {
            if (renderers == null) return;

            foreach (var r in renderers)
            {
                if (r != null)
                    r.enabled = true;
            }

            enemy.HealthModule.SetHealthBarVisible(true);
        }

        public void Hide()
        {
            if (renderers == null) return;

            foreach (var r in renderers)
            {
                if (r != null)
                    r.enabled = false;
            }

            enemy.HealthModule.SetHealthBarVisible(false);
        }
    }
}