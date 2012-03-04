#include "ShaderConst.h"

struct PSIN_Sky
{
    float4 Position : POSITION0;
    float3 Direction: TEXCOORD0;
    float4 TexCoord : TEXCOORD1;
};

PSIN_Sky main(float4 Position : POSITION0, uniform float4x4 ModelView : register(VC_MODELVIEW))
{
    PSIN_Sky OUT;

    OUT.Position = mul(float4(Position.xyz,1.0f), ModelView);
	OUT.Direction = normalize(Position.xyz);
	
	//Projective texture mapping
	float4x4 texMat = { 0.5,  0.0,  0.0,  0.5,
    0.0, -0.5,  0.0,  0.5,
    0.0,  0.0,  1.0,  0.0,
    0.0,  0.0,  0.0,  1.0 };
    OUT.TexCoord = mul(texMat, OUT.Position);
    return OUT;
}
