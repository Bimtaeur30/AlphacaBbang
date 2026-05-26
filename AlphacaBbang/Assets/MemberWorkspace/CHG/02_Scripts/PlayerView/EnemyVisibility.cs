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
        }

        public void Hide()
        {
            if (renderers == null) return; 
            
            foreach (var r in renderers)
            {
                if (r != null) 
                    r.enabled = false;
            }
        }
    }
}