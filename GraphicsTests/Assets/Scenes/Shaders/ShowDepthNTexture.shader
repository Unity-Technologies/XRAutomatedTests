Shader "Hidden/ShowDepthNTexture" {
Properties {
    _MainTex ("Base (RGB)", RECT) = "white" {}
}

SubShader {
    Pass {
        ZTest Always Cull Off ZWrite Off
        Fog { Mode off }

CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthNormalsTexture;

half4 frag (v2f_img i) : SV_Target
{
    half4 tex = tex2D(_MainTex, i.uv);
    half4 depth = tex2D(_CameraDepthNormalsTexture, i.uv);
    float z;
    float3 n;
    DecodeDepthNormal (depth, z, n);
    half4 col;
    col.r = z;
    col.g = n.x*0.5+0.5;
    col.b = n.y*0.5+0.5;
    col.a = tex.a;
    return col;
}
ENDCG

    }
}

Fallback off

}
