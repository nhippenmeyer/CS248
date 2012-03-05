#define PI 3.141592653589793
//-----------------------------
// Lighting Functions
//-----------------------------
float Chebyshev(float t, float2 Moment)
{
   float p = (t <= Moment.x);
   float V = Moment.y - dot(Moment.x,Moment.x);
   V = max(V, 0.001f);
   float d = t-Moment.x;
   float pMax = V / (V+d*d);
   return smoothstep(0,1,max(p,pMax));

}

half cos2tan2( float x ) { return (1-x*x)/(x*x); }

half BeckmannDist( float dotNH, float SpecularExponent )
{
    half invm2 = SpecularExponent / 2;
    half D = exp( -cos2tan2( dotNH ) * invm2 ) / pow( dotNH, 4 ) * invm2;
    return D;
}