float4 main(float2 TexCoord : TEXCOORD0, uniform sampler BaseMap : register(S0),
			uniform float2 InvRes : register(C0)
) : COLOR
{
    float4 finalColor = 0;
    float sum = 0;
    for(float i = -2.5; i <= 2.5; i++)
    {
		float2 offset = float2(0,i)*InvRes;
		float4 C = tex2D(BaseMap, TexCoord+offset);
		finalColor += C;
		sum++;
	}
	finalColor /= sum;
    return finalColor;
}