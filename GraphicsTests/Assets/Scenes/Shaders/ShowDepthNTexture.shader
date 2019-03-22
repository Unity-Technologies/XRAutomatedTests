Shader "Hidden/ShowDepthNTexture" {
Properties {
    _MainTex ("Base (RGB)", RECT) = "white" {}
	_Slice("Slice", Range(0, 1)) = 0
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
#if defined(STEREO_INSTANCING_ON) || defined(STEREO_MULTIVIEW_ON)
float _Slice = 0;
UNITY_DECLARE_TEX2DARRAY(_CameraDepthNormalsTexture);
#else
	uniform sampler2D _CameraDepthNormalsTexture;
	half4 _MainTex_ST;
#endif

half4 frag (v2f_img i) : SV_Target
{
    half4 tex = tex2D(_MainTex, i.uv);
#if defined(STEREO_INSTANCING_ON) || defined(STEREO_MULTIVIEW_ON)
	float3 texcoord = float3(i.uv.xy, _Slice);		
    half4 depth = UNITY_SAMPLE_TEX2DARRAY(_CameraDepthNormalsTexture, texcoord);
#else
	half4 depth = tex2D(_CameraDepthNormalsTexture, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
#endif

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
