using System.Collections;
using System.Collections.Generic;
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
        
        [SerializeField] private MeshFilter coneMeshFilter;
        [SerializeField] private MeshFilter circleMeshFilter;
        [SerializeField] private int circleSegments = 36;

        private Mesh viewMesh;
        private Mesh circleMesh;

        public float ViewRadius => viewRadius;
        public float ViewAngle => viewAngle;
        public float CloseViewRadius => closeViewRadius;

        private void Start()
        {
            viewMesh = new Mesh { name = "View Cone Mesh" };
            coneMeshFilter.mesh = viewMesh;

            circleMesh = new Mesh { name = "Close View Circle Mesh" };
            circleMeshFilter.mesh = circleMesh;
        }

        private void Update()
        {
            RotateToMouse();
        }

        private void LateUpdate()
        {
            DrawViewCone();
            DrawCloseCircle();
        }
        
        public bool IsVisible(Vector3 worldPosition)
        {
            Vector3 toTarget = worldPosition - transform.position;
            float distance = toTarget.magnitude;

            //closeView
            if (distance <= closeViewRadius)
                return true;

            // Con
            if (distance <= viewRadius)
            {
                float angle = Vector3.Angle(transform.forward, toTarget);
                if (angle <= viewAngle / 2f)
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

        // ─────────────────────────────────────────
        // 시야 콘 메쉬
        // ─────────────────────────────────────────
        private void DrawViewCone()
        {
            int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
            float stepAngleSize = viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            ViewCastInfo prevViewCast = new ViewCastInfo();

            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
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

            BuildMesh(viewMesh, viewPoints);
        }

        // ─────────────────────────────────────────
        // 근거리 원형 메쉬
        // ─────────────────────────────────────────
        private void DrawCloseCircle()
        {
            List<Vector3> circlePoints = new List<Vector3>();
            float angleStep = 360f / circleSegments;

            for (int i = 0; i <= circleSegments; i++)
            {
                float angle = i * angleStep;
                Vector3 dir = DirFromAngle(angle + transform.eulerAngles.y, true);
                circlePoints.Add(transform.position + dir * closeViewRadius);
            }

            BuildMesh(circleMesh, circlePoints);
        }

        private void BuildMesh(Mesh mesh, List<Vector3> viewPoints)
        {
            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;

            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        // ─────────────────────────────────────────
        // 헬퍼
        // ─────────────────────────────────────────
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

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, viewRadius, obstacleLayerMask))
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            else
                return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
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

        private void RotateToMouse()
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Vector3 lookPoint = hit.point;
                lookPoint.y = transform.position.y;
                Vector3 dir = (lookPoint - transform.position).normalized;
                if (dir.sqrMagnitude < 0.001f) return;

                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    Quaternion.LookRotation(dir),
                    10f * Time.deltaTime
                );
            }
        }
    }
}