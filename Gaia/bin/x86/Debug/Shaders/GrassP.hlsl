#include "Prepass.h"
#include "ShaderConst.h"

struct PSIN
{
    float4 Position : POSITION0;
    float3 Normal	: TEXCOORD0;
    float3 WorldPos : TEXCOORD1;
    float3 LocalPos : TEXCOORD2;
};

GBUFFER main(PSIN IN, uniform sampler BaseMap : register(S0),
			uniform sampler NormalMap : register(S1), 
			uniform float4 EyePos : register(PC_EYEPOS)
) : COLOR
{
	GBUFFER OUT;
    float3 N = float3(0,1,0);
    float4 brownColor = float4(0.2, 0.21, 0.1, 1);
    float4 greenColor = float4(0, 1, 0, 1);
	
    OUT.Color = lerp(brownColor, greenColor, IN.LocalPos.y);
    OUT.Normal = float4(CompressNormal(N),0,0);
    OUT.Depth = length(IN.WorldPos-EyePos.xyz)/EyePos.w;
    OUT.Data = 0;
    return OUT;
}