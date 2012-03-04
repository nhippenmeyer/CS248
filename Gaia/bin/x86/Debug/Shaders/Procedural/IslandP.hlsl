float4 main(float3 TexCoord : TEXCOORD0, uniform sampler Noise0 : register(S0), 
			uniform sampler Noise1 : register(S1), uniform sampler Noise2 : register(S2),
			uniform sampler Noise3 : register(S3)
) : COLOR
{
	float3 wp = TexCoord * 2 - 1;
	wp.y += tex3D(Noise0, wp*0.0167)*1.35 + tex3D(Noise1, wp*0.097)*0.6 + tex3D(Noise2, wp*0.8)*0.0715;
	float fracTerm = floor(wp.y * 11)/10;
	float density = -wp.y-fracTerm*0.5;
	
    return saturate(density * 0.5 + 0.5); //Compress density to 0-1
}