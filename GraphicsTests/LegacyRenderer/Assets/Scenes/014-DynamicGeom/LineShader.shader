// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Line Shader" {
Properties {
    _Color ("Color", Color) = (1,0.5,0.5,1)
}
SubShader {
    Pass {

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

float4 _Color;

struct v2f
{
    float4 pos : SV_POSITION;
    float4 color : COLOR0;
    float psize : PSIZE;
};

v2f vert( appdata_base v )
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.color = _Color;
    o.psize = 1.0;
    return o;
}

half4 frag (v2f i) : SV_Target
{
    return i.color;
}
ENDCG
    }
}

FallBack "VertexLit"
}
