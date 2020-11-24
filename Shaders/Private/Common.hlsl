#ifndef _BaseCommon_
#define _BaseCommon_

//#define float half
//#define float2 half2
//#define float3 half3
//#define float4 half4
//#define float3x3 half3x3
//#define float4x4 half4x4
//#define float4x3 half4x3
#define Pi 3.1415926
#define Inv_Pi 0.3183091
#define Two_Pi 6.2831852
#define Inv_Two_Pi 0.15915494

SamplerState Global_point_clamp_sampler, Global_bilinear_clamp_sampler, Global_trilinear_clamp_sampler, Global_point_repeat_sampler, Global_bilinear_repeat_sampler, Global_trilinear_repeat_sampler;

float Square(float x)
{
    return x * x;
}

float2 Square(float2 x)
{
    return x * x;
}

float3 Square(float3 x)
{
    return x * x;
}

float4 Square(float4 x)
{
    return x * x;
}

float pow2(float x)
{
    return x * x;
}

float2 pow2(float2 x)
{
    return x * x;
}

float3 pow2(float3 x)
{
    return x * x;
}

float4 pow2(float4 x)
{
    return x * x;
}

float pow3(float x)
{
    return x * x * x;
}

float2 pow3(float2 x)
{
    return x * x * x;
}

float3 pow3(float3 x)
{
    return x * x * x;
}

float4 pow3(float4 x)
{
    return x * x * x;
}

float pow4(float x)
{
    float xx = x * x;
    return xx * xx;
}

float2 pow4(float2 x)
{
    float2 xx = x * x;
    return xx * xx;
}

float3 pow4(float3 x)
{
    float3 xx = x * x;
    return xx * xx;
}

float4 pow4(float4 x)
{
    float4 xx = x * x;
    return xx * xx;
}

float pow5(float x)
{
    float xx = x * x;
    return xx * xx * x;
}

float2 pow5(float2 x)
{
    float2 xx = x * x;
    return xx * xx * x;
}

float3 pow5(float3 x)
{
    float3 xx = x * x;
    return xx * xx * x;
}

float4 pow5(float4 x)
{
    float4 xx = x * x;
    return xx * xx * x;
}

float pow6(float x)
{
    float xx = x * x;
    return xx * xx * xx;
}

float2 pow6(float2 x)
{
    float2 xx = x * x;
    return xx * xx * xx;
}

float3 pow6(float3 x)
{
    float3 xx = x * x;
    return xx * xx * xx;
}

float4 pow6(float4 x)
{
    float4 xx = x * x;
    return xx * xx * xx;
}
float min3(float a, float b, float c)
{
    return min(min(a, b), c);
}

float max3(float a, float b, float c)
{
    return max(a, max(b, c));
}

float4 min3(float4 a, float4 b, float4 c)
{
    return float4(
        min3(a.x, b.x, c.x),
        min3(a.y, b.y, c.y),
        min3(a.z, b.z, c.z),
        min3(a.w, b.w, c.w));
}

float4 max3(float4 a, float4 b, float4 c)
{
    return float4(
        max3(a.x, b.x, c.x),
        max3(a.y, b.y, c.y),
        max3(a.z, b.z, c.z),
        max3(a.w, b.w, c.w));
}

float4 Texture1DSample(Texture1D Tex, SamplerState Sampler, float UV)
{
#if COMPUTESHADER
	return Tex.SampleLevel(Sampler, UV, 0);
#else
	return Tex.Sample(Sampler, UV);
#endif
}

float4 Texture2DSample(Texture2D Tex, SamplerState Sampler, float2 UV)
{
#if COMPUTESHADER
	return Tex.SampleLevel(Sampler, UV, 0);//
#else
	return Tex.Sample(Sampler, UV);
#endif
}


