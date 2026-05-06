using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts
{

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class FogOfWarOverlay : MonoBehaviour
    {
        [SerializeField] private float overlaySize = 100f;
        [SerializeField] private float y = 0.1f;
        
        [SerializeField, Range(0f, 1f)] private float fogAlpha = 0.85f;
        [SerializeField] private Color fogColor = new Color(0.2f, 0.2f, 0.2f, 1f);

        private MeshFilter _meshFilter;
        private MeshRenderer _renderer;
        private Material _fogMaterial;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _renderer = GetComponent<MeshRenderer>();

            BuildQuadMesh();
            CreateFogMaterial();
        }

        private void BuildQuadMesh()
        {
            float h = overlaySize * 0.5f;
            Mesh mesh = new Mesh { name = "FogOverlay" };

            mesh.vertices = new Vector3[]
            {
                new(-h, 0, -h), new(-h, 0, h),
                new( h, 0, h),  new( h, 0, -h)
            };
            mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
            mesh.uv = new Vector2[]
            {
                Vector2.zero, Vector2.up,
                Vector2.one,  Vector2.right
            };
            mesh.RecalculateNormals();
            _meshFilter.mesh = mesh;
        }

        private void CreateFogMaterial()
        {
            _fogMaterial = new Material(Shader.Find("Custom/FogOverlay"));
            fogColor.a = fogAlpha;
            _fogMaterial.SetColor("_FogColor", fogColor);
            _renderer.material = _fogMaterial;
        }

        private void LateUpdate()
        {
            if (Camera.main != null)
            {
                Vector3 pos = Camera.main.transform.position;
                pos.y = y;
                transform.position = pos;
            }
        }
    }
}