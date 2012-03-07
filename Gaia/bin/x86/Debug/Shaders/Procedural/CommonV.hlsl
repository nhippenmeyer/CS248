struct PSIN_Procedural
{
    float4 Position : POSITION0;
    float3 TexCoord : TEXCOORD0;
    float3 WorldPos : TEXCOORD1;
};

PSIN_Procedural main(float4 Position : POSITION0, float2 TexCoord : TEXCOORD0, 
					uniform float Depth : register(C0), uniform float4x4 World : register(C1)
)
{
    PSIN_Procedural OUT;

	OUT.Position = float4(Position.xyz,1.0f);
	OUT.TexCoord = float3(TexCoord, Depth);
	OUT.WorldPos = mul(float4(OUT.TexCoord*2-1,1), World);
    return OUT;
}
