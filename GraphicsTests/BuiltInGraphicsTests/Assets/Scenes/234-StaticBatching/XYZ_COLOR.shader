// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/XYZ_COLOR" {
    Properties {
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            void vert( in appdata IN, out v2f o )
            {
                o.vertex = UnityObjectToClipPos( IN.vertex );
                o.color = IN.color;
            }

            void frag( in v2f IN, out fixed4 o : SV_Target0 )
            {
                o = IN.color;
            }

            ENDCG
        }
    }
}
