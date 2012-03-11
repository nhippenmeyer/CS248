struct PSIN
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 RandCoord: TEXCOORD1;
};

float4 main(PSIN IN, uniform sampler PositionMap : register(S0),
			uniform float4x4 viewProjection : register(C0),
			uniform float particleSize : register(C4)
) : COLOR
{
	float4 position = float4(tex2D(PositionMap, IN.TexCoord).xyz, 1.0f);
	float4 ssPos = mul(position, viewProjection);
	ssPos /= ssPos.w;
	return lerp(particleSize, 64, saturate(ssPos.z*0.5+0.5));
}