﻿
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

struct Attributes
{
	float3 positionOS : POSITION;
	float3 normalOS : NORMAL;
};

struct Interpolators
{
	float4 positionCS : SV_POSITION;
};

float3 _LightDirection;

float4 GetShadowCasterPositionCS (float3 positionWS, float3 normalWS)
{
	float3 lightDirectionWS = _LightDirection;
	float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
#if UNITY_REVERSED_Z
	positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#else
	positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#endif
	return positionCS;
}

Interpolators Vertex (Attributes i)
{
	Interpolators o;

	VertexPositionInputs posInputs = GetVertexPositionInputs(i.positionOS);
	VertexNormalInputs normalInputs = GetVertexNormalInputs(i.normalOS);

	o.positionCS = GetShadowCasterPositionCS(posInputs.positionWS, normalInputs.normalWS);

	return o;
}

float4 Fragment (Interpolators i) : SV_TARGET
{
	return 0;
}