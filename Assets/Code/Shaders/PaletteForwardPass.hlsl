
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct Attributes
{
	float3 positionOS : POSITION;
	float3 normalOS : NORMAL;
	float2 uv : TEXCOORD0;
};

struct Interpolators
{
	float4 positionCS : SV_POSITION;
	float3 positionWS : TEXCOORD1;
	float3 normalWS : TEXCOORD2;

	float2 uv : TEXCOORD0;
};

Interpolators Vertex (Attributes i)
{
	Interpolators o;

	VertexPositionInputs posInputs = GetVertexPositionInputs(i.positionOS);
	VertexNormalInputs normalInputs = GetVertexNormalInputs(i.normalOS);

	o.positionCS = posInputs.positionCS;
	o.positionWS = posInputs.positionWS;
	o.normalWS = normalInputs.normalWS;
	o.uv = i.uv;

	return o;
}

TEXTURE2D(_Palette); SAMPLER(sampler_Palette);
TEXTURE2D(_Reference); SAMPLER(sampler_Reference);
TEXTURE2D(_ShadowMap); SAMPLER(sampler_ShadowMap);
TEXTURE2D(_ShadowMapMetal); SAMPLER(sampler_ShadowMapMetal);
float4 _Color;

float _OverrideUV;
float _PaletteIndex;
float _PaletteSize;

float4 Fragment (Interpolators i) : SV_TARGET
{
	float2 uv = i.uv;
	if (_OverrideUV > 0.5f)
	{
		uv = float2(_PaletteIndex % _PaletteSize, floor(_PaletteIndex / _PaletteSize)) / _PaletteSize;
	}

	float4 col = SAMPLE_TEXTURE2D(_Palette, sampler_Palette, uv) * _Color;
	float4 ref = SAMPLE_TEXTURE2D(_Reference, sampler_Reference, uv);

	InputData inputData = (InputData)0;
	inputData.positionWS = i.positionWS;
	inputData.normalWS = normalize(i.normalWS);
	inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(i.positionWS);
	inputData.shadowCoord = TransformWorldToShadowCoord(i.positionWS);

	SurfaceData surfaceData = (SurfaceData)0;
	surfaceData.albedo = 1.0;
	surfaceData.alpha = col.a;
	surfaceData.specular = 0.5;
	surfaceData.metallic = ref.x;
	surfaceData.smoothness = ref.y;

	float4 lighting = UniversalFragmentPBR(inputData, surfaceData);
	float lIntensity = (lighting.r + lighting.g + lighting.b) / 3.0;
	float s = max(max(lighting.r, lighting.g), lighting.b);
	float4 lightCol = 0;
	if (s > 0.001) lightCol = lighting / s;

	float4 shadowMap = SAMPLE_TEXTURE2D(_ShadowMap, sampler_ShadowMap, float2(lIntensity, 0.5));
	float4 shadowMapMetallica = SAMPLE_TEXTURE2D(_ShadowMapMetal, sampler_ShadowMapMetal, float2(lIntensity, 0.5));

	lighting = lerp(unity_AmbientSky, lightCol, lerp(shadowMap, shadowMapMetallica, ref.x));
	
	float d = 1 + dot(inputData.normalWS, float3(0, 1, 0)) * 0.2f;

	return lighting * col * d;
}