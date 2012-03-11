uniform sampler Noise0 : register(S0);
uniform sampler Noise1 : register(S1); 
uniform sampler Noise2 : register(S2);
uniform sampler Noise3 : register(S3);

float DensityFunction(float3 wp)
{
	float3 warp = float3(tex3D(Noise0, wp * 0.125).r,tex3D(Noise1, wp * 0.0725).r,tex3D(Noise2, wp * 0.09725).r);
	wp.xyz += warp*0.15;
	wp.y += tex3D(Noise0, wp*0.0167)*0.705 + tex3D(Noise1, wp*0.097)*0.26 + tex3D(Noise2, wp*0.8)*0.0715;
	float fracTerm = floor(wp.y * 11)/10;
	float density = -wp.y-fracTerm*0.75;
	
	float atten = saturate(1.0-pow(length(dot(wp.xz,wp.xz)),15));
	
    return atten*saturate(density * 0.5 + 0.5); //Compress density to 0-1
}

float4 main(float3 TexCoord : TEXCOORD0, float3 WorldPos : TEXCOORD1, 
			uniform float3 InvRes : register(C0)
) : COLOR
{
	
	float density = 0;
	float sum = 0;
		
	for(float i = -3.5; i <= 3.5; i++)
	{
		for(float j = -3.5; j <= 3.5; j++)
		{
			for(float k = -3.5; k <= 3.5; k++)
			{
				float3 wp = (WorldPos*0.5+0.5) + (float3(i,j,k)*InvRes);
				density += DensityFunction(wp*2-1);
				sum++;
			}
		}
	}
	
	density /= sum;
	return density;
	
}