float4 Texture3DSample(Texture3D Tex, SamplerState Sampler, float3 UV)
{
#if COMPUTESHADER
	return Tex.SampleLevel(Sampler, UV, 0);
#else
	return Tex.Sample(Sampler, UV);
#endif
}


float4 TextureCubeSample(TextureCube Tex, SamplerState Sampler, float3 UV)
{
#if COMPUTESHADER
	return Tex.SampleLevel(Sampler, UV, 0);
#else
	return Tex.Sample(Sampler, UV);
#endif
}


float4 Texture1DSampleLevel(Texture1D Tex, SamplerState Sampler, float UV, float Mip)
{
	return Tex.SampleLevel(Sampler, UV, Mip);
}


float4 Texture2DSampleLevel(Texture2D Tex, SamplerState Sampler, float2 UV, float Mip)
{
	return Tex.SampleLevel(Sampler, UV, Mip);
}


float4 Texture2DSampleBias(Texture2D Tex, SamplerState Sampler, float2 UV, float MipBias)
{
#if COMPUTESHADER
	return Tex.SampleLevel(Sampler, UV, 0);
#else
	return Tex.SampleBias(Sampler, UV, MipBias);
#endif
}


float4 Texture2DSampleGrad(Texture2D Tex, SamplerState Sampler, float2 UV, float2 DDX, float2 DDY)
{
	return Tex.SampleGrad(Sampler, UV, DDX, DDY);
}


float4 Texture3DSampleLevel(Texture3D Tex, SamplerState Sampler, float3 UV, float Mip)
{
	return Tex.SampleLevel(Sampler, UV, Mip);
}


float4 Texture3DSampleBias(Texture3D Tex, SamplerState Sampler, float3 UV, float MipBias)
{
#if COMPUTESHADER
	return Tex.SampleBias(Sampler, UV, 0);
#else
	return Tex.SampleBias(Sampler, UV, MipBias);
#endif
}


float4 Texture3DSampleGrad(Texture3D Tex, SamplerState Sampler, float3 UV, float3 DDX, float3 DDY)
{
	return Tex.SampleGrad(Sampler, UV, DDX, DDY);
}


float4 TextureCubeSampleLevel(TextureCube Tex, SamplerState Sampler, float3 UV, float Mip)
{
	return Tex.SampleLevel(Sampler, UV, Mip);
}


float TextureCubeSampleDepthLevel(TextureCube TexDepth, SamplerState Sampler, float3 UV, float Mip)
{
	return TexDepth.SampleLevel(Sampler, UV, Mip).x;
}


float4 TextureCubeSampleBias(TextureCube Tex, SamplerState Sampler, float3 UV, float MipBias)
{
#if COMPUTESHADER
	return Tex.SampleLevel(Sampler, UV, 0);
#else
	return Tex.SampleBias(Sampler, UV, MipBias);
#endif
}


float4 TextureCubeSampleGrad(TextureCube Tex, SamplerState Sampler, float3 UV, float3 DDX, float3 DDY)
{
	return Tex.SampleGrad(Sampler, UV, DDX, DDY);
}

/////////////////BicubicSampler
void Bicubic2DCatmullRom(in float2 UV, in float2 Size, in float2 InvSize, out float2 Sample[3], out float2 Weight[3])
{
    UV *= Size;

    float2 tc = floor(UV - 0.5) + 0.5;
    float2 f = UV - tc;
    float2 f2 = f * f;
    float2 f3 = f2 * f;

    float2 w0 = f2 - 0.5 * (f3 + f);
    float2 w1 = 1.5 * f3 - 2.5 * f2 + 1;
    float2 w3 = 0.5 * (f3 - f2);
    float2 w2 = 1 - w0 - w1 - w3;

    Weight[0] = w0;
    Weight[1] = w1 + w2;
    Weight[2] = w3;

    Sample[0] = tc - 1;
    Sample[1] = tc + w2 / Weight[1];
    Sample[2] = tc + 2;

    Sample[0] *= InvSize;
    Sample[1] *= InvSize;
    Sample[2] *= InvSize;
}

