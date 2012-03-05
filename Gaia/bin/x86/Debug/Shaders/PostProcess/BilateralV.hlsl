float4 main(float2 TexCoord : TEXCOORD0, uniform sampler BaseMap : register(S0),
			uniform float regGauss : register(C0), uniform float regData : register(C1)
) : COLOR
{
	float4 Color = tex2D(BaseMap, TexCoord);
    float4 finalColor = 0;
    float sum = 0;
    for(float i = -2.5; i <= 2.5; i++)
    {
		float2 offset = float2(0,i)*InvRes;
		float4 C = tex2D(BaseMap, TexCoord+offset);
		float4 diff = C-Color;
		float weight = exp(-(i*i)*regGauss)*exp(-dot(diff,diff)*regData);
		finalColor += weight*C;
		sum += weight;
	}
	finalColor /= sum;
    return finalColor;
}