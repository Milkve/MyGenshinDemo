// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Shadertoy/StarField" {
	Properties {

		iMouse ("Mouse Pos", Vector) = (0, 0, 0, 0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass{
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma fragmentoption ARB_precision_hint_fastest   
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag
 
		#define vec2 float2
		#define vec3 float3
		#define vec4 float4
		#define mat2 float2x2		
		#define mat3 float3x3
		#define mat4 float4x4
		#define iGlobalTime _Time.y
		#define iTime _Time.y
		#define mod fmod
		#define mix lerp
		#define fract frac
		#define Texture2D tex2D
		#define iResolution _ScreenParams
 ///////////////////////
		#define iterations 17
#define formuparam 0.53

#define volsteps 20
#define stepsize 0.1

#define zoom   0.800
#define tile   0.850
#define speed  0.001 

#define brightness 0.0015
#define darkmatter 0.300
#define distfading 0.730
#define saturation 0.850

#define PI (4.0 * atan(1.0))
#define TWO_PI PI*2.


#define HASHSCALE1 443.8975

		fixed4 iMouse;


float hash11(float p) {
    vec3 p3 = fract(vec3(p,p,p) * HASHSCALE1);
    p3 += dot(p3, p3.yzx + 19.19);
    return fract((p3.x + p3.y) * p3.z);
}

float lerp2(float a, float b, float t) {
    return a + t * (b - a);
}

float noise(float p) {
    float i = floor(p);
    float f = fract(p);
    float t = f * f * (3.0 - 2.0 * f);
    return lerp(f * hash11(i), (f - 1.0) * hash11(i + 1.0), t);
}
float fbm(float x, float persistence, int octaves) {
    float total = 0.0;
    float maxValue = 0.0;
    float amplitude = 1.0;
    float frequency = 1.0;
    for (int i = 0; i < octaves; ++i) {
        total += noise(x * frequency) * amplitude;
        maxValue += amplitude;
        amplitude *= persistence;
        frequency *= 2.0;
    }
    return (total / maxValue);
}
float msine2(vec2 uv) {
    return (fbm(uv.x / 10., 0.25, 4)*20. + 0.5);
}


float xRandom(float x) {
    return mod(x * 7241.6465 + 2130.465521, 64.984131);
}


float shape2(vec2 uv, int N, float radius_in, float radius_out, float zoom3) {
    float color = 0.0;
    float d = 0.0;
    float a = atan2(uv.y, uv.x) + PI;
    float rx = TWO_PI / float(N);
    d = cos(floor(.5 + a / rx) * rx - a) * length(uv);
    color = smoothstep(.44, .44 + (2. + 1.2 * zoom3) / iResolution.y, abs(d - radius_in) + radius_out);
    return (1. - color);
}


float msine(vec2 uv) {
    float heightA = 0.025;
    float heightB = 0.025;
    float heightC = 0.013; //+0.071*sin(iTime/105.); //xD
    uv.y = sin((uv.x + (1.))*5.0) * heightA;
    uv.y = uv.y + sin((uv.x + (0. / 5.))*3.0) * heightB;
    uv.y = uv.y + sin((uv.x + (1.))*2.0) * heightC;
    return uv.y;
}


vec3 remap(vec3 ori ,vec3 start,vec3 end){
    ori*=(end-start);
    ori+=start;
    return ori;
}


////////////////////
 
		struct v2f{
			float4 pos:SV_POSITION;
			float4 srcPos:TEXCOORD0;
		};
 
		v2f vert(appdata_base v){
			v2f o;
			o.pos=UnityObjectToClipPos(v.vertex);
			o.srcPos=ComputeScreenPos(o.pos);
			return o;
		}
		vec4 main(vec2 fragCoord);
		float4 frag(v2f iParam):COLOR{
			//获取uv对应的当前分辨率下的点   uv范围（0-1） 与分辨率相乘
			vec2 fragCoord=((iParam.srcPos.xy/iParam.srcPos.w)*_ScreenParams.xy);
			return main(fragCoord);
		}
		//要先定义方法声明才能使用

		vec4 main(vec2 fragCoord){
			vec2 uv=fragCoord.xy/iResolution.xy-.5;
	//uv.y*=iResolution.y/iResolution.x;
    
    // uv.y=abs(uv.y);
    
    vec2 res = iResolution.xy / iResolution.y;
     uv = (fragCoord.xy) / iResolution.y - res / 2.0;

    uv.y+=0.12;
    if (uv.y<0.) uv.x +=0.01*sin(100.*uv.y+iTime);//line from hamtarodeluxe
    uv.y=-abs(uv.y);
    
	vec3 dir=vec3(uv*zoom,1.);
	float time=iTime*speed+.25;

	//mouse rotation
	float a1=.5+iMouse.x/iResolution.x/20.;
	float a2=.8+iMouse.y/iResolution.y/20.;
	mat2 rot1=mat2(cos(a1),sin(a1),-sin(a1),cos(a1));
	mat2 rot2=mat2(cos(a2),sin(a2),-sin(a2),cos(a2));
	dir.xz=mul(dir.xz,rot1);
	dir.xy=mul(dir.xy,rot2);
	vec3 from=vec3(1.,.5,0.5);
	from+=vec3(time*2.,time,-2.);
	from.xz=mul(from.xz,rot1);
	from.xy=mul(from.xy,rot1);
	
	//volumetric rendering
	float s=0.1,fade=1.;
	vec3 v=vec3(0.,0.,0.);
	for (int r=0; r<volsteps; r++) {
		vec3 p=from+s*dir*.5;
		p = abs(vec3(tile,tile,tile)-mod(p,vec3(tile*2.,tile*2.,tile*2.))); // tiling fold
		float pa,a=pa=0.;
		for (int i=0; i<iterations; i++) { 
			p=abs(p)/dot(p,p)-formuparam; // the magic formula
			a+=abs(length(p)-pa); // absolute sum of average change
			pa=length(p);
		}
		float dm=max(0.,darkmatter-a*a*.001); //dark matter
		a*=a*a; // add contrast
		if (r>6) fade*=1.-dm; // dark matter, don't render near
		//v+=vec3(dm,dm*.5,0.);
		v+=fade;
		v+=vec3(s,s*s,s*s*s*s)*a*brightness*fade; // coloring based on distance
		fade*=distfading; // distance fading
		s+=stepsize;
	}
	//v=layer_bghills(uv)*mix(vec3(length(v)),v,saturation); //color adjust
		return vec4(remap(v*.01,vec3(0.38,0.38,0.77),vec3(0.6,0.6,0.95))+uv.y*0.6,1.);	
		}
 
 
 
		ENDCG
		}
	}
	FallBack "Diffuse"
}