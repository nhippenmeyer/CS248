#include "ShaderConst.h"

struct PSIN_Particle
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float2 IndexCoord : TEXCOORD1;
};

PSIN_Particle main(float4 Position : POSITION0, float2 TexCoord : TEXCOORD0, 
				uniform sampler2D PositionMap : register(S0),
				uniform float4x4 ModelView : register(VC_MODELVIEW)
)
{
    PSIN_Particle OUT;

	float4 worldPos = float4(Position.xyz,1.0);
    OUT.Position = mul(worldPos, ModelView);
	OUT.TexCoord = TexCoord;
	OUT.IndexCoord = TexCoord;
    return OUT;
}
