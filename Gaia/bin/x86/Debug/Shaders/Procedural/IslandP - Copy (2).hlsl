#define NOISETEXCOUNT 4
#define OCTAVES 3

#define CYLINDERS 3

uniform sampler NoiseMaps[NOISETEXCOUNT] : register(S0);

const float3 cylinder[CYLINDERS] = {
	0.3, 0.3, 0.4,
	-0.6, -0.5, 0.4,
	0.1, -0.2, 0.5,
};
float DensityFunction(float3 wp)
{
	float3 warp = float3(tex3D(NoiseMaps[0], wp * 0.125).r,tex3D(NoiseMaps[1], wp * 0.0725).r,tex3D(NoiseMaps[2], wp * 0.09725).r);
	wp.xyz += warp*0.35;
	
	float density = -wp.y;
	density += saturate(cylinder[0].z/length(wp.xz-cylinder[0].xy) - 1);
	density += saturate(cylinder[1].z/length(wp.xz-cylinder[1].xy) - 1);
	density += saturate(cylinder[2].z/length(wp.xz-cylinder[2].xy) - 1);
	
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
				float3 wp = (WorldPos*0.5+0.5) + (float3(i,j,k)*InvRes);
				density += DensityFunction(wp*2-1);
				sum++;
			}
		}
	}
	
	density /= sum;
	return density;
	
}