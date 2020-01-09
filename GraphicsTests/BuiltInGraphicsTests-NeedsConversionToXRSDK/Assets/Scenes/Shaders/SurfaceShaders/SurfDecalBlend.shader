Shader "Surface/DecalBlend" {
Properties {
  _Decal ("Decal", 2D) = "gray" {}
  _Color ("Color", Color) = (1,1,1,1)
}
SubShader {
    Offset -1, -1 // push it towards the camera a bit
    Tags { "Queue" = "Geometry+100" } // make it render after regular opaque geometry

CGPROGRAM
// not including "nofog" or "noshadow" since shader is used for them in 168-BlendStrips
#pragma surface surf Lambert decal:blend nolightmap novertexlights
struct Input {
  float2 uv_Decal;
};
sampler2D _Decal;
float4 _Color;
void surf (Input IN, inout SurfaceOutput o)
{
  half4 c = tex2D (_Decal, IN.uv_Decal) * _Color;
  o.Albedo = c.rgb;
  o.Alpha = c.a;
}
ENDCG
}

// old cards: just make it not be rendered
SubShader {
    Pass {
        ColorMask 0
        ZWrite Off
    }
}

Fallback Off
}
