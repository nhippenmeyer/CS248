struct VS_Inst
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float  Index	: TEXCOORD1;
};

struct VS_COMMON
{
    float4 Position : POSITION0;
	float3 Normal	: NORMAL0;
	float3 Tangent	: TANGENT0;
    float2 TexCoord : TEXCOORD0;
};

struct PSIN
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct PSSHADOW
{
	float4 Position : POSITION0;
	float3 WorldPos : TEXCOORD0;
};