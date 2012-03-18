#define NOISETEXCOUNT 4
#define OCTAVES 3

uniform sampler NoiseMaps[NOISETEXCOUNT] : register(S0);

float DensityFunction(float3 wp)
{
	float atten = saturate(1.0 - pow(length(dot(wp.xz,wp.xz))/0.9, 0.07));
	float yOrig = wp.y;
	
	wp *= 8;
	float3 warp = float3(tex3D(NoiseMaps[0], wp * 0.125). r,tex3D(NoiseMaps[1], wp * 0.0725).r,tex3D(NoiseMaps[2], wp * 0.09725).r);
	warp += float3(tex3D(NoiseMaps[0], wp * 4.0).r * 0.25, tex3D(NoiseMaps[1], wp * 4.0).r * 0.25, tex3D(NoiseMaps[2], wp * 4.0).r * 0.25);
	warp += float3(tex3D(NoiseMaps[0], wp * 2.0).r * 0.5, tex3D(NoiseMaps[1], wp * 2.0).r * 0.5, tex3D(NoiseMaps[2], wp * 2.0).r * 0.5);
	warp += float3(tex3D(NoiseMaps[0], wp * 0.5).r * 2.0, tex3D(NoiseMaps[1], wp * 0.5).r * 2.0, tex3D(NoiseMaps[2], wp * 0.5).r * 2.0);
	warp += float3(tex3D(NoiseMaps[0], wp * 0.025).r * 4.0, tex3D(NoiseMaps[1], wp * 0.25).r * 4.0, tex3D(NoiseMaps[2], wp * 0.25).r * 4.0);
	
	//float yInc = tex3D(NoiseMaps[0], wp*0.0467).r*0.805 + tex3D(NoiseMaps[1], wp*0.097).r*0.36 + tex3D(NoiseMaps[2], wp*0.8).r*0.0715;
	//yInc += float3(tex3D(NoiseMaps[0], wp * 40.0).r * 0.025, tex3D(NoiseMaps[1], wp * 40.0).r * 0.025, tex3D(NoiseMaps[2], wp * 40.0).r * 0.025);
	//yInc += float3(tex3D(NoiseMaps[0], wp * 20.0).r * 0.05, tex3D(NoiseMaps[1], wp * 20.0).r * 0.05, tex3D(NoiseMaps[2], wp * 20.0).r * 0.05);
	
	//wp.y += yInc;
	
	//float fracTerm = floor(wp.y * 11)/10;
	float density = -wp.y; //-fracTerm*0.35;	
	
	density += tex3D(NoiseMaps[0], wp * 4.03).r * 0.5;
	density += tex3D(NoiseMaps[1], wp * 1.96).r * 0.5;
	density += tex3D(NoiseMaps[2], wp * 1.01).r * 0.5;
	density += tex3D(NoiseMaps[0], wp * 0.54).r * 1;
	density += tex3D(NoiseMaps[1], wp * 0.24).r * 1;
	density += tex3D(NoiseMaps[2], wp * 0.127).r * 1;
	density += tex3D(NoiseMaps[0], wp * 0.06).r * 4;
	density += tex3D(NoiseMaps[1], wp * 0.034).r * 6;
	density += tex3D(NoiseMaps[2], wp * 0.016).r * 10;
	
	density = lerp(-yOrig, density*0.5+0.5, atten) + 0.02;
	
	return saturate(density); //Compress density to 0-1
}

float CreateMountains(float3 wp)
{
	float density = -wp.y + tex3D(NoiseMaps[0], (wp+0.23)*0.867).r*0.205;
	density += tex3D(NoiseMaps[2], wp*0.02867).r*0.805;
	
	return density*0.5+0.5;
}

struct PSIN_Procedural
{
    float4 Position : POSITION0;
    float3 TexCoord : TEXCOORD0;
    float3 WorldPos : TEXCOORD1;
};

float4 main(PSIN_Procedural IN, 
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
				float3 wp = IN.TexCoord + (float3(i,j,k)*InvRes);
				wp = wp * 2-1;
				density += DensityFunction(wp);
				sum++;
			}
		}
	}
	
	density /= sum;
	return density;
	
}