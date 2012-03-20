struct PSIN
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

PSIN main(float4 Position : POSITION0, float2 TexCoord : TEXCOORD0, uniform float4 MinMax : register(C0))
{
    PSIN OUT;

	OUT.Position = Position;
	OUT.Position.xy = Position * MinMax.xy + MinMax.zw;
    OUT.TexCoord = TexCoord;
    return OUT;
}
