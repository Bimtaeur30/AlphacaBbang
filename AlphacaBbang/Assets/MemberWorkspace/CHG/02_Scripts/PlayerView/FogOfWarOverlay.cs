using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts
{

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class FogOfWarOverlay : MonoBehaviour
    {
        [Tooltip("맵 전체를 덮을 크기 (맵보다 크게)")]
        [SerializeField] private float overlaySize = 100f;

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

            _fogMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            fogColor.a = fogAlpha;
            _fogMaterial.color = fogColor;

            _fogMaterial.SetFloat("_Surface", 1);          
            _fogMaterial.SetFloat("_Blend", 0);            
            _fogMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _fogMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _fogMaterial.SetInt("_ZWrite", 0);
            _fogMaterial.renderQueue = 3001;           

            _fogMaterial.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.NotEqual);
            _fogMaterial.SetInt("_Stencil", 1);
            _fogMaterial.SetInt("_StencilOp", (int)UnityEngine.Rendering.StencilOp.Keep);

            _fogMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            _renderer.material = _fogMaterial;
        }

        private void LateUpdate()
        {
            if (Camera.main != null)
            {
                Vector3 pos = Camera.main.transform.position;
                pos.y = 0f;
                transform.position = pos;
            }
        }
    }
}