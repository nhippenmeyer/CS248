#define PI 3.141592653589793
//-----------------------------
// Lighting Functions
//-----------------------------
float Chebyshev(float2 Moment, float t)
{
	float minV = 0.00003f;
	float p = (t <= Moment.x);
	float V = Moment.y - dot(Moment.x,Moment.x);
	V = max(V, minV);
	float d = t-Moment.x;
	float pMax = V / (V+d*d);
	return max(p,pMax);

}

half cos2tan2( float x ) { return (1-x*x)/(x*x); }

half BeckmannDist( float dotNH, float SpecularExponent )
{
    half invm2 = SpecularExponent / 2;
    half D = exp( -cos2tan2( dotNH ) * invm2 ) / pow( dotNH, 4 ) * invm2;
    return D;
}