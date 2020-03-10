Shader "Surface/WorldNormalRefl" {
Properties {
  _Cube ("Texture", CUBE) = "" {}
  _ReflCube ("Refl Texture", CUBE) = "" {}
}
SubShader {
    Tags { "RenderType" = "Opaque" }
CGPROGRAM
#pragma surface surf Lambert nofog nolightmap noshadow
struct Input {
  fixed3 worldNormal;
  fixed3 worldRefl;
};
samplerCUBE _Cube;
samplerCUBE _ReflCube;
void surf (Input IN, inout SurfaceOutput o)
{
    o.Albedo = texCUBE (_Cube, IN.worldNormal).rgb * 0.5 + texCUBE (_ReflCube, IN.worldRefl).rgb * 0.3;
}
ENDCG
}
Fallback "VertexLit"
}
