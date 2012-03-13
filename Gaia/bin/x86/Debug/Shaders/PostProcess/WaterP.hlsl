#include "../ShaderConst.h"
#include "Water.h"
#include "../Prepass.h"
float4 main(float4 TexCoord : TEXCOORD0, float4 worldPos : TEXCOORD1, 
			float3 Normal : TEXCOORD2, float3 Tangent : TEXCOORD3,
			uniform sampler BaseMap : register(S0), uniform sampler DepthMap : register(S1),
			uniform sampler NormalMap : register(S2), 
			uniform float4 BumpTC[4] : register(C0),
			uniform float time : register(PC_TIME),
			uniform float4 EyePos : register(PC_EYEPOS),
			uniform float3 L : register(PC_LIGHTPOS)
) : COLOR
{
	float2 TC = TexCoord.xy/TexCoord.w;
    float z = tex2D(DepthMap, TC).r*EyePos.w;
    if(z == 0.0)
		z = EyePos.w;
		
	float t = length(EyePos.xyz-worldPos.xyz);
	clip(z-t+EPS);
    
    float3 V = normalize(EyePos.xyz-worldPos.xyz);
    float3 wpScene = V*z+EyePos.xyz;
    
    
    float3 T = normalize(Tangent);
    float3 N = normalize(Normal);
    float3x3 TBN = float3x3(T,cross(N,T), N);
    
    float3 Navg = tex2D(NormalMap, worldPos.xz*BumpTC[0].xy+BumpTC[0].zw*time)*2-1;
    Navg += tex2D(NormalMap, worldPos.xz*BumpTC[1].xy+BumpTC[1].zw*time)*2-1;
    Navg += tex2D(NormalMap, worldPos.xz*BumpTC[2].xy+BumpTC[2].zw*time)*2-1;
    Navg += tex2D(NormalMap, worldPos.xz*BumpTC[3].xy+BumpTC[3].zw*time)*2-1;
    Navg *= 0.25;
    N = normalize(mul(Navg, TBN));
    
    float densityFalloff = saturate(abs(worldPos.y-wpScene.y)/10);
    float density = abs(worldPos.y-wpScene.y);//lerp(0.81769, 0.1, densityFalloff);
    float refractCoeff = saturate(1.0-length(EyePos.xyz-worldPos)/50)+0.15;
    float4 outColor = ComputeWater(N, V, L, TC, density, refractCoeff);
    outColor.a = worldPos.w;
    return outColor;
}