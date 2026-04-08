using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MemberWorkspace.CHG._02_Scripts
{
    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;
        
        public ViewCastInfo(bool hit, Vector3 point, float dst, float angle)
        {
            this.hit = hit;
            this.point = point;
            this.dst = dst;
            this.angle = angle;
        }
    }

    public struct Edge
    {
        public Vector3 PointA, PointB;

        public Edge(Vector3 pointA, Vector3 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }
    }
    
    public class PlayerVisibility : MonoBehaviour
    {
        public float viewRadius;
        [Range(0,360f)]
        public float viewAngle;
        
        public LayerMask obstacleLayerMask;
        public LayerMask targetLayerMask;
        
        public List<Transform> visibleTargets = new List<Transform>();
        public float meshResolution;

        private Mesh viewMesh;
        public MeshFilter meshFilter;

        public int adgeResolvelterations;
        public float edgeDstThreshold;
        
        private void Start()
        {
            viewMesh = new Mesh();
            viewMesh.name = "View Mesh";
            meshFilter.mesh = viewMesh;
            
            StartCoroutine(FindTargetCoroutineDelay(0.2f));
        }
        private void Update()
        {
            RotateToMouse(); //test
        }

        private void LateUpdate()
        {
            DrawFindOfView();
        }

        private IEnumerator FindTargetCoroutineDelay(float delay)
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

            foreach (Collider collider in colliders)
            {
                Transform target = collider.GetComponent<Transform>();
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
            List<Vector3> viewPoints = new List<Vector3>();
            
            
            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
                
                ViewCastInfo newViewCast =  ViewCast(angle);
                
                viewPoints.Add(newViewCast.point);
            }
            
            int vertexCount = viewPoints.Count +1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount-2) * 3];
            vertices[0] = Vector3.zero;
            
            for (int i = 0; i < vertexCount-1; i++)
            {
                vertices[i +1] = transform.InverseTransformPoint(viewPoints[i]);
                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }
            viewMesh.Clear();
            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles; 
            viewMesh.RecalculateNormals();
        }

        private ViewCastInfo ViewCast(float globalAngle)
        {
            Vector3 dir = DirFromAngle(globalAngle, true);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleLayerMask))
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            else 
                return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
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