#include "../ShaderConst.h"
#define NUM_WAVES 4
#define K 1.93333
struct PSIN
{
    float4 Position : POSITION0;
    float4 TexCoord : TEXCOORD0;
    float4 WorldPos	: TEXCOORD1;
    float3 Normal	: TEXCOORD2;
    float3 Tangent	: TEXCOORD3;
};

PSIN main(float4 Position : POSITION0, uniform float4x4 ModelView : register(VC_MODELVIEW),
			uniform float4x4 World : register(VC_WORLD),
			uniform float4 EyePos :register(VC_EYEPOS),
			uniform float4 WaveDirs[NUM_WAVES] : register(C4),
			uniform float waterScale : register(C33),
			uniform float Amplitude[NUM_WAVES] : register(C34),
			uniform float Time : register(VC_TIME)
)
{
    PSIN OUT;
    
    float2 derivs = 0;
    float height = 0;
    float2 offsetPos = Position.xz+EyePos.xz/waterScale;
    for(int i = 0; i < NUM_WAVES; i++)
    {
		float dir = dot(offsetPos, WaveDirs[i].xy);
		float wavePos = WaveDirs[i].w*dir+WaveDirs[i].z*WaveDirs[i].w*Time;		
		float2 derivWaveDir = WaveDirs[i].w*Position.xz*WaveDirs[i].xy;
		
		float sinPos = sin(wavePos);
		float cosPos = cos(wavePos);
		height += Amplitude[i]*pow(sinPos, K);
		derivs += K*Amplitude[i]*pow(sinPos, K-1)*cosPos*derivWaveDir;
    }
    height /= NUM_WAVES;
    derivs /= NUM_WAVES;

    OUT.Tangent = normalize(float3(0,derivs.y,1));
    OUT.Normal = normalize(float3(-derivs.x, 1, -derivs.y));
	float4 worldPos = mul(float4(Position.xyz+float3(0,height,0),1.0f), World);
    OUT.Position = mul(worldPos, ModelView);
    float atten = saturate(pow(1.0/length(Position.xz),2.2)-1);
    OUT.WorldPos = float4(worldPos.xyz, atten);
    
	
	//Projective texture mapping
	float4x4 texMat = { 0.5,  0.0,  0.0,  0.5,
    0.0, -0.5,  0.0,  0.5,
    0.0,  0.0,  1.0,  0.0,
    0.0,  0.0,  0.0,  1.0 };
    OUT.TexCoord = mul(texMat, OUT.Position);
    return OUT;
}
