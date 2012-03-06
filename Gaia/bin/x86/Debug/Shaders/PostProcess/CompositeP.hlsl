#include "../Prepass.h"
float4 main(float2 TexCoord : TEXCOORD0, uniform sampler ColorMap : register(S0),
			uniform sampler LightMap : register(S1), uniform sampler DepthMap : register(S2) ) : COLOR
{
    float2 TC = TexCoord.xy;
    clip(tex2D(DepthMap, TC).r-EPS);
    float4 finalColor = (tex2D(DepthMap, TC).r-EPS > 0)?1:0;
    finalColor.rgb = tex2D(ColorMap, TC).rgb*(tex2D(LightMap, TC).rgb+0.35);
	return finalColor;
}