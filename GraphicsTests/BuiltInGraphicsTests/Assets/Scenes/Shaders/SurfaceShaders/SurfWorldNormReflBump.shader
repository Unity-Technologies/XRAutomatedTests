Shader "Surface/WorldNormalReflBump" {
Properties {
  _Cube ("Texture", CUBE) = "" {}
  _BumpMap ("Bump", 2D) = "bump" {}
  _ReflCube ("Refl Texture", CUBE) = "" {}
}
SubShader {
    Tags { "RenderType" = "Opaque" }
CGPROGRAM
#pragma surface surf Lambert nofog nolightmap noshadow
struct Input {
  float2 uv_BumpMap;
  fixed3 worldNormal;
  fixed3 worldRefl;
  INTERNAL_DATA
};
samplerCUBE _Cube;
sampler2D _BumpMap;
samplerCUBE _ReflCube;
void surf (Input IN, inout SurfaceOutput o)
{
    o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
    fixed3 worldNormal = WorldNormalVector (IN, o.Normal);
    fixed3 worldRefl = WorldReflectionVector (IN, o.Normal);
    o.Albedo = texCUBE (_Cube, worldNormal).rgb * 0.5 + texCUBE (_ReflCube, worldRefl).rgb * 0.3;
}
ENDCG
}
Fallback "VertexLit"
}
