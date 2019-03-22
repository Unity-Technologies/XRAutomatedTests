Shader "Surface/Slices" {
Properties {
  _MainTex ("Texture", 2D) = "white" {}
}
SubShader {
    Tags { "RenderType" = "Opaque" }
  Cull Off
CGPROGRAM
#pragma surface surf Lambert addshadow nofog nolightmap noshadow novertexlights
struct Input {
  float2 uv_MainTex;
  float3 worldPos;
};
sampler2D _MainTex;
void surf (Input IN, inout SurfaceOutput o)
{
  clip (frac((IN.worldPos.y+IN.worldPos.z*0.1) * 5) - 0.5);
  o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
}
ENDCG
}
Fallback "VertexLit"
}
