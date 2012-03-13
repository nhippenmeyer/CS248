#define FBias 0.11
#define FPower 6
#define SpecPower 28.72000
uniform sampler RefractMap : register(S0);
uniform sampler ReflectMap : register(S3);

float fresnel(float NdotV, float bias, float power)
{
   return bias + (1.0-bias)*pow(1.0 - max(NdotV, 0), power);
}

float4 ComputeWater(float3 N, float3 V, float3 L, float2 TC, float density, float refractCoeff)
{
	const float3 waterColor = float3(0.1288, 0.5429, 0.8631);
	//const float3 waterColor = float3(0.2041,0.8631,0.7588);
	const float3 reflectColor = float3(0.1686, 0.1688, 0.2593);
	const float3 NPlane = float3(0,1,0);
    float3 R = normalize(2*dot(N,-V)*N-V);
    float2 distCrd = N.xz*refractCoeff;
    TC += distCrd;
    
    float3 ReflMap = tex2D(ReflectMap, TC);
    float3 ReflTerm = ReflMap;//lerp(reflectColor, ReflMap, saturate(dot(-V,N)));
    float3 RefrCol = tex2D(RefractMap,TC);
    
    float3 coeff = -log(2)/waterColor; 
 
    float3 DiffTerm = RefrCol*exp(coeff*min(density,100)*0.01);//lerp(waterColor, RefrCol, density);
   
    float fTerm = fresnel(dot(V,NPlane), FBias, FPower);
    
    float spec = pow(max(dot(normalize(L+V), N),0.0), SpecPower);
   
    float4 finalColor = 1;
    finalColor.rgb = lerp(DiffTerm, ReflTerm, fTerm)+spec;
    
	return finalColor;
}