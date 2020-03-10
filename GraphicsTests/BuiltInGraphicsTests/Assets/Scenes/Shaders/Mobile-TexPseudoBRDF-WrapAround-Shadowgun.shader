// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "MADFINGER/Characters/BRDFLit (Supports Backlight)" {
Properties {
    _MainTex ("Base (RGB) Gloss (A)", 2D) = "grey" {}
    _BumpMap ("Normalmap", 2D) = "bump" {}
    _BRDFTex ("NdotL NdotH (RGB)", 2D) = "white" {}
    _LightProbesLightingAmount("Light probes lighting amount", Range(0,1)) = 0.9
}
SubShader {
    Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
    LOD 400

CGPROGRAM
#pragma surface surf MyPseudoBRDF vertex:vert exclude_path:prepass nolightmap noforwardadd noambient approxview

struct MySurfaceOutput {
    fixed3 Albedo;
    fixed3 Normal;
    fixed3 Emission;
    fixed Specular;
    fixed Gloss;
    fixed Alpha;
};


sampler2D _BRDFTex;

float _LightProbesLightingAmount;

struct Input {
    float2 uv_MainTex;
    float2 uv_BumpMap;
    fixed3 SHLightingColor;
};


 void vert (inout appdata_full v,out Input o)
 {
    UNITY_INITIALIZE_OUTPUT(Input,o)

    float3 wrldNormal   = mul((float3x3)unity_ObjectToWorld,SCALED_NORMAL);
    float3 SHLighting   = ShadeSH9(float4(wrldNormal,1));

    o.SHLightingColor = saturate(SHLighting + (1 - _LightProbesLightingAmount).xxx);
 }


inline fixed4 LightingMyPseudoBRDF (MySurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
{
    fixed3 halfDir = normalize(lightDir + viewDir);

    fixed nl = dot (s.Normal, lightDir);
    fixed nh = dot (s.Normal, halfDir);
    fixed4 l = tex2D(_BRDFTex, fixed2(nl * 0.5 + 0.5, nh));

    fixed4 c;

    c.rgb = s.Albedo * (l.rgb + s.Gloss * l.a) * 2;
    c.a = 0;

    return c;
}


sampler2D _MainTex;
sampler2D _BumpMap;

void surf (Input IN, inout MySurfaceOutput o) {
    fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
    o.Albedo = tex.rgb * IN.SHLightingColor;
    o.Gloss = tex.a;
    o.Alpha = tex.a;
    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}
ENDCG

    }

FallBack "Diffuse"
}
