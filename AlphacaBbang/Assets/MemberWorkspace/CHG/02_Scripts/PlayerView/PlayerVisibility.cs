using System.Collections;
using System.Collections.Generic;
using MemberWorkspace.CHG._02_Scripts.PlayerView;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MemberWorkspace.CHG._02_Scripts
{
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
        [Header("View")]
        [SerializeField] private float viewRadius = 10f;
        [SerializeField] private float closeViewRadius = 3f;
        [Range(0, 360f)]
        [SerializeField] private float viewAngle = 90f;
        

        [Header("layer")]
        [SerializeField] private LayerMask obstacleLayerMask;

        [Header("Mesh")]
        public float meshResolution = 5f; //1 angle in ray count
        public int edgeResolveIterations = 4;
        public float edgeDstThreshold = 0.5f;
        
        [SerializeField] private MeshFilter coneMeshFilter;        // 3D cone
        [SerializeField] private MeshFilter flatConeMeshFilter;    // Flat cone
        [SerializeField] private MeshFilter circleMeshFilter;      // 3D circle
        [SerializeField] private MeshFilter flatCircleMeshFilter;  // Flat circle
        [SerializeField] private int circleSegments = 36;

        [Header("EnemyVisible")] 
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private float checkDelay;
        
        private Mesh viewMesh;
        private Mesh flatViewMesh;
        private Mesh circleMesh;
        private Mesh flatCircleMesh;
        private Collider[] _overlapResults = new Collider[50];
        private readonly HashSet<EnemyVisibility> _previousVisible = new();
        
        public float ViewRadius => viewRadius;
        public float CloseViewRadius => closeViewRadius;
        public float ViewAngle => viewAngle;

        
        private IEnumerator Start()
        {
            viewMesh = new Mesh { name = "View Cone Mesh 3D" };
            coneMeshFilter.mesh = viewMesh;
            
            flatViewMesh = new Mesh { name = "View Cone Mesh Flat" };
            flatConeMeshFilter.mesh = flatViewMesh;

            circleMesh = new Mesh { name = "Close View Circle Mesh 3D" };
            circleMeshFilter.mesh = circleMesh;
            
            flatCircleMesh = new Mesh { name = "Close View Circle Mesh Flat" };
            flatCircleMeshFilter.mesh = flatCircleMesh;
            
            yield return StartCoroutine(UpdateVisibilityCoroutine());
        }
        
        private IEnumerator UpdateVisibilityCoroutine()
        {
            WaitForSeconds delay = new WaitForSeconds(checkDelay);
            while (true)
            {
                yield return delay;
                
                HashSet<EnemyVisibility> currentVisible = new();
                
                int cnt = Physics.OverlapSphereNonAlloc(
                    transform.position, viewRadius, _overlapResults, enemyLayer
                );
                
                for (int i = 0; i < cnt; i++)
                {
                    if (!_overlapResults[i].TryGetComponent(out EnemyVisibility enemy))
                        continue;

                    if (!IsVisible(_overlapResults[i].transform.position))
                        continue;

                    currentVisible.Add(enemy);

                    if (!_previousVisible.Contains(enemy))
                    {
                        enemy.Show();
                    }
                }

                foreach (var enemy in _previousVisible)
                {
                    if (!currentVisible.Contains(enemy))
                        enemy.Hide();
                }

                _previousVisible.Clear();
                foreach (var enemy in currentVisible)
                    _previousVisible.Add(enemy);
            }
        }

        private void LateUpdate()
        {
            DrawViewCone();
            DrawFlatViewCone();
            DrawCloseCircle();
            DrawFlatCloseCircle();
        }
        
        public bool IsVisible(Vector3 worldPosition)
        {
            Vector3 toTarget = worldPosition - transform.position;
            float distance = toTarget.magnitude;
            
            // close mesh
            if (distance <= closeViewRadius)
            {
                bool blocked = Physics.Raycast(
                    transform.position,
                    toTarget.normalized,
                    distance,
                    obstacleLayerMask
                );
                if (!blocked) return true;
            }
            // con mesh
            if (distance <= ViewRadius)
            {
                float angle = Vector3.Angle(transform.forward, toTarget);
                if (angle <= ViewAngle / 2f)
                {
                    bool blocked = Physics.Raycast(
                        transform.position,
                        toTarget.normalized,
                        distance,
                        obstacleLayerMask
                    );
                    if (!blocked) return true;
                }
            }

            return false;
        }
        
        // Draw 3D cone mesh
        private void DrawViewCone()
        {
            int stepCount = Mathf.RoundToInt(ViewAngle * meshResolution);
            float stepAngleSize = ViewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            ViewCastInfo prevViewCast = new ViewCastInfo();

            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(angle);

                if (i != 0)
                {
                    bool edgeDstThresholdExceed =
                        Mathf.Abs(prevViewCast.dst - newViewCast.dst) > edgeDstThreshold;

                    if (prevViewCast.hit != newViewCast.hit ||
                        (prevViewCast.hit && newViewCast.hit && edgeDstThresholdExceed))
                    {
                        Edge e = FindEdge(prevViewCast, newViewCast);
                        if (e.PointA != Vector3.zero) viewPoints.Add(e.PointA);
                        if (e.PointB != Vector3.zero) viewPoints.Add(e.PointB);
                    }
                }

                viewPoints.Add(newViewCast.point);
                prevViewCast = newViewCast;
            }

            BuildMesh(viewMesh, viewPoints, 3f);
        }
        
        // Draw flat cone mesh
        private void DrawFlatViewCone()
        {
            int stepCount = Mathf.RoundToInt(ViewAngle * meshResolution);
            float stepAngleSize = ViewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            ViewCastInfo prevViewCast = new ViewCastInfo();

            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(angle);

                if (i != 0)
                {
                    bool edgeDstThresholdExceed =
                        Mathf.Abs(prevViewCast.dst - newViewCast.dst) > edgeDstThreshold;

                    if (prevViewCast.hit != newViewCast.hit ||
                        (prevViewCast.hit && newViewCast.hit && edgeDstThresholdExceed))
                    {
                        Edge e = FindEdge(prevViewCast, newViewCast);
                        if (e.PointA != Vector3.zero) viewPoints.Add(e.PointA);
                        if (e.PointB != Vector3.zero) viewPoints.Add(e.PointB);
                    }
                }

                viewPoints.Add(newViewCast.point);
                prevViewCast = newViewCast;
            }

            BuildMesh(flatViewMesh, viewPoints, 0f);
        }
        
        // Draw 3D close circle mesh
        private void DrawCloseCircle()
        {
            List<Vector3> circlePoints = new List<Vector3>();
            float angleStep = 360f / circleSegments;

            for (int i = 0; i <= circleSegments; i++)
            {
                float angle = i * angleStep;
                Vector3 dir = DirFromAngle(angle + transform.eulerAngles.y, true);

                if (Physics.Raycast(transform.position, dir, out RaycastHit hit, closeViewRadius, obstacleLayerMask))
                    circlePoints.Add(hit.point);
                else
                    circlePoints.Add(transform.position + dir * closeViewRadius);
            }
            BuildMesh(circleMesh, circlePoints, 3f);
        }
        
        // Draw flat close circle mesh
        private void DrawFlatCloseCircle()
        {
            List<Vector3> circlePoints = new List<Vector3>();
            float angleStep = 360f / circleSegments;

            for (int i = 0; i <= circleSegments; i++)
            {
                float angle = i * angleStep;
                Vector3 dir = DirFromAngle(angle + transform.eulerAngles.y, true);

                if (Physics.Raycast(transform.position, dir, out RaycastHit hit, closeViewRadius, obstacleLayerMask))
                    circlePoints.Add(hit.point);
                else
                    circlePoints.Add(transform.position + dir * closeViewRadius);
            }
            BuildMesh(flatCircleMesh, circlePoints, 0f);
        }
        
        private void BuildMesh(Mesh mesh, List<Vector3> viewPoints, float height)
        {
            int count = viewPoints.Count;
            
            if (height <= 0.01f) // Flat mesh
            {
                Vector3[] vertices = new Vector3[count + 1];
                List<int> triangles = new List<int>();

                vertices[0] = new Vector3(0, height, 0);

                for (int i = 0; i < count; i++)
                {
                    Vector3 p = transform.InverseTransformPoint(viewPoints[i]);
                    vertices[i + 1] = new Vector3(p.x, height, p.z);

                    if (i < count - 1) 
                    {
                        triangles.Add(0);
                        triangles.Add(i + 1);
                        triangles.Add(i + 2);
                    }
                }

                mesh.Clear();
                mesh.vertices = vertices;
                mesh.triangles = triangles.ToArray();
                mesh.RecalculateNormals();
            }
            else // 3D mesh
            {
                int vertexCount = count * 2 + 2;
                Vector3[] vertices = new Vector3[vertexCount];
                List<int> triangles = new List<int>();

                vertices[0] = Vector3.zero;
                vertices[1] = new Vector3(0, height, 0);

                for (int i = 0; i < count; i++)
                {
                    Vector3 p = transform.InverseTransformPoint(viewPoints[i]);
                    vertices[2 + i * 2] = p;
                    vertices[2 + i * 2 + 1] = new Vector3(p.x, height, p.z);

                    if (i < count - 1)
                    {
                        int a = 2 + i * 2;
                        int b = 2 + i * 2 + 1;
                        int c = 2 + (i + 1) * 2;
                        int d = 2 + (i + 1) * 2 + 1;

                        triangles.Add(0); triangles.Add(a); triangles.Add(c);
                        triangles.Add(1); triangles.Add(d); triangles.Add(b);
                        triangles.Add(a); triangles.Add(b); triangles.Add(c);
                        triangles.Add(b); triangles.Add(d); triangles.Add(c);
                    }
                }

                mesh.Clear();
                mesh.vertices = vertices;
                mesh.triangles = triangles.ToArray();
                mesh.RecalculateNormals();
            }
        }
        
        private Edge FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            float minAngle = minViewCast.angle;
            float maxAngle = maxViewCast.angle;
            Vector3 minPoint = Vector3.zero;
            Vector3 maxPoint = Vector3.zero;

            for (int i = 0; i < edgeResolveIterations; i++)
            {
                float angle = (minAngle + maxAngle) / 2f;
                ViewCastInfo newViewCast = ViewCast(angle);

                bool edgeDstThresholdExceed =
                    Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;

                if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceed)
                {
                    minAngle = angle;
                    minPoint = newViewCast.point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.point;
                }
            }

            return new Edge(minPoint, maxPoint);
        }

        private ViewCastInfo ViewCast(float globalAngle)
        {
            Vector3 dir = DirFromAngle(globalAngle, true);

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, ViewRadius, obstacleLayerMask))
            {
                Vector3 toHit = (hit.point - transform.position).normalized;
                Vector3 adjustedPoint = hit.point - toHit * 0.1f;
        
                return new ViewCastInfo(true, adjustedPoint, hit.distance - 0.1f, globalAngle);
            }
            else
            {
                return new ViewCastInfo(false, transform.position + dir * ViewRadius, ViewRadius, globalAngle);
            }
        }

        public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal) angleDegrees += transform.eulerAngles.y;
            return new Vector3(
                Mathf.Cos((-angleDegrees + 90f) * Mathf.Deg2Rad),
                0,
                Mathf.Sin((-angleDegrees + 90f) * Mathf.Deg2Rad)
            );
        }
        
        
    }
}