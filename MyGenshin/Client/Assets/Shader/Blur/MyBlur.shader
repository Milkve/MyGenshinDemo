Shader "Custom/MyBlur" 
{
 
	Properties
	{
		_MainTex("Main", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_BlurRadius("BlurRadius",Range(0,1))=0
		_ShowRadius("ShowRadius",Range(0,1))=0
		_Color("Color",Color)=(0,0,0,1)
		_Color1("Color1",Color)=(0,0,0,1)
		_Color2("Color2",Color)=(0,0,0,1)
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	
	struct v2f_blur
	{
		float4 pos : SV_POSITION; 
		float2 uv  : TEXCOORD0;	  
	};
	sampler2D _MainTex;
	sampler2D _Mask;
	float4 _MainTex_TexelSize;
	float _BlurRadius;
	float _ShowRadius;
	fixed4 _Color;
	fixed4 _Color1;
	fixed4 _Color2;
	fixed2 center=fixed2(0.5,0.5);
	fixed maxdis=0;
	v2f_blur vert_blur(appdata_img v)
	{
		v2f_blur o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		fixed2 upper=fixed2(0,0);
		maxdis = distance(center,upper);
		return o;
	}
 
	fixed4 frag_blur(v2f_blur i) : SV_Target
	{
		[unroll(10)]
		fixed4 col = fixed4(0,0,0,0);
 		for(fixed j=0 ;j<11;j++ ){
 			col += tex2D(_MainTex, i.uv+float2(j/10*_BlurRadius*2-_BlurRadius,0) )/10;
 		}
		col.a=(0.30 * col.r + 0.59 * col.g + 0.11 * col.b);
		col*=_Color;
		fixed d=distance(i.uv,center);
		// fixed a =max(0,(d-maxdis*_ShowRadius))/maxdis*(1-_ShowRadius);
		fixed a =d/maxdis;
		// fixed a =d*10;
		fixed4 col2=(_Color1*i.uv.y*_Color1.a+_Color2*(1-i.uv.y)*_Color2.a);
		fixed4 col3=tex2D(_Mask, i.uv);
		// col2.a=(0.30 * col2.r + 0.59 * col2.g + 0.11 * col2.b);
		return col+col2*lerp(0.6,1,col3.a);
	}
 
	ENDCG
 
	SubShader
	{
		Pass
		{
			ZTest Always
			Cull Off
			ZWrite Off
			Fog{ Mode Off }
 			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert_blur
			#pragma fragment frag_blur
			ENDCG
		}
	}
}