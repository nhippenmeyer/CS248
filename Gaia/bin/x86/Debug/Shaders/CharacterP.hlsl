#include "Prepass.h"
#include "ShaderConst.h"

struct PSIN
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal	: TEXCOORD1;
    float3 Tangent  : TEXCOORD2;
    float3 WorldPos : TEXCOORD3;
};

GBUFFER main(PSIN input, uniform sampler BaseMap : register(S0), uniform sampler NormalMap : register(S1),
uniform float4 EyePos : register(PC_EYEPOS)
) : COLOR
{
	GBUFFER OUT;
    float3 N = normalize(input.Normal);
	float3 T = normalize(input.Tangent);
	float3x3 TBN = float3x3(T,cross(N,T), N);
	N = normalize(mul(tex2D(NormalMap, input.TexCoord)*2-1, TBN);
    OUT.Color = tex2D(BaseMap, input.TexCoord);
    OUT.Normal = float4(CompressNormal(N),0,0);
    OUT.Depth = length(input.WorldPos-EyePos.xyz)/EyePos.w;
    OUT.Data = 0;
    return OUT;
}