Shader "BoschingMachine/Palette"
{
	Properties
	{
		[Header(Surface Options)]
		[MainTexture][NoScaleOffset] _Palette("Palette Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset] _Reference("Palette Reference (MS_)", 2D) = "white" {}
		[NoScaleOffset] _ShadowMap("Shadow Map (V)", 2D) = "white" {}
		[NoScaleOffset] _ShadowMapMetal("Shadow Map Metallica (V)", 2D) = "white" {}

		[MainColor]_Color("Color", Color) = (1, 1, 1, 1)
		[MaterialToggle] _OverrideUV("Override UV", float) = 0
		_PaletteIndex ("Palette Index", Integer) = 0
		_PaletteSize ("Palette Size", Integer) = 0
	}

	SubShader
	{
		Tags { "RenderPipeline"="UniversalPipeline" }

		Pass
		{
			Name "Palette"
			Tags { "LightMode" = "UniversalForward" }

			HLSLPROGRAM

			#define _SPECULAR_COLOR
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile_fragment _ _SHADOWS_SOFT

			#pragma vertex Vertex
			#pragma fragment Fragment

			#include "PaletteForwardPass.hlsl"

			ENDHLSL
		}

		Pass
		{
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			HLSLPROGRAM

			#pragma vertex Vertex;
			#pragma fragment Fragment;

			#include "PaletteShadowCaster.hlsl"

			ENDHLSL
		}
	}
}