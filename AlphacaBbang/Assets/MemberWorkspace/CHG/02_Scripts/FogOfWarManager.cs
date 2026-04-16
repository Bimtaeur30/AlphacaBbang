using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

namespace MemberWorkspace.CHG._02_Scripts
{
    public class FogOfWarManager : MonoBehaviour
    {
        /*[Header("마스크 카메라 (별도 카메라)")]
        [SerializeField] private Camera maskCamera;

        [Header("안개 설정")]
        [SerializeField] private Color fogColor = new Color(0f, 0f, 0f, 0.7f);
        [SerializeField] private Material fogMaterial;

        public static RenderTexture MaskTexture { get; private set; }

        private void Start()
        {
            // depth 24bit + Stencil 포함으로 생성
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
        }*/
    }
}