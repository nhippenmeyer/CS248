#include "../ShaderConst.h"

float4 main(float4 TexCoord : TEXCOORD0, float3 Direction : TEXCOORD1, uniform sampler BaseMap : register(S0),
			uniform sampler DepthMap : register(S1), uniform float4 EyePos : register(PC_EYEPOS),
			uniform float4x4 CurrProj : register(C0), uniform float4x4 LastProj : register(C4)
) : COLOR
{
    float2 TC = TexCoord.xy;
	float4 color = tex2D(BaseMap, TC);
	float r = tex2D(DepthMap,TC).r*EyePos.w;
	if(r <= 0.0)
		r = EyePos.w;
	int sampCount = 32;
	if(r > 0.0)
	{
		float4 d =  float4(normalize(Direction)*r+EyePos.xyz,1);
		float4 currPos = mul(d, CurrProj);
		currPos.xy /= currPos.w;
		float4 lastPos = mul(d, LastProj);
		lastPos.xy /= lastPos.w;
		float2 velocity = (((currPos.xy-lastPos.xy)/2)/sampCount);
		for(int i = 0; i < sampCount; i++)  
		{  
			TC += velocity;
			// Sample the color buffer along the velocity vector.  
			color += tex2D(BaseMap, TC);  
		}
		color /= sampCount;
	}
	
    return color;
}