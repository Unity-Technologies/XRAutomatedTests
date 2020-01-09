// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Tests/Use Texture ST Without Texture" {
Properties {
    _UnusedTexture ("The texture", 2D) = "white" {}
}
SubShader {
    Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
struct v2f {
    float4 pos : SV_POSITION;
    fixed4 color : COLOR;
};
float4 _UnusedTexture_ST;
v2f vert (appdata_base v) {
    v2f o;
    o.pos = UnityObjectToClipPos (v.vertex);
    o.color = _UnusedTexture_ST;
    return o;
}
fixed4 frag (v2f i) : SV_Target
{
    return i.color;
}
ENDCG
    }
}
FallBack Off
}
