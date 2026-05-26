using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MemberWorkspace.CHG._02_Scripts.PlayerView
{
    public class FogRenderFeature : ScriptableRendererFeature
    {
        private static readonly int MaskTexID        = Shader.PropertyToID("_MaskTex");
        private static readonly int PlayerPosID      = Shader.PropertyToID("_PlayerPos");
        private static readonly int PlayerForwardID  = Shader.PropertyToID("_PlayerForward");
        private static readonly int ViewRadiusID     = Shader.PropertyToID("_ViewRadius");
        private static readonly int ViewAngleID      = Shader.PropertyToID("_ViewAngle");
        private static readonly int CloseRadiusID    = Shader.PropertyToID("_CloseViewRadius");

        public override void Create() { }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game) return;
            if (!renderingData.cameraData.camera.CompareTag("MainCamera")) return;
    
            if (FogOfWar.PlayerTransform == null || FogOfWar.PlayerVisibility == null) return; 

            if (FogOfWar.PlayerTransform == null) return;

            Vector3 forward = FogOfWar.PlayerTransform.forward;
            forward.y = 0;
            forward.Normalize();
            
            Shader.SetGlobalVector("_PlayerPos",       FogOfWar.PlayerTransform.position);
            Shader.SetGlobalVector("_PlayerForward",   forward);
            Shader.SetGlobalFloat ("_ViewRadius",      FogOfWar.PlayerVisibility.ViewRadius);
            Shader.SetGlobalFloat ("_ViewAngle",       FogOfWar.PlayerVisibility.ViewAngle);
            Shader.SetGlobalFloat ("_CloseViewRadius", FogOfWar.PlayerVisibility.CloseViewRadius);
        }
    }
}