#define BICUBIC_CATMULL_ROM_SAMPLES 5

struct FCatmullRomSamples
{
    // Constant number of samples (BICUBIC_CATMULL_ROM_SAMPLES)
    uint Count;

    // Constant sign of the UV direction from master UV sampling location.
    int2 UVDir[BICUBIC_CATMULL_ROM_SAMPLES];

    // Bilinear sampling UV coordinates of the samples
    float2 UV[BICUBIC_CATMULL_ROM_SAMPLES];

    // Weights of the samples
    float Weight[BICUBIC_CATMULL_ROM_SAMPLES];

    // Final multiplier (it is faster to multiply 3 RGB values than reweights the 5 weights)
    float FinalMultiplier;
};

FCatmullRomSamples GetBicubic2DCatmullRomSamples(float2 UV, float2 Size, in float2 InvSize)
{
    FCatmullRomSamples Samples;
    Samples.Count = BICUBIC_CATMULL_ROM_SAMPLES;

    float2 Weight[3];
    float2 Sample[3];
    Bicubic2DCatmullRom(UV, Size, InvSize, Sample, Weight);

    // Optimized by removing corner samples
    Samples.UV[0] = float2(Sample[1].x, Sample[0].y);
    Samples.UV[1] = float2(Sample[0].x, Sample[1].y);
    Samples.UV[2] = float2(Sample[1].x, Sample[1].y);
    Samples.UV[3] = float2(Sample[2].x, Sample[1].y);
    Samples.UV[4] = float2(Sample[1].x, Sample[2].y);

    Samples.Weight[0] = Weight[1].x * Weight[0].y;
    Samples.Weight[1] = Weight[0].x * Weight[1].y;
    Samples.Weight[2] = Weight[1].x * Weight[1].y;
    Samples.Weight[3] = Weight[2].x * Weight[1].y;
    Samples.Weight[4] = Weight[1].x * Weight[2].y;

    Samples.UVDir[0] = int2(0, -1);
    Samples.UVDir[1] = int2(-1, 0);
    Samples.UVDir[2] = int2(0, 0);
    Samples.UVDir[3] = int2(1, 0);
    Samples.UVDir[4] = int2(0, 1);

    // Reweight after removing the corners
    float CornerWeights;
    CornerWeights = Samples.Weight[0];
    CornerWeights += Samples.Weight[1];
    CornerWeights += Samples.Weight[2];
    CornerWeights += Samples.Weight[3];
    CornerWeights += Samples.Weight[4];
    Samples.FinalMultiplier = 1 / CornerWeights;

    return Samples;
}

float4 Texture2DSampleBicubic(Texture2D Tex, SamplerState Sampler, float2 UV, float2 Size, in float2 InvSize)
{
	FCatmullRomSamples Samples = GetBicubic2DCatmullRomSamples(UV, Size, InvSize);

	float4 OutColor = 0;
	for (uint i = 0; i < Samples.Count; i++)
	{
		OutColor += Tex.SampleLevel(Sampler, Samples.UV[i], 0) * Samples.Weight[i];
	}
	OutColor *= Samples.FinalMultiplier;

	return OutColor;
}

//converts an input 1d to 2d position. Useful for locating z frames that have been laid out in a 2d grid like a flipbook.
float2 Tile1Dto2D(float xsize, float idx)
{
	float2 xyidx = 0;
	xyidx.y = floor(idx / xsize);
	xyidx.x = idx - xsize * xyidx.y;

	return xyidx;
}

