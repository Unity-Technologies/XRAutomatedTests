Shader "Hidden/MRT" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
    Pass {
        Cull Off ZWrite Off ZTest Always
        Fog { Mode off }

CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

void frag (v2f_img i, out fixed4 col0 : SV_Target0, out fixed4 col1 : SV_Target1, out fixed4 col2 : SV_Target2, out fixed4 col3 : SV_Target3)
{
    col0 = fixed4(i.uv.xy,0,1);
    col1 = fixed4(i.uv.yxx,1);
    col2 = fixed4(1-i.uv.xy,1,1);
    col3 = fixed4(1-i.uv.xxy,1);
}

ENDCG

    }
}

Fallback off
}
