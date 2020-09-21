Shader "Hidden/SSRCompositing"
{
    Properties
    {

    }

	CGINCLUDE
        #include "UnityCG.cginc"
        #include "../Private/Common.hlsl"
        #include "../Private/ImageBasedLighting.hlsl"

        float4x4 _Matrix_InvViewProj;
        sampler2D _MainTex, _SRV_SSRColor, _SRV_SSRAlpha;
        sampler2D _CameraDepthTexture, _CameraMotionVectorsTexture, _CameraGBufferTexture0, _CameraGBufferTexture1, _CameraGBufferTexture2, _CameraReflectionsTexture;

        struct VertexInput 
        {
            float2 uv0 : TEXCOORD0;
            float4 vertex : POSITION;
        };

        struct VertexOutput
        {
            float2 uv0 : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };

        VertexOutput VertexFunction_Quad(VertexInput AppInput) 
        {
            VertexOutput VertexOut;
            VertexOut.vertex = UnityObjectToClipPos(AppInput.vertex);
            VertexOut.uv0 = AppInput.uv0;
            return VertexOut;
        }

        VertexOutput VertexFunction_Triangle(VertexInput AppInput) 
        {
            VertexOutput VertexOut;
            VertexOut.vertex = float4(AppInput.vertex.xy, 0, 1);
            VertexOut.uv0 = (AppInput.vertex.xy + 1) * 0.5;
            #if UNITY_UV_STARTS_AT_TOP
                VertexOut.uv0.y = 1 - VertexOut.uv0.y;
            #endif
            return VertexOut;
        }

        float4 PixelFunction_CompositingSSR(VertexOutput VertexIn) : SV_Target
        {	 
            float2 UV = VertexIn.uv0;

            float SceneDepth = tex2D(_CameraDepthTexture, UV);
            float4 WorldNormal = tex2D(_CameraGBufferTexture2, UV) * 2 - 1;
            float4 SpecularColor = tex2D(_CameraGBufferTexture1, UV);
            float Roughness = clamp(1 - SpecularColor.a, 0.02, 1);

            float3 ScreenPos = GetScreenSpacePos(UV, SceneDepth);
            float3 WorldPos = GetWorldSpacePos(ScreenPos, _Matrix_InvViewProj);
            float3 ViewDir = GetViewDir(WorldPos, _WorldSpaceCameraPos);

            float NoV = saturate(dot(WorldNormal, -ViewDir));
            float4 EnvBRDF = EnvBRDFApprox(SpecularColor.rgb, Roughness, NoV);

            float4 SceneColor = tex2D(_MainTex, UV);

            float4 CubemapColor = tex2D(_CameraReflectionsTexture, UV);
            SceneColor.rgb = max(1e-5, SceneColor.rgb - CubemapColor.rgb);
            float4 SSRColor = tex2D(_SRV_SSRColor, UV);
            float SSRMask = saturate(Square(tex2D(_SRV_SSRAlpha, UV).a) * 2);
            float4 ReflectionColor = (CubemapColor * (1 - SSRMask)) + (SSRColor * EnvBRDF * SSRMask);

            return SceneColor + ReflectionColor;
        }
	ENDCG

    SubShader
    {
        Cull Off
		ZTest Always
        ZWrite Off

		Pass 
		{
			Name "SSR_Compositing"

			CGPROGRAM
			#pragma target 4.5
			#pragma vertex VertexFunction_Triangle
			#pragma fragment PixelFunction_CompositingSSR
			ENDCG
		}
    }
}
