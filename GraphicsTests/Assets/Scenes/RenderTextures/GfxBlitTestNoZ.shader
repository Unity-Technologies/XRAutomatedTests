// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Tests/Blit Without ZStates" {
Properties {
    _MainTex ("", 2D) = "white" {}
}
SubShader {
    Pass {
        Fog { Mode Off } // no Cull Off, and no Z states
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        struct v2f {
            float4 pos : SV_POSITION;
            half2 uv : TEXCOORD0;
        };

        v2f vert (appdata_base v) {
            v2f o;
            o.pos = UnityObjectToClipPos (v.vertex);
            o.uv = v.texcoord.xy;
            return o;
        }

        sampler2D _MainTex;

        fixed4 frag (v2f i) : COLOR {
            fixed4 c = tex2D (_MainTex, i.uv) * fixed4(1.0,0.8,0.6,1.0);
            return c;
        }
        ENDCG
    }
}
SubShader {
    Pass {
        Fog { Mode Off } // no Cull Off, and no Z states
        SetTexture [_MainTex] { combine texture }
    }
}
FallBack Off
}
