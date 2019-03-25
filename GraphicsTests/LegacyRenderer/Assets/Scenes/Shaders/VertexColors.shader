// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Vertex Colored" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
}


SubShader {
    Pass {
        BindChannels {
            Bind "vertex", vertex
            Bind "color", color
        }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            float4 _MainTex_ST;
            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);;
                o.color = v.color;
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            sampler2D _MainTex;
            half4 frag (v2f i) : SV_Target
            {
                return i.color * tex2D(_MainTex, i.texcoord);
            }
            ENDCG
    }
}

SubShader {
    Pass {
        BindChannels {
            Bind "vertex", vertex
            Bind "color", color
            Bind "normal", normal
        }
        SetTexture[_MainTex] { combine primary }
    }
}
FallBack Off
}
