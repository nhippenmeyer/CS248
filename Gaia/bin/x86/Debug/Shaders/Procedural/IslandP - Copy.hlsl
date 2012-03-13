#define NOISETEXCOUNT 4
#define OCTAVES 3

uniform sampler NoiseMaps[NOISETEXCOUNT] : register(S0);

float DensityFunction(float3 wp)
{
	float atten = saturate(1.0-pow(length(dot(wp.xz,wp.xz))/0.95, 1.67428));
	float yOrig = wp.y;
	float3 warp = float3(tex3D(NoiseMaps[0], wp * 0.125).r,tex3D(NoiseMaps[1], wp * 0.0725).r,tex3D(NoiseMaps[2], wp * 0.09725).r);
	wp.xyz += warp*0.75;
	
	float yInc = tex3D(NoiseMaps[0], wp*0.0467).r*0.705 + tex3D(NoiseMaps[1], wp*0.097).r*0.26 + tex3D(NoiseMaps[2], wp*0.8).r*0.0715;
	
	wp.y += yInc;
	
	float fracTerm = floor(wp.y * 11)/10;
	float density = -wp.y-fracTerm*0.35;
	
	density = lerp(-yOrig, density*0.5+0.5, atten);
	
	/*
	float amp = 0.705*atten;
	float persist = 0.76;
	float3 freq = wp * 0.0167;
	int j = 0;
	for(int i = 0; i < OCTAVES; i++)
	{
		density += tex3D(NoiseMaps[j], freq).r*amp;
		freq *= 1.9875;
		amp *= persist;
		j++;
		if(j >= NOISETEXCOUNT)
			j = 0;
	}
	*/
		
    return saturate(density); //Compress density to 0-1
}

float4 main(float3 TexCoord : TEXCOORD0, float3 WorldPos : TEXCOORD1, 
			uniform float3 InvRes : register(C0)
) : COLOR
{
	
	float density = 0;
	float sum = 0;
		
	for(float i = -2.5; i <= 2.5; i++)
	{
		for(float j = -2.5; j <= 2.5; j++)
		{
			for(float k = -2.5; k <= 2.5; k++)
			{
				float3 wp = TexCoord + (float3(i,j,k)*InvRes);
				density += DensityFunction(wp*2-1);
				sum++;
			}
		}
	}
	
	density /= sum;
	return density;
	
}