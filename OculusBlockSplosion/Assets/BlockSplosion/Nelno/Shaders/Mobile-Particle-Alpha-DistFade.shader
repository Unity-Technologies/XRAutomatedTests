// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//
Shader "Mobile/Particles/Alpha Blended DistFade" {
Properties {
	_MainTex ("Particle Texture", 2D) = "white" {}
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

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			vertOut vert( appdata_full v ) {
				vertOut vo;
				
				vo.pos = UnityObjectToClipPos( v.vertex );
				
				vo.uv = TRANSFORM_TEX( v.texcoord, _MainTex );
				
				vo.color.xyz = v.color.xyz;
/*				
				float3 wspos = mul( _Object2World, v.vertex ).xyz;
				const float FADE_TO_ZERO_DIST = 25.0;
				float dist = clamp( wspos.y - _WorldSpaceCameraPos.y, 0.0, FADE_TO_ZERO_DIST );
				vo.color.w = 1.0 - ( dist / FADE_TO_ZERO_DIST );
*/			
				const float FADE_TO_ZERO_DIST = 3.0;
				float dist = clamp( v.vertex.z, 0.0, FADE_TO_ZERO_DIST );
				vo.color.w = 1.0 - ( dist / FADE_TO_ZERO_DIST );
				
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