float4 PseudoVolumeTexture(Texture2D Tex, SamplerState TexSampler, float3 inPos, float2 xysize, float numframes,
	uint mipmode = 0, float miplevel = 0, float2 InDDX = 0, float2 InDDY = 0)
{
	float zframe = ceil(inPos.z * numframes);
	float zphase = frac(inPos.z * numframes);

	float2 uv = frac(inPos.xy) / xysize;

	float2 curframe = Tile1Dto2D(xysize.x, zframe) / xysize;
	float2 nextframe = Tile1Dto2D(xysize.x, zframe + 1) / xysize;

	float4 sampleA = 0, sampleB = 0;
	switch (mipmode)
	{
	case 0: // Mip level
		sampleA = Tex.SampleLevel(TexSampler, uv + curframe, miplevel);
		sampleB = Tex.SampleLevel(TexSampler, uv + nextframe, miplevel);
		break;
	case 1: // Gradients automatic from UV
		sampleA = Texture2DSample(Tex, TexSampler, uv + curframe);
		sampleB = Texture2DSample(Tex, TexSampler, uv + nextframe);
		break;
	case 2: // Deriviatives provided
		sampleA = Tex.SampleGrad(TexSampler, uv + curframe,  InDDX, InDDY);
		sampleB = Tex.SampleGrad(TexSampler, uv + nextframe, InDDX, InDDY);
		break;
	default:
		break;
	}

	return lerp(sampleA, sampleB, zphase);
}

float Luma4(float3 Color)
{
    return (Color.g * 2) + (Color.r + Color.b);
}

float Luminance(float3 rgb)
{
    return dot( rgb, float3(0.0396819152, 0.458021790, 0.00609653955) );
}

float HdrWeight4(float3 Color, float Exposure)
{
    return rcp(Luma4(Color) * Exposure + 4);
}

float HdrWeightY(float Color, float Exposure)
{
    return rcp(Color * Exposure + 4);
}

float3 RGBToYCoCg(float3 RGB)
{
    float Y = dot(RGB, float3(1, 2, 1));
    float Co = dot(RGB, float3(2, 0, -2));
    float Cg = dot(RGB, float3(-1, 2, -1));

    float3 YCoCg = float3(Y, Co, Cg);
    return YCoCg;
}

float3 YCoCgToRGB(float3 YCoCg)
{
    float Y = YCoCg.x * 0.25;
    float Co = YCoCg.y * 0.25;
    float Cg = YCoCg.z * 0.25;

    float R = Y + Co - Cg;
    float G = Y + Cg;
    float B = Y - Co - Cg;
    
    return float3(R, G, B);
}

float acosFast(float inX)
{
    float x = abs(inX);
    float res = -0.156583f * x + (0.5 * Pi);
    res *= sqrt(1 - x);
    return (inX >= 0) ? res : Pi - res;
}

float asinFast(float x)
{
    return (0.5 * Pi) - acosFast(x);
}

float ClampedPow(float X, float Y)
{
	return pow(max(abs(X), 0.000001), Y);
}

float CharlieL(float x, float r)
{
    r = saturate(r);
    r = 1 - (1 - r) * (1 - r);

    float a = lerp(25.3245, 21.5473, r);
    float b = lerp(3.32435, 3.82987, r);
    float c = lerp(0.16801, 0.19823, r);
    float d = lerp(-1.27393, -1.97760, r);
    float e = lerp(-4.85967, -4.32054, r);

    return a / (1 + b * pow(x, c)) + d * x + e;
}

void ConvertAnisotropyToRoughness(float Roughness, float Anisotropy, out float RoughnessT, out float RoughnessB) {
	Roughness *= Roughness;
    float AnisoAspect = sqrt(1 - 0.9 * Anisotropy);
    RoughnessT = Roughness / AnisoAspect; 
    RoughnessB = Roughness * AnisoAspect; 
}

float3 ComputeGrainNormal(float3 grainDir, float3 V) {
	float3 B = cross(-V, grainDir);
	return cross(B, grainDir);
}

float3 GetAnisotropicModifiedNormal(float3 grainDir, float3 N, float3 V, float Anisotropy) {
	float3 grainNormal = ComputeGrainNormal(grainDir, V);
	return normalize(lerp(N, grainNormal, Anisotropy));
}

