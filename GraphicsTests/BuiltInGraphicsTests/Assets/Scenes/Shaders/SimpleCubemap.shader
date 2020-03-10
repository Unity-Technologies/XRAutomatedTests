// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TexGen/Simple Cubemap" {
Properties {
    _MainTex ("Base (RGB)", CUBE) = "" {}
}

SubShader {
    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        samplerCUBE _MainTex;

        struct appdata_t {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
        };

        struct v2f {
            float4 vertex : SV_POSITION;
            float3 texcoord : TEXCOORD0;
        };

        v2f vert (appdata_t v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.texcoord = v.normal;
            return o;
        }

        half4 frag (v2f i) : SV_Target
        {
            return texCUBE(_MainTex, i.texcoord);
        }
        ENDCG
    }
}

}
