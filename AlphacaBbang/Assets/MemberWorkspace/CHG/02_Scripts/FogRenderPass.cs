using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using MemberWorkspace.CHG._02_Scripts;

public class FogRenderPass : ScriptableRenderPass
{
    private Material fogMaterial;
    private static readonly int MaskTexID = Shader.PropertyToID("_MaskTex");

    public FogRenderPass(Material mat, RenderPassEvent passEvent)
    {
        fogMaterial = mat;
        renderPassEvent = passEvent;
        requiresIntermediateTexture = true;
    }

    private class PassData
    {
        public TextureHandle src;
        public TextureHandle dst;
        public Material fogMaterial;
        public RenderTexture maskTexture;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (fogMaterial == null) return;

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

        TextureHandle srcHandle = resourceData.activeColorTexture;

        // 임시 텍스처 생성 (src → tmp → dst 방식)
        var desc = cameraData.cameraTargetDescriptor;
        desc.depthBufferBits = 0;
        desc.colorFormat = RenderTextureFormat.ARGB32;

        TextureHandle tmpHandle = UniversalRenderer.CreateRenderGraphTexture(
            renderGraph, desc, "_FogTempTex", false
        );

        using (var builder = renderGraph.AddUnsafePass<PassData>("FogOfWar", out var passData))
        {
            passData.src         = srcHandle;
            passData.dst         = tmpHandle;
            passData.fogMaterial = fogMaterial;
            passData.maskTexture = FogOfWarManager.MaskTexture;

            builder.UseTexture(srcHandle, AccessFlags.Read);
            builder.UseTexture(tmpHandle, AccessFlags.Write);

            builder.SetRenderFunc((PassData data, UnsafeGraphContext ctx) =>
            {
                if (data.maskTexture != null)
                    data.fogMaterial.SetTexture(MaskTexID, data.maskTexture);

                // src → fogMaterial 적용 → tmp
                Blitter.BlitCameraTexture(ctx.cmd, data.src, data.dst, data.fogMaterial, 0);
                // tmp → src (화면에 최종 출력)
                Blitter.BlitCameraTexture(ctx.cmd, data.dst, data.src);
            });
        }
    }
}