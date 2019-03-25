// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/Additive" {
Properties {
    _ShadowTex ("Cookie", 2D) = "" {}
    _FalloffTex ("FallOff", 2D) = "" {}
}

Subshader {
    Pass {
        ZWrite Off
        Offset -1, -1

        Fog { Color (1, 1, 1) }
        AlphaTest Greater 0
        ColorMask RGB
        Blend SrcAlpha One

        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct v2f {
                float4 uvShadow : TEXCOORD0;
                float4 uvFalloff : TEXCOORD1;
                float4 pos : SV_POSITION;
            };
            float4x4 unity_Projector;
            float4x4 unity_ProjectorClip;
            v2f vert (float4 vertex : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (vertex);
                o.uvShadow = mul (unity_Projector, vertex);
                o.uvFalloff = mul (unity_ProjectorClip, vertex);
                return o;
            }
            sampler2D _ShadowTex;
            sampler2D _FalloffTex;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texS = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
                texS.a = 1.0-texS.a;
                fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));
                fixed4 res;
                res.rgb = fixed3(1,1,1);
                res.a = texS.a * texF.a;
                return res;
            }
        ENDCG
    }
}
}
