using System;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using InfinityTech.Runtime.Rendering.Feature;
using IntParameter = UnityEngine.Rendering.PostProcessing.IntParameter;
using FloatParameter = UnityEngine.Rendering.PostProcessing.FloatParameter;

[System.Serializable]
public enum PostRenderSize
{
		Full = 1,
		Half = 2
}

[Serializable]
public class SSRRenderMode : ParameterOverride<PostRenderSize> {}

[Serializable]
[PostProcess(typeof(ScreenSpaceReflectionRender), PostProcessEvent.BeforeTransparent, "InfinityRender/ScreenSpaceReflection")]
public class ScreenSpaceReflection : PostProcessEffectSettings
{
    [Header("TraceProperty")]
    public SSRRenderMode RenderMode = new SSRRenderMode() {value = PostRenderSize.Full};

    [Range(1, 12)] 
    public IntParameter NumRays = new IntParameter(){value = 4};

    [Range(8, 64)] 
    public IntParameter NumSteps = new IntParameter(){value = 8};
    
    [Range(0, 1)]
    public FloatParameter BRDFBias = new FloatParameter() { value = 0.7f };

    [Range(0.05f, 0.25f)]
    public FloatParameter Fadeness = new FloatParameter() { value = 0.1f };

    [Range(0, 1)]
    public FloatParameter RoughnessDiscard = new FloatParameter() { value = 0.5f };

/////////////////////////////////////////////////////////////////////////////////////////////
    [Header("FilterProperty")]
    public BoolParameter EnableSpatial = new BoolParameter(){value = true};
    [Range(1, 4)] 
    public IntParameter NumSpatial = new IntParameter(){value = 1};

    [Range(1, 2)] 
    public FloatParameter SpatialRadius = new FloatParameter(){value = 2};

    [Range(0, 8)] 
    public FloatParameter TemporalScale = new FloatParameter(){value = 1.25f};

    [Range(0, 0.99f)] 
    public FloatParameter TemporalWeight = new FloatParameter(){value = 0.99f};

    [Range(0, 2)]
    public IntParameter NumBilateral = new IntParameter() { value = 1 };

    /*[Range(0.1f, 1)]
    public FloatParameter BilateralColorWeight = new FloatParameter() { value = 1 };

    [Range(0.1f, 1)]
    public FloatParameter BilateralDepthWeight = new FloatParameter() { value = 1 };

    [Range(0.1f, 1)]
    public FloatParameter BilateralNormalWeight = new FloatParameter() { value = 0.1f };*/

    /////////////////////////////////////////////////////////////////////////////////////////////
    public override bool IsEnabledAndSupported(PostProcessRenderContext context) {
			return enabled
			       && context.camera.actualRenderingPath == RenderingPath.DeferredShading
			       && SystemInfo.supportsMotionVectors
			       && SystemInfo.supportsComputeShaders
			       && SystemInfo.copyTextureSupport > CopyTextureSupport.None;
    }

}

public class ScreenSpaceReflectionRender : PostProcessEffectRenderer<ScreenSpaceReflection>
{
    private int[] ColorPyramidMipIDs, DepthPyramidMipIDs;
    private SSRParameterDescriptor SSRParameter;
    private SSRInputDescriptor SSRInputData;
    private SVGFParameterDescriptor SVGFParamete;
    private SVGFInputDescriptor SVGFInputData;

    private int2 PrevScreenSize;
    private RenderTexture RTV_PyramidDepth;
    //private Shader CompositingShader;
    private Material CompositingMaterial;

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public override void Init() {
        //CompositingShader = Resources.Load<Shader>("Hidden/SSRCompositing");
        CompositingMaterial = new Material(Shader.Find("Hidden/SSRCompositing"));
        PyramidDepthGenerator.DepthPyramidInit(ref DepthPyramidMipIDs);
        SSRInputData.FrameIndex = 0;
        SVGFInputData.FrameIndex = 0;
    }

