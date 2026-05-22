using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.Universal;

namespace MemberWorkspace.CHG._02_Scripts.PlayerView
{
    public class FogOfWar : MonoBehaviour
    {
        [SerializeField] private Camera maskCamera;
        [SerializeField] private Color fogColor = new Color(0.25f, 0.25f, 0.25f, 0.85f);
        [SerializeField] private Material fogMaterial;

        public static RenderTexture MaskTexture      { get; private set; }
        public static Transform PlayerTransform      { get; private set; }
        public static PlayerVisibility PlayerVisibility { get; private set; }

        private Camera _mainCamera;

        private void Awake()
        {

            GameObject player = GameObject.FindWithTag("Player");
            PlayerTransform   = player.transform;
            PlayerVisibility  = player.GetComponent<PlayerVisibility>();

            MaskTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
            MaskTexture.depthStencilFormat = GraphicsFormat.D24_UNorm_S8_UInt;
            MaskTexture.Create();

            maskCamera.targetTexture   = MaskTexture;
            maskCamera.cullingMask     = LayerMask.GetMask("ViewMesh");
            maskCamera.clearFlags      = CameraClearFlags.SolidColor;
            maskCamera.backgroundColor = Color.black;

            var camData = maskCamera.GetUniversalAdditionalCameraData();
            camData.renderPostProcessing = false;
            camData.antialiasing         = AntialiasingMode.None;
            camData.renderType           = CameraRenderType.Base;

            if (fogMaterial != null)
                fogMaterial.SetColor("_FogColor", fogColor);
        }

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            SyncMaskCamera();
            maskCamera.Render();
        }

        private void SyncMaskCamera()
        {
            maskCamera.transform.SetPositionAndRotation(
                _mainCamera.transform.position,
                _mainCamera.transform.rotation
            );
            maskCamera.fieldOfView      = _mainCamera.fieldOfView;
            maskCamera.orthographic     = _mainCamera.orthographic;
            maskCamera.orthographicSize = _mainCamera.orthographicSize;
            maskCamera.nearClipPlane    = _mainCamera.nearClipPlane;
            maskCamera.farClipPlane     = _mainCamera.farClipPlane;
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