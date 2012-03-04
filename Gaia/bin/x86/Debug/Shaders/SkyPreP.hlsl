#define InnerRadius 6356.7523142
#define OuterRadius 6356.7523142 * 1.0157313
#define PI 3.1415159
#define PI4 12.5663706
#define NumSamples 50

float3 HDR( float3 LDR, float exposure)
{
   return 1.0f - exp( exposure * LDR );
}

float getRayleighPhase(float fCos2)
{
   return 0.75 * (1.0 + fCos2);
}

float HitOuterSphere( float3 O, float3 Dir ) 
{
   float3 L = -O;

   float B = dot( L, Dir );
   float C = dot( L, L );
   float D = C - B * B; 
   float q = sqrt( OuterRadius * OuterRadius - D );
   float t = B;
   t += q;

   return t;
}

float2 GetDensityRatio( float fHeight, float2 HeightParams)
{
   float fScale = 1.0 / (OuterRadius - InnerRadius);
   const float fAltitude = (fHeight - InnerRadius) * fScale;
   return exp( -fAltitude / (HeightParams.xy*100) );
}

float2 t( float3 P, float3 Px, float2 HeightParams)
{
   float fScale = 1.0 / (OuterRadius - InnerRadius);
   float2 OpticalDepth = 0;

   float3 v3Vector =  Px - P;
   float fFar = length( v3Vector );
   float3 v3Dir = v3Vector / fFar;
         
   float fSampleLength = fFar / NumSamples;
   float fScaledLength = fSampleLength * fScale;
   float3 v3SampleRay = v3Dir * fSampleLength;
   P += v3SampleRay * 0.5f;
         
   for(int i = 0; i < NumSamples; i++)
   {
      float fHeight = length( P );
      OpticalDepth += GetDensityRatio( fHeight, HeightParams);
      P += v3SampleRay;
   }      

   OpticalDepth *= fScaledLength;
   return OpticalDepth;
}

float4 main(float3 Direction : TEXCOORD0, uniform float3 InvWavelength : register(C0),
			uniform float3 WavelengthMie : register(C1), uniform float2 HeightParams : register(C2), 
			uniform float3 LightPosition : register(C3), uniform float fExposure : register(C4)
) : COLOR
{
    float fScale = 1.0 / (OuterRadius - InnerRadius);
    float KrESun = HeightParams.x * 12.0f;
    float KmESun = HeightParams.y * 6.0f;
    float Kr4PI = HeightParams.x * PI4;
    float Km4PI = HeightParams.y * PI4;
	const float3 v3PointPv = float3( 0, InnerRadius + 1e-3, 0 );
	float3 v3Dir = normalize(Direction);
	float fFarPvPa = HitOuterSphere( v3PointPv, v3Dir );
	float3 v3Ray = v3Dir;
	float3 v3SunDir = normalize(LightPosition);
	float3 v3PointP = v3PointPv;
	float fSampleLength = fFarPvPa / NumSamples;
	float fScaledLength = fSampleLength * fScale;
	float3 v3SampleRay = v3Ray * fSampleLength;
	v3PointP += v3SampleRay * 0.5f;
	        
	float3 v3RayleighSum = 0;
	float3 v3MieSum = 0;

	for( int k = 0; k < NumSamples; k++ )
	{
		float PointPHeight = length( v3PointP );

		float2 DensityRatio = GetDensityRatio( PointPHeight, HeightParams);
		DensityRatio *= fScaledLength;

		float2 ViewerOpticalDepth = t( v3PointP, v3PointPv, HeightParams);
		          
		float dFarPPc = HitOuterSphere( v3PointP, v3SunDir );
		float2 SunOpticalDepth = t( v3PointP, v3PointP + v3SunDir * dFarPPc, HeightParams);

		float2 OpticalDepthP = SunOpticalDepth.xy + ViewerOpticalDepth.xy;
		float3 v3Attenuation = exp( - Kr4PI * InvWavelength * OpticalDepthP.x - Km4PI * OpticalDepthP.y );

		v3RayleighSum += DensityRatio.x * v3Attenuation;
		v3MieSum += DensityRatio.y * v3Attenuation;

		v3PointP += v3SampleRay;
	}

	float3 RayLeigh = v3RayleighSum * KrESun;
	float3 Mie = v3MieSum * KmESun;
	RayLeigh *= InvWavelength;
	Mie *= WavelengthMie;
	float fCos = dot(v3SunDir, -v3Dir);
	float fCos2 = fCos * fCos;
    return float4(HDR(getRayleighPhase(fCos2) * RayLeigh + Mie, fExposure),1);
}