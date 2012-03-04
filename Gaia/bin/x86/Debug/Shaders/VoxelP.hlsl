struct PSIN_Voxel
{
    float4 Position : POSITION0;
    float3 Normal	: TEXCOORD0;
    float3 WorldPos : TEXCOORD1;
    float3 LocalPos : TEXCOORD2;
};

float4 main(PSIN_Voxel input, uniform sampler ColorSAMP0 : register(S0), uniform sampler NormalSAMP0 : register(S1), uniform sampler ColorSAMP1 : register(S2),
uniform sampler NormalSAMP1 : register(S3), uniform sampler ColorSAMP2 : register(S4), uniform sampler NormalSAMP2 : register(S5), uniform sampler ColorSAMP3 : register(S6),
uniform sampler NormalSAMP3 : register(S7), uniform sampler ColorSAMP4 : register(S8), uniform sampler NormalSAMP4 : register(S9), uniform sampler ClimateSAMP : register(S10)
) : COLOR
{
    float3 N = normalize(input.Normal);
	float3 T = N.zxy;
	float3x3 TBN = float3x3(T,cross(N,T), N);
	
	float3 blend_weights = 7*(abs(N.xyz)-0.2);   // Tighten up the blending zone:  
	blend_weights = pow(blend_weights,3);
	//blend_weights = (blend_weights - 0.06) * 3;  
	blend_weights = max(blend_weights, 0);      
	// Force weights to sum to 1.0 (very important!)  
	blend_weights /= (blend_weights.x + blend_weights.y + blend_weights.z );
	float3 TC = input.WorldPos*0.125;
	
	float4 col1 = (N.x < 0.0f)?pow(tex2D(ColorSAMP3, TC.zy),2.2):pow(tex2D(ColorSAMP4, TC.zy),2.2);
	float4 col2 = (N.y < 0.0f)? pow(tex2D(ColorSAMP1, TC.xz),2.2) : pow(tex2D(ColorSAMP0, TC.xz),2.2);//pow(tex2D(ColorSAMP0, TC.xz),2.2);
	float4 col3 = pow(tex2D(ColorSAMP2, TC.xy),2.2);
	
	float3 n1 = (N.x < 0.0f)?tex2D(NormalSAMP3, TC.zy)*2-1:tex2D(NormalSAMP4, TC.zy)*2-1;
	float3 n2 = ((N.y < 0.0f)?tex2D(NormalSAMP1, TC.xz):tex2D(NormalSAMP0, TC.xz))*2-1;
	float3 n3 = tex2D(NormalSAMP2, TC.xy)*2-1;

	// Finally, blend the results of the 3 planar projections.  
	float4 blendColor = col1.xyzw * blend_weights.x +  
                col2.xyzw * blend_weights.y +  
                col3.xyzw * blend_weights.z;  
	float3 blendBump = n1.xyz * blend_weights.x +  
                   n2.xyz * blend_weights.y +  
                   n3.xyz * blend_weights.z;
    N = normalize(mul(blendBump, TBN)); 
    float3 climate = pow(tex2D(ClimateSAMP, float2(input.LocalPos.y*0.5+0.5, 0.5)), 2.2);
    float4 outColor = blendColor*(max(dot(N, normalize(float3(0,0.6,1))),0)+0.06)*float4(climate,1);
    outColor.rgb = pow(outColor.rgb,1.0/2.2);
    return outColor;
}