float3 GetViewSpaceNormal(float3 normal, float4x4 _WToCMatrix)
{
    const float3 viewNormal = mul((float3x3)_WToCMatrix, normal.rgb);
    return normalize(viewNormal);
}

float3 GetScreenSpacePos(float2 uv, float depth)
{
    return float3(uv * 2 - 1, depth);
}

float3 GetWorldSpacePos(float3 ScreenPos, float4x4 Matrix_InvViewProj)
{
    float4 worldPos = mul( Matrix_InvViewProj, float4(ScreenPos, 1) );
    return worldPos.xyz / worldPos.w;
}

float3 GetViewSpacePos(float3 ScreenPos, float4x4 Matrix_InvProj)
{
    float4 viewPos = mul(Matrix_InvProj, float4(ScreenPos, 1));
    return viewPos.xyz / viewPos.w;
}

float3 GetViewDir(float3 worldPos, float3 ViewPos)
{
    return normalize(worldPos - ViewPos);
}

float2 GetMotionVector(float SceneDepth, float2 inUV, float4x4 Matrix_InvViewProj, float4x4 _PrevViewProjectionMatrix, float4x4 _ViewProjectionMatrix)
{
    float3 ScreenPos = GetScreenSpacePos(inUV, SceneDepth);
    float4 worldPos = float4(GetWorldSpacePos(ScreenPos, Matrix_InvViewProj), 1);

    float4 prevClipPos = mul(_PrevViewProjectionMatrix, worldPos);
    float4 curClipPos = mul(_ViewProjectionMatrix, worldPos);

    float2 prevHPos = prevClipPos.xy / prevClipPos.w;
    float2 curHPos = curClipPos.xy / curClipPos.w;

    float2 vPosPrev = (prevHPos.xy + 1) / 2;
    float2 vPosCur = (curHPos.xy + 1) / 2;
    return vPosCur - vPosPrev;
}

float2 UnitVectorToOctahedron( float3 N )
{
    N.xy /= dot( 1, abs(N) );
    if( N.z <= 0 ) {
        N.xy = ( 1 - abs(N.yx) ) * ( N.xy >= 0 ? float2(1, 1) : float2(-1, -1) );
    }
    return N.xy;
}

float3 OctahedronToUnitVector( float2 Oct )
{
    float3 N = float3( Oct, 1 - dot( 1, abs(Oct) ) );
    if( N.z < 0 ) {
        N.xy = ( 1 - abs(N.yx) ) * ( N.xy >= 0 ? float2(1, 1) : float2(-1, -1) );
    }
    return normalize(N);
}

float2 UnitVectorToHemiOctahedron( float3 N )
{
	N.xy /= dot( 1, abs(N) );
	return float2( N.x + N.y, N.x - N.y );
}

float3 HemiOctahedronToUnitVector( float2 Oct )
{
	Oct = float2( Oct.x + Oct.y, Oct.x - Oct.y ) * 0.5;
	float3 N = float3( Oct, 1 - dot( 1, abs(Oct) ) );
	return normalize(N);
}

float3 Pack1212To888( float2 x )
{
	// Pack 12:12 to 8:8:8
#if 0
	uint2 x1212 = (uint2)( x * 4095.0 );
	uint2 High = x1212 >> 8;
	uint2 Low = x1212 & 255;
	uint3 x888 = uint3( Low, High.x | (High.y << 4) );
	return x888 / 255.0;
#else
	float2 x1212 = floor( x * 4095 );
	float2 High = floor( x1212 / 256 );	// x1212 >> 8
	float2 Low = x1212 - High * 256;	// x1212 & 255
	float3 x888 = float3( Low, High.x + High.y * 16 );
	return saturate( x888 / 255 );
#endif
}

