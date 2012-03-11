#include "../ShaderConst.h"

struct PSIN
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 RandCoord: TEXCOORD1;
};

float4 main(PSIN IN, uniform sampler PositionMap : register(S0),
			uniform sampler NoiseMap[3] : register(S1), 
			uniform float elapsedTime : register(C0),
			uniform float2 lifetimeParams : register(C1),
			uniform float4 BlendColors[MAX_PARTICLECOLORS] : register(PC_PARTICLECOLORS),
			uniform float BlendTimes[MAX_PARTICLECOLORS] : register(PC_PARTICLETIMES)
) : COLOR
{
	float time = lifetimeParams.x - tex2D(PositionMap, IN.TexCoord).w;
	float4 color = 0;
	int index = 0;
	for(int i = 0; i < MAX_PARTICLECOLORS; i++)
	{
		if(time < BlendTimes[i])
		{
			index = i;
			i = MAX_PARTICLECOLORS;
		}
	}
	int prevIndex = max(0, index-1);
	float amount = (time-BlendTimes[prevIndex])/(BlendTimes[index]-BlendTimes[prevIndex]);
	color = lerp(BlendColors[prevIndex], BlendColors[index], amount);
	return color;
}