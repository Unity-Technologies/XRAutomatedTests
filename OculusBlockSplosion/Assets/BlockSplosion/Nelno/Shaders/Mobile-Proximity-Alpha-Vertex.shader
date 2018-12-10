// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//
Shader "Mobile/ProximityAlpha-Vertex" {
Properties {
	_MainTex ( "Base(RGBA)", 2D ) = "white" {}
	_Distance( "Distance", Float ) = 2.0
	_Scale( "Scale", Float ) = 4.0
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off
	Lighting Off 
	ZWrite Off 
	Fog { Color (0,0,0,0) }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}
	
	SubShader {
		Pass {
			CGPROGRAM 
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			
			struct vertOut {
				float4 pos 		: SV_POSITION;
				float2 uv		: TEXCOORD0;			
				fixed4 color 	: COLOR;
			}; 

			uniform sampler2D 	_MainTex;
			uniform float4 		_MainTex_ST;			
			uniform float3 		_Position[20];
			uniform float 	 	_NumPositions;
			uniform float 		_Distance;
			uniform float		_Scale;

			vertOut vert( appdata_full v ) {
				vertOut vo;
				
				vo.pos = UnityObjectToClipPos( v.vertex );
				vo.uv = TRANSFORM_TEX( v.texcoord, _MainTex );
				vo.color = v.color;
				float4 wspos = mul( unity_ObjectToWorld, v.vertex );
				float alpha = 0.0;
				for ( int i = 0; i < 20; i++ ) {
					float dist = distance( wspos.xyz, _Position[i] );
					alpha += 1.0f - clamp( dist / _Distance, 0.0, 1.0 );				
				}
				vo.color.a = clamp( alpha * _Scale, 0.0, 1.0 );

				return vo;
			}
			
			fixed4 frag( vertOut vi ) : COLOR0 {
				float4 result = tex2D( _MainTex, vi.uv );
				result.a *= vi.color.a;
				return result;
			}
			
			ENDCG
		}
	}
}
}