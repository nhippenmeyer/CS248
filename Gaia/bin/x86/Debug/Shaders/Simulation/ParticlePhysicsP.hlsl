#include "../ShaderConst.h"
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

/**
 * This uses the following data structure for particles:
 * emitOrigin - contains the position where particles will spawn from
 * lifetimeParams - used to tell when particles must die
 * dataParams0 - contains initialAxis and randomInitSpeed
 * dataParams1 - contains initialSpeed, initialSpeedVariance
*/
PHYSICS main(PSIN IN, uniform sampler PositionMap : register(S0),
			uniform sampler VelocityMap : register(S1),
			uniform sampler RandomMap[3] : register(S2),
			uniform float4 emitOrigin : register(C0),
			uniform float4 lifetimeParams : register(C1),
			uniform float4 dataParams0 : register(C2),
			uniform float4 dataParams1 : register(C3),
			uniform float timeDT : register(C4),
			uniform float4 randOffsetParams : register(C5),
			uniform float3 forces[MAX_PARTICLEFORCES] : register(C6)
) : COLOR
{
	PHYSICS OUT;
	float4 position = tex2D(PositionMap, IN.TexCoord);
	float4 velocity = tex2D(VelocityMap, IN.TexCoord); //w contains mass
	
	if(position.w <= 0.0) //Uh oh! Gotta reset the particle!
	{
		float3 randOffset;
		randOffset.x = tex2D(RandomMap[2], IN.RandCoord*2.6).r;
		randOffset.y = tex2D(RandomMap[0], IN.RandCoord*1.25).r;
		randOffset.z = tex2D(RandomMap[1], IN.RandCoord*1.89).r;
		position.xyz = emitOrigin.xyz + randOffset*randOffsetParams.xyz*randOffsetParams.w;
		float3 randDir;
		randDir.x = tex2D(RandomMap[0], IN.RandCoord*2.6).r;
		randDir.y = tex2D(RandomMap[1], IN.RandCoord*1.25).r;
		randDir.z = tex2D(RandomMap[2], IN.RandCoord*1.89).r;
		position.w = lifetimeParams.x + lifetimeParams.y * tex2D(RandomMap[0], IN.RandCoord).r;
		velocity.xyz = dataParams0.xyz * dataParams1.x + dataParams1.y * tex2D(RandomMap[1], IN.RandCoord).r;
		velocity.xyz += randDir*dataParams0.w;
		velocity.w = lifetimeParams.z + lifetimeParams.w*tex2D(RandomMap[2], IN.RandCoord).r;
	}
	
	float3 netForce = 0;
	for(int i = 0; i < MAX_PARTICLEFORCES; i++)
	{
		netForce += forces[i];
	}
	float3 accel = netForce / velocity.w; //A = F/M
	
	State state;
	state.position = position.xyz;
	state.velocity = velocity.xyz;
	Derivative orig;
	orig.dx = 0;
	orig.dv = 0;
	Derivative a = evaluate(state, accel, 0, orig);
	Derivative b = evaluate(state, accel, timeDT*0.5f, a);
	Derivative c = evaluate(state, accel, timeDT*0.5f, b);
	Derivative d = evaluate(state, accel, timeDT, c);

	float dxdt = 1.0f/6.0f * (a.dx + 2.0f*(b.dx + c.dx) + d.dx);
	float dvdt = 1.0f/6.0f * (a.dv + 2.0f*(b.dv + c.dv) + d.dv);
		
	//float3 Ia = accel*timeDT;	 //integral of acceleration
	position.xyz += dxdt*timeDT; //velocity.xyz*timeDT + 0.5*Ia*timeDT;
	velocity.xyz += dvdt*timeDT; //Ia; //update velocity term
	position.w -= timeDT;
	
	OUT.Position = position;
	OUT.Velocity = velocity;
	
    return OUT;
}