    public override void Render(PostProcessRenderContext RenderContent) {
        RenderContent.command.BeginSample("ScreenSpaceReflection");

        int2 ScreenSize = new int2(RenderContent.camera.pixelWidth, RenderContent.camera.pixelHeight);
        int2 HZBSize = new int2(1024, 1024);
        //int2 HZBSize = ScreenSize;
        Matrix4x4 WorldToViewMatrix = RenderContent.camera.worldToCameraMatrix;
        Matrix4x4 ProjectionMatrix = GL.GetGPUProjectionMatrix(RenderContent.camera.projectionMatrix, false);
        Matrix4x4 ViewProjectionMatrix = ProjectionMatrix * WorldToViewMatrix;

        {
            SSRParameter.NumRays = settings.NumRays;
            SSRParameter.NumSteps = settings.NumSteps;
            SSRParameter.BRDFBias = settings.BRDFBias;
            SSRParameter.Fadeness = settings.Fadeness;
            SSRParameter.RoughnessDiscard = settings.RoughnessDiscard;
            ////////////////////////////////////////////////////////////////
            SSRInputData.FrameIndex += 1;
            SSRInputData.Matrix_Proj = ProjectionMatrix;
            SSRInputData.Matrix_InvProj = ProjectionMatrix.inverse;
            SSRInputData.Matrix_InvViewProj = ViewProjectionMatrix.inverse;
            SSRInputData.Matrix_WorldToView = WorldToViewMatrix;
            SSRInputData.SRV_PyramidColor = RenderContent.source;
            SSRInputData.SRV_SceneDepth = BuiltinRenderTextureType.ResolvedDepth;
            SSRInputData.SRV_GBufferNormal = BuiltinRenderTextureType.GBuffer2;
            SSRInputData.SRV_GBufferRoughness = BuiltinRenderTextureType.GBuffer1;
            SSRInputData.TraceResolution = new float4(ScreenSize.x / (int)settings.RenderMode.value, ScreenSize.y / (int)settings.RenderMode.value, 1.0f / (ScreenSize.x / (int)settings.RenderMode.value), 1.0f / (ScreenSize.y / (int)settings.RenderMode.value));
        }

        {
            SVGFParamete.NumSpatial = settings.NumSpatial;
            SVGFParamete.SpatialRadius = settings.SpatialRadius;
            SVGFParamete.TemporalScale = settings.TemporalScale;
            SVGFParamete.TemporalWeight = settings.TemporalWeight;
            ////////////////////////////////////////////////////////////////
            SVGFInputData.FrameIndex += 1;
            SVGFInputData.Resolution = new float4(ScreenSize.x, ScreenSize.y, 1.0f / ScreenSize.x , 1.0f / ScreenSize.y);
            SVGFInputData.Matrix_InvProj = ProjectionMatrix.inverse;
            SVGFInputData.Matrix_ViewProj = ViewProjectionMatrix;
            SVGFInputData.Matrix_InvViewProj = ViewProjectionMatrix.inverse;
            SVGFInputData.Matrix_WorldToView = WorldToViewMatrix;
            SVGFInputData.SRV_GBufferMotion = BuiltinRenderTextureType.MotionVectors;
            SVGFInputData.SRV_SceneDepth = BuiltinRenderTextureType.ResolvedDepth;
            SVGFInputData.SRV_GBufferNormal = BuiltinRenderTextureType.GBuffer2;
            SVGFInputData.SRV_GBufferRoughness = BuiltinRenderTextureType.GBuffer1;
        }

        //////Depth Pyramid
        RenderContent.command.BeginSample("Depth Pyramid");
        RenderTextureDescriptor PyramidDepthDesc = new RenderTextureDescriptor(HZBSize.x, HZBSize.y, RenderTextureFormat.RHalf, 0) {
            bindMS = false,
            useMipMap = true,
            autoGenerateMips = false,
            dimension = TextureDimension.Tex2D
        };
        RTV_PyramidDepth = RenderTexture.GetTemporary(PyramidDepthDesc);
        RTV_PyramidDepth.filterMode = FilterMode.Point;

        SSRInputData.SRV_PyramidDepth = new RenderTargetIdentifier(RTV_PyramidDepth);
        RenderContent.command.BlitFullscreenTriangle(BuiltinRenderTextureType.ResolvedDepth, SSRInputData.SRV_PyramidDepth);
        PyramidDepthGenerator.DepthPyramidUpdate(ref DepthPyramidMipIDs,ref HZBSize, SSRInputData.SRV_PyramidDepth, RenderContent.command);
        RenderContent.command.EndSample("Depth Pyramid");

        //////Ray Casting
        RenderContent.command.BeginSample("Reflection Raytrace");
        RenderContent.command.GetTemporaryRT(SSRShaderID.UAV_ReflectionUVWPDF, (int)SSRInputData.TraceResolution.x, (int)SSRInputData.TraceResolution.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default, 1, true);
        RenderContent.command.GetTemporaryRT(SSRShaderID.UAV_ReflectionColorMask, (int)SSRInputData.TraceResolution.x, (int)SSRInputData.TraceResolution.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default, 1, true);
        RenderTargetIdentifier ReflectionUWVPDF = new RenderTargetIdentifier(SSRShaderID.UAV_ReflectionUVWPDF);
        RenderTargetIdentifier ReflectionColorMask = new RenderTargetIdentifier(SSRShaderID.UAV_ReflectionColorMask);
        SSR.Render(RenderContent.command, ReflectionUWVPDF, ReflectionColorMask, ref SSRParameter, ref SSRInputData);
        RenderContent.command.EndSample("Reflection Raytrace");

        //////Spatial Filter
        RenderContent.command.BeginSample("Reflection SpatialFilter");
        RenderContent.command.GetTemporaryRT(SVGF_SpatialShaderID.UAV_SpatialColor, (int)SSRInputData.TraceResolution.x, (int)SSRInputData.TraceResolution.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default, 1, true);
        RenderTargetIdentifier UAV_SpatialColor;
        if(settings.EnableSpatial) {
            UAV_SpatialColor = new RenderTargetIdentifier(SVGF_SpatialShaderID.UAV_SpatialColor);
            SVGFilter.SpatialFilter(RenderContent.command, ReflectionUWVPDF, ReflectionColorMask, UAV_SpatialColor, ref SVGFParamete, ref SVGFInputData);
        } else {
            UAV_SpatialColor = ReflectionColorMask;
        }
        RenderContent.command.EndSample("Reflection SpatialFilter");

        //////Temporal Filter
        RenderContent.command.BeginSample("Reflection TemporalFilter");
        RenderContent.command.GetTemporaryRT(SVGF_TemporalShaderID.SRV_PrevColor, (int)SSRInputData.TraceResolution.x, (int)SSRInputData.TraceResolution.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default, 1, false);
        RenderContent.command.GetTemporaryRT(SVGF_TemporalShaderID.UAV_TemporalColor, (int)SSRInputData.TraceResolution.x, (int)SSRInputData.TraceResolution.y, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Default, 1, true);
        RenderTargetIdentifier SRV_PrevColor = new RenderTargetIdentifier(SVGF_TemporalShaderID.SRV_PrevColor);
        RenderTargetIdentifier UAV_TemporalColor = new RenderTargetIdentifier(SVGF_TemporalShaderID.UAV_TemporalColor);
        SVGFilter.TemporalFilter(RenderContent.command, ReflectionUWVPDF, UAV_SpatialColor, SRV_PrevColor, UAV_TemporalColor, ref SVGFParamete, ref SVGFInputData);
        RenderContent.command.CopyTexture(UAV_TemporalColor, SRV_PrevColor);
        RenderContent.command.EndSample("Reflection TemporalFilter");

        RenderContent.command.BeginSample("Reflection Blit");
        //PropertySheet Sheet = RenderContent.propertySheets.Get(CompositingShader);
        RenderContent.command.SetGlobalMatrix("_Matrix_InvViewProj", ViewProjectionMatrix.inverse);
        RenderContent.command.SetGlobalTexture("_SRV_SSRAlpha", UAV_SpatialColor);
        RenderContent.command.SetGlobalTexture("_SRV_SSRColor", UAV_TemporalColor);
        BlitFullscreenTriangle(RenderContent.command, RenderContent.source, RenderContent.destination, CompositingMaterial, 0);
        RenderContent.command.EndSample("Reflection Blit");
        
        {
            RenderContent.command.ReleaseTemporaryRT(SSRShaderID.UAV_ReflectionUVWPDF);
            RenderContent.command.ReleaseTemporaryRT(SSRShaderID.UAV_ReflectionColorMask);
            RenderContent.command.ReleaseTemporaryRT(SVGF_SpatialShaderID.UAV_SpatialColor);
            RenderContent.command.ReleaseTemporaryRT(SVGF_TemporalShaderID.SRV_PrevColor);
            RenderContent.command.ReleaseTemporaryRT(SVGF_TemporalShaderID.UAV_TemporalColor);
            RenderTexture.ReleaseTemporary(RTV_PyramidDepth);
            SVGFInputData.Matrix_PrevViewProj = ViewProjectionMatrix;
        }

        RenderContent.command.EndSample("ScreenSpaceReflection");
    }

    public override void Release() {

    }

	public static void BlitFullscreenTriangle(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material, int pass, bool clear = false)
	{
		cmd.SetGlobalTexture("_MainTex", source);
		cmd.SetRenderTarget(destination);

		if (clear)
			cmd.ClearRenderTarget(true, true, Color.clear);

		cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, material, 0, pass);
	}
}
