#include "../ShaderConst.h"

struct PSIN
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Direction: TEXCOORD1;
};

PSIN main(float4 Position : POSITION0, float2 TexCoord : TEXCOORD0, 
		  uniform float4x4 InvViewProj : register(VC_MODELVIEW), uniform float2 InvRes : register(VC_INVTEXRES))
{
    PSIN OUT;

    OUT.Position = Position;
    float4 p = mul(float4(Position.xy,1,1), InvViewProj);
    OUT.TexCoord = TexCoord + 0.5*InvRes;
    OUT.Direction = p.xyz;
    return OUT;
}
