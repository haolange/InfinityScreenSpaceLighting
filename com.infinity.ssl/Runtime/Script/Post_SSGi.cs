using System;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Rendering;
using InfinityExtend.Rendering.Runtime;
using UnityEngine.Rendering.PostProcessing;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;

[Serializable]
[PostProcess(typeof(SSGIRender), PostProcessEvent.BeforeTransparent, "InfinityRender/ScreenSpaceGlobalIllumination")]
public class ScreenSpaceGlobalIllumination : PostProcessEffectSettings
{
    [Header("TraceProperty")]
    [Range(1, 16)] 
    public IntParameter NumRays = new IntParameter(){value = 10};

    [Range(8, 32)] 
    public IntParameter NumSteps = new IntParameter(){value = 8};

    [Range(0.05f, 5.0f)] 
    public FloatParameter Thickness = new FloatParameter(){value = 0.1f};

    [Range(1, 5)]
    public FloatParameter Intensity = new FloatParameter() { value = 1 };

    /////////////////////////////////////////////////////////////////////////////////////////////
    [Header("FilterProperty")]
    [Range(1, 4)]
    public IntParameter NumSpatial = new IntParameter() { value = 1 };

    [Range(1, 2)]
    public FloatParameter SpatialRadius = new FloatParameter() { value = 2 };

    [Range(0, 8)]
    public FloatParameter TemporalScale = new FloatParameter() { value = 1.25f };

    [Range(0, 0.99f)]
    public FloatParameter TemporalWeight = new FloatParameter() { value = 0.99f };

    [Range(0, 2)]
    public IntParameter NumBilateral = new IntParameter() { value = 2 };

    [Range(0.1f, 1)]
    public FloatParameter BilateralColorWeight = new FloatParameter() { value = 1 };

    [Range(0.1f, 1)]
    public FloatParameter BilateralDepthWeight = new FloatParameter() { value = 1 };

    [Range(0.1f, 1)]
    public FloatParameter BilateralNormalWeight = new FloatParameter() { value = 0.1f };


    /////////////////////////////////////////////////////////////////////////////////////////////
    public override bool IsEnabledAndSupported(PostProcessRenderContext context) {
			return enabled
			       && context.camera.actualRenderingPath == RenderingPath.DeferredShading
			       && SystemInfo.supportsMotionVectors
			       && SystemInfo.supportsComputeShaders
			       && SystemInfo.copyTextureSupport > CopyTextureSupport.None;
    }

}

public class SSGIRender : PostProcessEffectRenderer<ScreenSpaceGlobalIllumination>
{
    private int[] ColorPyramidMipIDs, DepthPyramidMipIDs;
    private SSGiParameterDescriptor SSGiParameter;
    private SSGiInputDescriptor SSGiData;
    private SVGFParameterDescriptor SVGFParamete;
    private SVGFInputDescriptor SVGFInputData;

    private int2 PrevScreenSize;
    private RenderTexture RTV_TemporalPrev;

    public override void Init() {
        PyramidDepthGenerator.DepthPyramidInit(ref DepthPyramidMipIDs);
        SSGiData.FrameIndex = 0;
        SVGFInputData.FrameIndex = 0;
    }

