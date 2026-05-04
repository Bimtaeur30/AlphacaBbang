using UnityEngine;

namespace MemberWorkspace.CHG._02_Scripts
{

    [RequireComponent(typeof(MeshRenderer))]
    public class FovStencilWriter : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float visibleAreaAlpha = 0f;

        private void Awake()
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            Color c = Color.white;
            c.a = visibleAreaAlpha;
            mat.color = c;

            mat.SetFloat("_Surface", 1);
            mat.SetFloat("_Blend", 0);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;

            mat.SetInt("_Stencil", 1);
            mat.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.Always);
            mat.SetInt("_StencilOp", (int)UnityEngine.Rendering.StencilOp.Replace);

            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            GetComponent<MeshRenderer>().material = mat;
        }
    }
}