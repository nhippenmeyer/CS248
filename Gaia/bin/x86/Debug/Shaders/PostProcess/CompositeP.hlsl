#include "../ShaderConst.h"
#include "../Prepass.h"
float4 main(float2 TexCoord : TEXCOORD0, uniform sampler ColorMap : register(S0),
			uniform sampler LightMap : register(S1), uniform sampler DepthMap : register(S2), 
			uniform float4 EyePos : register(PC_EYEPOS)
) : COLOR
{
    float2 TC = TexCoord.xy;
    clip(tex2D(DepthMap, TC).r*EyePos.w-EPS);
    float4 finalColor = 1;
    finalColor.rgb = tex2D(ColorMap, TC).rgb*(tex2D(LightMap, TC).rgb+0.35);
	return finalColor;
}