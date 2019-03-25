// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Support/ImmediateMode" {
    SubShader {
        Pass {
            BindChannels {
                Bind "vertex", vertex
                Bind "color", color
            }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                return i.color;
            }
            ENDCG

        }
    }

   SubShader {
        Pass {
            BindChannels {
                Bind "vertex", vertex
                Bind "color", color
            }

            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            BindChannels {
                Bind "vertex", vertex
                Bind "color", color
            }

            Blend SrcAlpha OneMinusSrcAlpha
            SetTexture[_Dummy] {
                combine primary
            }
        }
    }
}
