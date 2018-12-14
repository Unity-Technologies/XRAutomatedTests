// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//
Shader "Mobile/ProximityAlpha" {
Properties {
	_MainTex ( "Base(RGBA)", 2D ) = "white" {}
	//_Position0 ( "Position", Vector ) = ( 0.0, 0.0, 0.0, 0.0 )
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
				float4 wspos	: TEXCOORD1;
			}; 

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;			
			uniform float3 _Position[10];
			uniform float  _NumPositions;

			vertOut vert( appdata_full v ) {
				vertOut vo;
				
				vo.pos = UnityObjectToClipPos( v.vertex );
				vo.uv = TRANSFORM_TEX( v.texcoord, _MainTex );			
				vo.color = v.color;
				vo.wspos = mul( unity_ObjectToWorld, v.vertex );

				return vo;
			}
			
			fixed4 frag( vertOut vi ) : COLOR0 {
				const float FADE_TO_ZERO_DIST = 3.0;
				float alpha = 0.0f;
				for ( int i = 0; i < 10; ++i ) {
					float dist = distance( vi.wspos, _Position[i] );
					alpha += clamp( 1.0f - ( dist / FADE_TO_ZERO_DIST ), 0.0, 1.0 );
				}
				float4 result = tex2D( _MainTex, vi.uv );
				result *= clamp( alpha * 2.0, 0.0, 1.0 );
				return result;						
			}
			
			ENDCG
		}
	}
}
}