#define EPS 0.00001

float2 CompressNormal( float3 N )
{
   float scale = 1.7777;
   float2 enc = N.xy / (N.z+1);
   enc /= scale;
   enc = enc*0.5+0.5;
   return normalize(enc);
}

float3 DecompressNormal(float2 G)
{
   float scale = 1.7777;
   float3 nn = float3(G,0)*float3(2*scale,2*scale,0) + float3(-scale,-scale,1);
   float g = 2.0 / dot(nn.xyz,nn.xyz);
   float3 n;
   n.xy = g*nn.xy;
   n.z = g-1;
   return normalize(n);
}

struct GBUFFER
{
	float4 Color		: COLOR0;
	float4 Normal		: COLOR1;
	float4 Depth		: COLOR2;
	float4 Data			: COLOR3;
};