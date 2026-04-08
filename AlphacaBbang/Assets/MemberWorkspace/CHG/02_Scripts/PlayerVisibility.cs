using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MemberWorkspace.CHG._02_Scripts
{
    public class PlayerVisibility : MonoBehaviour
    {
        public float viewRadius;
        [Range(0,360f)]
        public float viewAngle;
        
        public LayerMask obstacleLayerMask;
        public LayerMask targetLayerMask;
        
        public List<Transform> visibleTargets = new List<Transform>();
        public float meshResolution;
        
        private void Start()
        {
            StartCoroutine(FindTargetCoroudineDelay(0.2f));
        }
        private void Update()
        {
            RotateToMouse(); //test
        }

        private void LateUpdate()
        {
            DrawFindOfView();
        }

        private IEnumerator FindTargetCoroudineDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                FindVisibleTarget();
            }
        }

        private void FindVisibleTarget()
        {
            visibleTargets.Clear();
            
            Collider[] colliders = Physics.OverlapSphere(transform.position, viewRadius, targetLayerMask);

            for (int i = 0; i < colliders.Length; i++)
            {
                Transform target = colliders[i].GetComponent<Transform>();
                Vector3 dir = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, dir) < viewAngle / 2)
                {
                    float distance = Vector3.Distance(transform.position, target.transform.position);

                    if (!Physics.Raycast(transform.position, dir, distance, obstacleLayerMask))
                    {
                        visibleTargets.Add(target);
                    }
                }
            }
        }

        public Vector3 DirFromAngle(float angleDegrees, bool angleGlobal)
        {
            if (!angleGlobal)
            {
                angleDegrees += transform.eulerAngles.y;
            }
            
            return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, 
                Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
        }

        private void DrawFindOfView()
        {
            int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
            float stepAngleSize = viewAngle / stepCount;

            for (int i = 0; i < stepCount; i++)
            {
                float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
                Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) 
                    * viewRadius, Color.green);
            }
            
        }
       
        

        #region TestCodes

        private void RotateToMouse()
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Vector3 lookPoint = hit.point;
                lookPoint.y = transform.position.y;

                Vector3 dir = (lookPoint - transform.position).normalized;

                if (dir.sqrMagnitude < 0.001f)
                    return;

                Quaternion targetRotation = Quaternion.LookRotation(dir);

                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    targetRotation,
                    10f * Time.deltaTime);
            }
        }

        #endregion

    }
}