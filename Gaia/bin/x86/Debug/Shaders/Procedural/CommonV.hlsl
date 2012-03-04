struct PSIN_Procedural
{
    float4 Position : POSITION0;
    float3 TexCoord : TEXCOORD0;
};

PSIN_Procedural main(float4 Position : POSITION0, float2 TexCoord : TEXCOORD0, uniform float Depth : register(C0))
{
    PSIN_Procedural OUT;

	OUT.Position = float4(Position.xyz,1.0f);
	OUT.TexCoord = float3(TexCoord, Depth);
    return OUT;
}