float2 Pack888To1212( float3 x )
{
	// Pack 8:8:8 to 12:12
#if 0
	uint3 x888 = (uint3)( x * 255.0 );
	uint High = x888.z >> 4;
	uint Low = x888.z & 15;
	uint2 x1212 = x888.xy | uint2( Low << 8, High << 8 );
	return x1212 / 4095.0;
#else
	float3 x888 = floor( x * 255 );
	float High = floor( x888.z / 16 );	// x888.z >> 4
	float Low = x888.z - High * 16;		// x888.z & 15
	float2 x1212 = x888.xy + float2( Low, High ) * 256;
	return saturate( x1212 / 4095 );
#endif
}

struct ThinGBufferData
{
    float Roughness;
    float Reflactance;
	float3 WorldNormal;
	float3 AlbedoColor;
};

float3 EncodeNormalDir( float3 N )
{
	return Pack1212To888( UnitVectorToOctahedron( N ) * 0.5 + 0.5 );
}

float3 DecodeNormalDir( float3 N )
{
	return OctahedronToUnitVector( Pack888To1212( N ) * 2 - 1 );
}

void EncodeIntGBuffer(ThinGBufferData GBufferData, out int EncodeData_GBufferA, out int EncodeData_GBufferB)
{
    int2 EncodeNormal = int2(saturate( UnitVectorToOctahedron(GBufferData.WorldNormal) * 0.5 + 0.5) * 0xFFF);
    int EncodeRoughness = int(saturate(GBufferData.Roughness) * 0xFF);
    int3 EncodeAlbedo = int3(saturate(GBufferData.AlbedoColor) * 0xFF);
    int EncodeReflactance = int(saturate(GBufferData.Reflactance) * 0xFF);
                    
    EncodeData_GBufferA = (EncodeNormal.x << 20) + (EncodeNormal.y << 8) + EncodeRoughness;
    EncodeData_GBufferB = (EncodeAlbedo.x << 24) + (EncodeAlbedo.y << 16) + (EncodeAlbedo.z << 8) + EncodeReflactance;
}

void DecodeIntGBuffer(int EncodeData_GBufferA, int EncodeData_GBufferB, out ThinGBufferData GBufferData)
{
    GBufferData.WorldNormal = OctahedronToUnitVector( (int2(EncodeData_GBufferA >> 20, EncodeData_GBufferA >> 8) & 0xFFF) / float(0xFFF)  * 2 - 1);
    GBufferData.Roughness = ((EncodeData_GBufferA >> 32) & 0xFF) / 255.0f;
    GBufferData.AlbedoColor = (int3(EncodeData_GBufferB >> 24, EncodeData_GBufferB >> 16, EncodeData_GBufferB >> 8) & 0xFF) / 255.0f;
    GBufferData.Reflactance = (EncodeData_GBufferB >> 24 & 0xFF) / 255.0f;
}

void EncodeFloatGBuffer(ThinGBufferData GBufferData, out float4 EncodeData_GBufferA, out float4 EncodeData_GBufferB)
{
    float3 EncodeNormal = EncodeNormalDir(GBufferData.WorldNormal);
    float EncodeRoughness = GBufferData.Roughness;
    float3 EncodeAlbedo = GBufferData.AlbedoColor;
    float EncodeReflactance = GBufferData.Reflactance;
                    
    EncodeData_GBufferA = float4(EncodeNormal, EncodeRoughness);
    EncodeData_GBufferB = float4(EncodeAlbedo, EncodeReflactance);
}

void DecodeFloatGBuffer(float4 EncodeData_GBufferA, float4 EncodeData_GBufferB, out ThinGBufferData GBufferData)
{
    GBufferData.WorldNormal = DecodeNormalDir(EncodeData_GBufferA.xyz);
    GBufferData.Roughness = EncodeData_GBufferA.a;
    GBufferData.AlbedoColor = EncodeData_GBufferB.rgb;
    GBufferData.Reflactance = EncodeData_GBufferB.a;
}

#endif
