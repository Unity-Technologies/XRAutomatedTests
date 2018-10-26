// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Grab/Invert" {

    SubShader {
        Tags { "Queue" = "Transparent" }

        GrabPass {
        }

        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _GrabTexture;

            struct v2f {
                float4 vertex : SV_POSITION;
                float4 uv : TEXCOORD0;
            };

            v2f vert(float4 vertex : POSITION, float2 uv: TEXCOORD0)
            {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(vertex);
                    o.uv = ComputeGrabScreenPos(o.vertex);
                    return o;
            }

            half4 frag(v2f i) : COLOR
            {
                return 1- tex2Dproj (_GrabTexture, UNITY_PROJ_COORD(i.uv));
            }

            ENDCG

        }
    }

    SubShader {
    Tags { "Queue" = "Transparent" }

    GrabPass {
    }

    Pass {
        SetTexture [_GrabTexture] { combine one-texture }
    }
}
}
