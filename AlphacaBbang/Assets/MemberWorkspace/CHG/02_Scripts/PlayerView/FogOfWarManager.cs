using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

namespace MemberWorkspace.CHG._02_Scripts
{
    /// <summary>
    /// This is Test Script
    /// </summary>
    public class FogOfWarManager : MonoBehaviour
    {
        [SerializeField] private Camera maskCamera;

        [SerializeField] private Color fogColor = new Color(0f, 0f, 0f, 0.7f);
        [SerializeField] private Material fogMaterial;

        public static RenderTexture MaskTexture { get; private set; }

        private void Start()
        {
            MaskTexture = new RenderTexture(
                Screen.width, Screen.height, 24,
                RenderTextureFormat.ARGB32
            );
            MaskTexture.depthStencilFormat = GraphicsFormat.D24_UNorm_S8_UInt;
            MaskTexture.Create();

            maskCamera.targetTexture    = MaskTexture;
            maskCamera.cullingMask      = LayerMask.GetMask("ViewMesh");
            maskCamera.clearFlags       = CameraClearFlags.SolidColor;
            maskCamera.backgroundColor  = Color.black;

            var camData = maskCamera.GetUniversalAdditionalCameraData();
            camData.renderType           = CameraRenderType.Base;
            camData.renderPostProcessing = false;

            if (fogMaterial != null)
                fogMaterial.SetColor("_FogColor", fogColor);
        }

        private void OnDestroy()
        {
            if (MaskTexture != null)
            {
                MaskTexture.Release();
                MaskTexture = null;
            }
        }
    }
}