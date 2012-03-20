#define g -0.9998

float getMiePhase(float fCos, float fCos2)
{
	float g2 = g * g;
	float3 v3HG;
	v3HG.x = 1.5f * ( (1.0f - g2) / (2.0f + g2) );
	v3HG.y = 1.0f + g2;
	v3HG.z = 2.0f * g;
	return v3HG.x * (1.0 + fCos2) / pow(v3HG.y - v3HG.z * fCos, 1.5);
}

float4 main(float3 Direction : TEXCOORD0, float4 TexCoord : TEXCOORD1, 
			uniform sampler BaseMap : register(S0), uniform samplerCUBE NightMap : register(S1),
			uniform float2 InvRes : register(C0), uniform float3 LightPosition : register(C3)
) : COLOR
{
    float2 TC = TexCoord.xy / TexCoord.w + 0.5*InvRes;
    float3 v3SunDir = normalize(LightPosition);
    float3 v3Dir = normalize(Direction);
    float fCos = dot(v3SunDir, -v3Dir);
    float fCos2 = fCos * fCos;
    float3 finalColor = tex2D(BaseMap, TC) + getMiePhase(fCos, fCos2)*saturate(v3SunDir.y);
    if(v3SunDir.y < 0.0)
    {
		finalColor = lerp(tex2D(BaseMap, TC), texCUBE(NightMap, v3Dir), pow(-v3SunDir.y, 0.45));
    }

    return float4(finalColor,0);
}