    public override void Render(PostProcessRenderContext RenderContent) {
        RenderContent.command.BeginSample("ScreenSpaceGlobalIllumination");

        int2 HZBSize = new int2(1024, 1024);
        int2 ScreenSize = new int2(RenderContent.camera.pixelWidth, RenderContent.camera.pixelHeight);
        Matrix4x4 ProjectionMatrix = GL.GetGPUProjectionMatrix(RenderContent.camera.projectionMatrix, false);
        Matrix4x4 WorldToViewMatrix = RenderContent.camera.worldToCameraMatrix;
        Matrix4x4 ViewProjectionMatrix = ProjectionMatrix * WorldToViewMatrix;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        {
            SSGiParameter.RayMask = false;
            SSGiParameter.NumRays = settings.NumRays;
            SSGiParameter.NumSteps = settings.NumSteps;
            SSGiParameter.Thickness = settings.Thickness;
            SSGiParameter.Intensity = settings.Intensity;

            SSGiData.TraceResolution = new float4(ScreenSize.x, ScreenSize.y, 1.0f / ScreenSize.x, 1.0f / ScreenSize.y);
            SSGiData.FrameIndex += 1;
            SSGiData.Matrix_Proj = ProjectionMatrix;
            SSGiData.Matrix_InvProj = ProjectionMatrix.inverse;
            SSGiData.Matrix_ViewProj = ViewProjectionMatrix;
            SSGiData.Matrix_InvViewProj = ViewProjectionMatrix.inverse;
            SSGiData.Matrix_WorldToView = WorldToViewMatrix;
            SSGiData.SRV_PyramidColor = RenderContent.source;
            SSGiData.SRV_SceneDepth = BuiltinRenderTextureType.ResolvedDepth;
            SSGiData.SRV_GBufferNormal = BuiltinRenderTextureType.GBuffer2;
        }

        {
            SVGFParamete.NumSpatial = settings.NumSpatial;
            SVGFParamete.SpatialRadius = settings.SpatialRadius;
            SVGFParamete.TemporalScale = settings.TemporalScale;
            SVGFParamete.TemporalWeight = settings.TemporalWeight;
            ////////////////////////////////////////////////////////////////
            SVGFInputData.FrameIndex += 1;
            SVGFInputData.Resolution = new float4(ScreenSize.x, ScreenSize.y, 1.0f / ScreenSize.x, 1.0f / ScreenSize.y);
            SVGFInputData.Matrix_InvProj = ProjectionMatrix.inverse;
            SVGFInputData.Matrix_ViewProj = ViewProjectionMatrix;
            SVGFInputData.Matrix_InvViewProj = ViewProjectionMatrix.inverse;
            SVGFInputData.Matrix_WorldToView = WorldToViewMatrix;
            SVGFInputData.SRV_GBufferMotion = BuiltinRenderTextureType.MotionVectors;
            SVGFInputData.SRV_SceneDepth = BuiltinRenderTextureType.ResolvedDepth;
            SVGFInputData.SRV_GBufferNormal = BuiltinRenderTextureType.GBuffer2;
            SVGFInputData.SRV_GBufferRoughness = BuiltinRenderTextureType.GBuffer1;
        }

        //////Set DepthPyramid Data
        RenderContent.command.BeginSample("Depth Pyramid");
        RenderTextureDescriptor PyramidDepthDesc = new RenderTextureDescriptor(HZBSize.x, HZBSize.y, RenderTextureFormat.RHalf, 0) {
            useMipMap = true,
            autoGenerateMips = false,
        };
        RenderContent.command.GetTemporaryRT(SSGiShaderID.SRV_PyramidDepth, PyramidDepthDesc, FilterMode.Point);
        SSGiData.SRV_PyramidDepth = new RenderTargetIdentifier(SSGiShaderID.SRV_PyramidDepth);
        RenderContent.command.BlitFullscreenTriangle(BuiltinRenderTextureType.ResolvedDepth, SSGiData.SRV_PyramidDepth);
        PyramidDepthGenerator.DepthPyramidUpdate(ref DepthPyramidMipIDs, ref HZBSize, SSGiData.SRV_PyramidDepth, RenderContent.command);
        RenderContent.command.EndSample("Depth Pyramid");

        RenderContent.command.BeginSample("Gi Raytrace");
        RenderContent.command.GetTemporaryRT(SSGiShaderID.UAV_ScreenIrradiance, (int)SSGiData.TraceResolution.x, (int)SSGiData.TraceResolution.y, 0, FilterMode.Point, RenderTextureFormat.RGB111110Float, RenderTextureReadWrite.Default, 1, true);
        RenderTargetIdentifier ScreenIrradiance = new RenderTargetIdentifier(SSGiShaderID.UAV_ScreenIrradiance);
        SSGi.Render(RenderContent.command, ScreenIrradiance, ref SSGiParameter, ref SSGiData);
        RenderContent.command.EndSample("Gi Raytrace");

        RenderContent.command.BeginSample("Reflection Blit");
        RenderContent.command.BlitFullscreenTriangle(ScreenIrradiance, RenderContent.destination);
        RenderContent.command.ReleaseTemporaryRT(SSGiShaderID.SRV_PyramidDepth);
        RenderContent.command.ReleaseTemporaryRT(SSGiShaderID.UAV_ScreenIrradiance);
        //RenderContent.command.ReleaseTemporaryRT(SVGF_SpatialShaderID.UAV_SpatialColor);
        //RenderContent.command.ReleaseTemporaryRT(SVGF_TemporalShaderID.UAV_TemporalColor);
        SVGFInputData.Matrix_PrevViewProj = ViewProjectionMatrix;
        RenderContent.command.EndSample("Reflection Blit");

        RenderContent.command.EndSample("ScreenSpaceReflection");
    }

    public override void Release() {
        RenderTexture.ReleaseTemporary(RTV_TemporalPrev);
    }
}