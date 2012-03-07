#include "../ShaderConst.h"
#include "../Prepass.h"
#include "Lighting.h"

float4 main(float4 TexCoord : TEXCOORD0, float3 Direction : TEXCOORD1, uniform sampler GBuffer : register(S0), 
			uniform sampler DepthMap : register(S1), uniform sampler DataMap : register(S2),
			uniform sampler ShadowMap : register(S3),
			uniform float4x4 ViewMatrix : register(C0),
			uniform float3 LightColor : register(PC_LIGHTCOLOR), uniform float4 LightParams : register(PC_LIGHTPARAMS), 
			uniform float3 LightPos : register(PC_LIGHTPOS), uniform float4 EyePos : register(PC_EYEPOS),
			uniform float4x4 LightModelView[NUM_SPLITS] : register(PC_LIGHTMODELVIEW), 
			uniform float2 LightClipPlanes[NUM_SPLITS] : register(PC_LIGHTCLIPPLANE),
			uniform float2 InvShadowRes : register(PC_INVSHADOWRES)
			
) : COLOR
{
	float2 TC = TexCoord.xy / TexCoord.w;
	float z = tex2D(DepthMap, TC).r*EyePos.w;
	clip(z-EPS);
	float3 N = DecompressNormal(tex2D(GBuffer, TC).xy);
	float3 L = normalize(LightPos);
	float3 V = normalize(Direction);

	float3 WorldPos = V*z+EyePos.xyz;
	float4 viewPos = mul(float4(WorldPos,1),ViewMatrix);
	
	int currSplit = 0;
	for (int n = 1; n < NUM_SPLITS; n++)
	{	
		if (viewPos.z <= LightClipPlanes[n].x && viewPos.z > LightClipPlanes[n].y)
		{
			currSplit = n;
		}
	}
	
	float4 shadowProjPos = mul(float4(WorldPos,1), LightModelView[currSplit]);
	
	float2 shadowTC = 0.5 * shadowProjPos.xy / shadowProjPos.w + 0.5f;
    shadowTC.x = (shadowTC.x / (float)NUM_SPLITS) + ((float)currSplit / (float)NUM_SPLITS);
    shadowTC.y = 1.0f - shadowTC.y;
        
    // Offset the coordinate by half a texel so we sample it correctly
    shadowTC += 0.5 * InvShadowRes;
    
    float Depth = shadowProjPos.z/shadowProjPos.w-0.0035;
    float w = 0;
    float shade = 0;
    for(float i = -1.5; i <= 1.5; i++)
    {
		for(float j = -1.5; j <= 1.5; j++)
		{
			float2 tcShadow = shadowTC.xy+float2(i,j)*InvShadowRes;
			w++;
			shade += (Depth <= tex2D(ShadowMap, tcShadow).r)?1:0;
		}
    }
    
    shade = max(0.0, shade/w);
    
    float3 vSplitColors [4];
	vSplitColors[0] = float3(1, 0, 0);
	vSplitColors[1] = float3(0, 1, 0);
	vSplitColors[2] = float3(0, 0, 1);
	vSplitColors[3] = float3(1, 1, 0);
	
	
	float NDL = max(0.0, dot(N,L));
	float3 R = 2*N*NDL-L;
	float4 data = tex2D(DataMap, TC);
	float spec = pow(dot(R,V),data.r*255)*data.g;
	
	float4 finalColor = float4(NDL*LightColor, spec)*shade;

    return finalColor;
}