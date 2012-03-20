#include "../ShaderConst.h"
#include "../Prepass.h"
struct PSIN
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float2 RandCoord: TEXCOORD1;
};

struct PHYSICS
{
	float4 Position : COLOR0;
	float4 Velocity : COLOR1;
};

struct State
{
	float3 position;
	float3 velocity;
};

struct Derivative
{
	float3 dx;
	float3 dv;
};

Derivative evaluate(State initial, float3 acceleration, float dt, Derivative d)
{
	State state;
	state.position = initial.position + d.dx*dt;
	state.velocity = initial.velocity + d.dv*dt;
	Derivative output;
	output.dx = state.velocity;
	output.dv = acceleration;
	return output;
}

State Integrate(State state, float3 accel, float timeDT)
{
	Derivative orig;
	orig.dx = 0;
	orig.dv = 0;
	Derivative a = evaluate(state, accel, 0, orig);
	Derivative b = evaluate(state, accel, timeDT*0.5f, a);
	Derivative c = evaluate(state, accel, timeDT*0.5f, b);
	Derivative d = evaluate(state, accel, timeDT, c);

	float3 dxdt = 1.0f/6.0f * (a.dx + 2.0f*(b.dx + c.dx) + d.dx);
	float3 dvdt = 1.0f/6.0f * (a.dv + 2.0f*(b.dv + c.dv) + d.dv);
	
	State newState;
	newState.position = state.position + dxdt*timeDT;
	newState.velocity = state.velocity + dvdt*timeDT;
	return newState;
}

/**
 * This uses the following data structure for particles:
 * emitOrigin - contains the position where particles will spawn from
 * lifetimeParams - used to tell when particles must die
 * dataParams0 - contains initialAxis and randomInitSpeed
 * dataParams1 - contains initialSpeed, initialSpeedVariance
*/
PHYSICS main(PSIN IN, uniform sampler PositionMap : register(S0),
			uniform sampler VelocityMap : register(S1),
			uniform sampler DepthMap : register(S2),
			uniform sampler NormalMap : register(S3),
			uniform sampler RandomMap[3] : register(S4),
			uniform float4 emitOrigin : register(C0),
			uniform float4 lifetimeParams : register(C1),
			uniform float4 dataParams0 : register(C2),
			uniform float4 dataParams1 : register(C3),
			uniform float timeDT : register(C4),
			uniform float4 randOffsetParams : register(C5),
			uniform float3x3 rotationMatrix : register(C6),
			uniform float3 forces[MAX_PARTICLEFORCES] : register(C9),
			uniform float4 eyePos : register(PC_EYEPOSPHYSICS),
			uniform float4x4 viewMatrix : register(PC_VIEWMATRIXPHYSICS)
) : COLOR
{
	PHYSICS OUT;
	float4 position = tex2D(PositionMap, IN.TexCoord);
	float4 velocity = tex2D(VelocityMap, IN.TexCoord); //w contains mass
	
	if(position.w >= lifetimeParams.x+timeDT && dataParams1.z < 0.5f) //Uh oh! Gotta reset the particle!
	{
		float3 randOffset;
		randOffset.x = tex2D(RandomMap[2], IN.RandCoord*2.6).r;
		randOffset.y = tex2D(RandomMap[0], IN.RandCoord*1.25).r;
		randOffset.z = tex2D(RandomMap[1], IN.RandCoord*1.89).r;
		position.xyz = emitOrigin.xyz + randOffset*mul(randOffsetParams.xyz, rotationMatrix)*randOffsetParams.w;
		float3 randDir;
		randDir.x = tex2D(RandomMap[0], IN.RandCoord*2.6).r;
		randDir.y = tex2D(RandomMap[1], IN.RandCoord*1.25).r;
		randDir.z = tex2D(RandomMap[2], IN.RandCoord*1.89).r;
		position.w = 0;// + lifetimeParams.y * tex2D(RandomMap[0], IN.RandCoord).r;
		velocity.xyz = mul(dataParams0.xyz, rotationMatrix) * dataParams1.x + dataParams1.y * tex2D(RandomMap[1], IN.RandCoord).r;
		velocity.xyz += randDir*dataParams0.w;
		velocity.w = lifetimeParams.z + lifetimeParams.w*tex2D(RandomMap[2], IN.RandCoord).r;
	}
		
	State state;
	state.position = position.xyz;
	state.velocity = velocity.xyz;
	
	float4 tcProj = mul(float4(position.xyz,1.0f), viewMatrix);
	tcProj /= float4(1,-1,1,1)*tcProj.w; 
	tcProj.xy = tcProj.xy * 0.5 + 0.5;
	
	if(tcProj.x >= 0.0 && tcProj.x <= 1.0 && tcProj.y >= 0.0 && tcProj.y <= 1.0)
	{
		float3 V = normalize(eyePos.xyz - position.xyz);// - eyePos.xyz);
		
		float depth = tex2D(DepthMap, tcProj.xy).r*eyePos.w;
		if(depth > EPS)
		{
			float3 worldPos = V*depth+eyePos.xyz;
			
			float3 normal = DecompressNormal(tex2D(NormalMap, tcProj).xy);
			
			if(length(eyePos.xyz-position.xyz) >= depth)
			{
				velocity.xyz = reflect(velocity.xyz, normal);
				state = Integrate(state, 0, timeDT);
			}
			/*
			float d = -dot(worldPos, normal);
			
			float3 vP = normalize(velocity.xyz);
			
			float t = (-dot(position.xyz,normal)+d)/dot(normal, vP);
			
			if(t <= length(velocity.xyz*timeDT))
			{
				velocity.xyz = -velocity.xyz;//reflect(velocity.xyz, normal);
				state = Integrate(state, 0, timeDT);
			}
			*/
			
		}
	
	}
		
	float3 netForce = 0;
	for(int i = 0; i < MAX_PARTICLEFORCES; i++)
	{
		netForce += forces[i];
	}
	float3 accel = netForce / velocity.w; //A = F/M
	
	state = Integrate(state, accel, timeDT);
	position.xyz = state.position;
	velocity.xyz = state.velocity;
	position.w += timeDT;
	
	OUT.Position = position;
	OUT.Velocity = velocity;
	
    return OUT;
}