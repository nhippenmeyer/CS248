float4 main(float2 TexCoord : TEXCOORD0, uniform sampler BaseMap : register(S0),
			uniform sampler RampMap : register(S1)
) : COLOR
{
	float4 finalColor = tex2D(BaseMap, TexCoord);
	finalColor.r = tex2D(RampMap, float2(finalColor.r,0.5)).r;
	finalColor.g = tex2D(RampMap, float2(finalColor.g,0.5)).g;
	finalColor.b = tex2D(RampMap, float2(finalColor.b,0.5)).b;
    
	return finalColor;
}