using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MemberWorkspace.CHG._02_Scripts.PlayerView
{
    public class FogRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Settings
        {
            public Material fogMaterial;
            public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public Settings settings = new Settings();
        private FogRenderPass fogPass;

        public override void Create()
        {
            fogPass = new FogRenderPass(settings.fogMaterial, settings.passEvent);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.fogMaterial == null) return;

            var cameraType = renderingData.cameraData.cameraType;

            // Scene뷰, Preview, MaskCamera 전부 제외
            // Main Camera 태그만 통과
            if (cameraType != CameraType.Game) return;
            if (!renderingData.cameraData.camera.CompareTag("MainCamera")) return;

            renderer.EnqueuePass(fogPass);
        }
    }
}