#include "../ShaderConst.h"
#include "../Prepass.h"
#include "Water.h"
float4 main(float2 TexCoord : TEXCOORD0, float3 Direction : TEXCOORD1, 
			uniform sampler BaseMap : register(S0), uniform sampler DepthMap : register(S1),
			uniform sampler NormalMap : register(S2), 
			uniform float4 BumpTC[4] : register(C0),
			uniform float waterScale : register(C4),
			uniform float time : register(PC_TIME),
			uniform float4 EyePos : register(PC_EYEPOS),
			uniform float3 L : register(PC_LIGHTPOS)
) : COLOR
{
	float t = -EyePos.y/Direction.y;
	clip(t);
	
	float2 TC = TexCoord.xy;
    float z = tex2D(DepthMap, TC).r*EyePos.w;
    if(z <= EPS)
		z = EyePos.w;
				
	float3 V = normalize(Direction);
    float3 worldPos = V*t+EyePos.xyz;
    float3 wpScene = V*z+EyePos.xyz;
	
	if(z-t+2*EPS < 0)
		clip(worldPos.y-wpScene.y);
		
	float atten = 1.0f-saturate(1.0f-length(EyePos.xz - worldPos.xz)/waterScale);
	clip(atten-EPS);
        
    float3 N = tex2D(NormalMap, worldPos.xz*BumpTC[0].xy+BumpTC[0].zw*time)*2-1;
    N += tex2D(NormalMap, worldPos.xz*BumpTC[1].xy+BumpTC[1].zw*time)*2-1;
    N += tex2D(NormalMap, worldPos.xz*BumpTC[2].xy+BumpTC[2].zw*time)*2-1;
    N += tex2D(NormalMap, worldPos.xz*BumpTC[3].xy+BumpTC[3].zw*time)*2-1;
    N *= 0.25;
    N = normalize(float3(N.y,1,N.x));
    
    float densityFalloff = saturate(abs(worldPos.y-wpScene.y)/10);
    float density = abs(worldPos.y-wpScene.y);//lerp(0.81769, 0.1, densityFalloff);
    float refractCoeff = saturate(1.0-length(EyePos.xyz-worldPos)/50)+0.15;
    float4 finalColor = ComputeWater(N, -V, L, TC, density, refractCoeff);
    //finalColor.w = saturate(atten);
    return finalColor;
}