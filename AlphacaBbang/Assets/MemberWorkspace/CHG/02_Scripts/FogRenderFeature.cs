using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MemberWorkspace.CHG._02_Scripts
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
            renderer.EnqueuePass(fogPass);
        }
    }
}