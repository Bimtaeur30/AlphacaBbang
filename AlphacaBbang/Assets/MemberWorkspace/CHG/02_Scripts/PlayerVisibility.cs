using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MemberWorkspace.CHG._02_Scripts
{
    //raycast hit Info
    public struct ViewCastInfo
    {
        public bool hit; //is hit?
        public Vector3 point; //hit point
        public float dst; // distance
        public float angle; // shout angle
        
        public ViewCastInfo(bool hit, Vector3 point, float dst, float angle)
        {
            this.hit = hit;
            this.point = point;
            this.dst = dst;
            this.angle = angle;
        }
    }

    //find obstacle edge point
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
        [SerializeField] private float viewRadius;
        [Range(0,360f)]
        [SerializeField] private float viewAngle;
        
        [SerializeField] private LayerMask obstacleLayerMask;
        //[SerializeField] private LayerMask targetLayerMask;
        
        //public List<Transform> visibleTargets = new List<Transform>();
        [SerializeField] private float meshResolution; //1 angle in ray count
        [SerializeField] private MeshFilter meshFilter;

        private Mesh viewMesh;  

        
        public int edgeResolveIterations; //
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
                //FindVisibleTarget();
            }
        }
        
        /*private void FindVisibleTarget()
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
        }*/

        

        private void DrawFindOfView()
        {
            int stepCount = Mathf.RoundToInt(viewAngle * meshResolution); // angle in ray shout count
            float stepAngleSize = viewAngle / stepCount; 
            List<Vector3> viewPoints = new List<Vector3>();
            ViewCastInfo prevViewCast = new ViewCastInfo();
            
            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i; //ray shout angle
                
                ViewCastInfo newViewCast =  ViewCast(angle); // ray Result

                if (i != 0)
                {
                    // new ray and old ray distance > edgeDstThreshold = find edge
                    bool edgeDstThresholdExceed = Mathf.Abs(prevViewCast.dst - newViewCast.dst) > edgeDstThreshold;

                    // find edge condition
                    if (prevViewCast.hit != newViewCast.hit ||
                        (prevViewCast.hit && newViewCast.hit && edgeDstThresholdExceed))
                    {
                        //find exact edge, to one-half search
                        Edge e = FindEdge(prevViewCast, newViewCast);

                        if (e.PointA != Vector3.zero)
                            viewPoints.Add(e.PointA);

                        if (e.PointB != Vector3.zero)
                            viewPoints.Add(e.PointB);
                    }
                }
                
                viewPoints.Add(newViewCast.point);
                prevViewCast = newViewCast;
            }
            
            
            int vertexCount = viewPoints.Count +1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount-2) * 3];
            Vector2[] uvs = new Vector2[vertexCount]; 

            vertices[0] = Vector3.zero;
            uvs[0] = new Vector2(0.5f, 0.5f);

            for (int i = 0; i < vertexCount-1; i++)
            {
                vertices[i +1] = transform.InverseTransformPoint(viewPoints[i]);
                
                float magnitude = vertices[i + 1].magnitude;
                Vector2 uvDir = new Vector2(vertices[i + 1].x, vertices[i + 1].z).normalized;
                uvs[i + 1] = new Vector2(0.5f, 0.5f) + uvDir * (magnitude / viewRadius) * 0.5f;

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
            viewMesh.uv = uvs; // 계산한 UV 적용
            viewMesh.RecalculateNormals();
        }
        
        Edge FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            float minAngle = minViewCast.angle;
            float maxAngle = maxViewCast.angle;
            Vector3 minPoint = Vector3.zero;
            Vector3 maxPoint = Vector3.zero;

            for (int i = 0; i < edgeResolveIterations; i++)
            {
                float angle = minAngle + (maxAngle - minAngle) / 2;
                ViewCastInfo newViewCast = ViewCast(angle);
                bool edgeDstThresholdExceed = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (edgeDstThresholdExceed)
                {
                    minAngle = angle;
                    maxPoint = newViewCast.point;
                }
                else
                {
                    maxAngle = angle;
                    minPoint = newViewCast.point;
                }
            }
            
            return new Edge(minPoint, maxPoint);
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
        
        private Vector3 DirFromAngle(float angleDegrees, bool angleGlobal)
        {
            if (!angleGlobal)
            {
                angleDegrees += transform.eulerAngles.y;
            }
            
            return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, 
                Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
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