using System;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts.PlayerView
{
    public class EnemyVisibility : MonoBehaviour, IVisible
    {
        private Renderer[] renderers;

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            Hide();
        }

        public void Show()
        {
            foreach (var r in renderers)
            {
                r.enabled = true;
            }
        }

        public void Hide()
        {
            foreach (var r in renderers)
            {
                r.enabled = false;
            }
        }
    }
}