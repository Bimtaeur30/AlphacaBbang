using System;
using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts
{
    public class ObstacleFade : MonoBehaviour
    {
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private Transform target;
        
        private void Update()
        {
            Vector3 direction = (target.position - transform.position).normalized;
            RaycastHit[] hits = Physics.RaycastAll(transform.position, direction,Mathf.Infinity, 
                obstacleLayer | playerLayer);


            for (int i = 0; i < hits.Length; i++)
            {
                
            }
        }

        
    }
}
