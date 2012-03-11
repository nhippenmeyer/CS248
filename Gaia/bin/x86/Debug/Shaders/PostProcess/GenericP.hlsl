float4 main(float2 TexCoord : TEXCOORD0, uniform sampler BaseMap : register(S0)
) : COLOR
{
	return tex2D(BaseMap, TexCoord);
}