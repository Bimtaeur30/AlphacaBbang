using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using MemberWorkspace.CHG._02_Scripts;

public class FogRenderPass : ScriptableRenderPass
{
    private Material fogMaterial;
    private static readonly int MaskTexID   = Shader.PropertyToID("_MaskTex");
    private static readonly int BlitScaleID = Shader.PropertyToID("_BlitScaleBias");

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
        UniversalCameraData   cameraData   = frameData.Get<UniversalCameraData>();

        TextureHandle srcHandle = resourceData.activeColorTexture;

        var desc = cameraData.cameraTargetDescriptor;
        desc.depthBufferBits = 0;
        desc.msaaSamples = 1;

        TextureHandle tmpHandle = UniversalRenderer.CreateRenderGraphTexture(
            renderGraph, desc, "_FogTempTex", false
        );

        using (var builder = renderGraph.AddRasterRenderPass<PassData>("FogOfWar_Apply", out var passData))
        {
            passData.src         = srcHandle;
            passData.dst         = tmpHandle;
            passData.fogMaterial = fogMaterial;
            passData.maskTexture = FogOfWarManager.MaskTexture;

            builder.UseTexture(srcHandle, AccessFlags.Read);  
            builder.SetRenderAttachment(tmpHandle, 0);   

            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                if (data.maskTexture != null)
                    data.fogMaterial.SetTexture(MaskTexID, data.maskTexture);

                data.fogMaterial.SetVector(BlitScaleID, new Vector4(1, 1, 0, 0));

                Blitter.BlitTexture(ctx.cmd, data.src, new Vector4(1, 1, 0, 0), data.fogMaterial, 0);
            });
        }

        using (var builder = renderGraph.AddRasterRenderPass<PassData>("FogOfWar_Copy", out var passData))
        {
            passData.src = tmpHandle;
            passData.dst = srcHandle;

            builder.UseTexture(tmpHandle, AccessFlags.Read);
            builder.SetRenderAttachment(srcHandle, 0);

            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                Blitter.BlitTexture(ctx.cmd, data.src, new Vector4(1, 1, 0, 0), 0, false);
            });
        }
    }
}