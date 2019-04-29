Shader "blend2maps"
{
    Properties
    {
_Diffuse("_Diffuse", 2D) = "gray" {}
_Diffuse2("_Diffuse2", 2D) = "black" {}
_normal1("_normal1", 2D) = "bump" {}
_normal2("_normal2", 2D) = "bump" {}

    }

    SubShader
    {
        Tags
        {
"Queue"="Geometry"
"IgnoreProjector"="False"
"RenderType"="Opaque"

        }


Cull Back
ZWrite On
ZTest LEqual
ColorMask RGBA
Fog{
}


        CGPROGRAM
#pragma surface surf BlinnPhongEditor nodynlightmap vertex:vert nofog
#pragma target 2.0


sampler2D _Diffuse;
sampler2D _Diffuse2;
sampler2D _normal1;
sampler2D _normal2;

            struct EditorSurfaceOutput {
                half3 Albedo;
                half3 Normal;
                half3 Emission;
                half3 Gloss;
                half Specular;
                half Alpha;
                half4 Custom;
            };

            inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
            {
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

            }

            inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
            {
                half3 h = normalize (lightDir + viewDir);

                half diff = max (0, dot ( lightDir, s.Normal ));

                float nh = max (0, dot (s.Normal, h));
                float spec = pow (nh, s.Specular*128.0);

                half4 res;
                res.rgb = _LightColor0.rgb * diff;
                res.w = spec * Luminance (_LightColor0.rgb);

                return LightingBlinnPhongEditor_PrePass( s, res );
            }

            inline half4 LightingBlinnPhongEditor_DirLightmap (EditorSurfaceOutput s, fixed4 color, fixed4 scale, half3 viewDir, bool surfFuncWritesNormal, out half3 specColor)
            {
                UNITY_DIRBASIS
                half3 scalePerBasisVector;

                half3 lm = DirLightmapDiffuse (unity_DirBasis, color, scale, s.Normal, surfFuncWritesNormal, scalePerBasisVector);

                half3 lightDir = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
                half3 viewDirNormalized = normalize (half3 (viewDir));
                half3 h = normalize (lightDir + viewDirNormalized);

                float nh = max (0, dot (s.Normal, h));
                float spec = pow (nh, s.Specular * 128.0);

                // specColor used outside in the forward path, compiled out in prepass
                specColor = lm * _SpecColor.rgb * s.Gloss * spec;

                // spec from the alpha component is used to calculate specular
                // in the Lighting*_Prepass function, it's not used in forward
                return half4(lm, spec);
            }

            struct Input {
                float2 uv_Diffuse;
float2 uv_Diffuse2;
float2 uv_normal1;
float2 uv_normal2;

            };

            void vert (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o)
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);


            }


            void surf (Input IN, inout EditorSurfaceOutput o) {
                o.Normal = float3(0.0,0.0,1.0);
                o.Alpha = 1.0;
                o.Albedo = 0.0;
                o.Emission = 0.0;
                o.Gloss = 0.0;
                o.Specular = 0.0;
                o.Custom = 0.0;

float4 Tex2D1=tex2D(_Diffuse,(IN.uv_Diffuse.xyxy).xy);
float4 Tex2D2=tex2D(_Diffuse2,(IN.uv_Diffuse2.xyxy).xy);
float4 Splat0=saturate(IN.uv_Diffuse2.y); // + Tex2D3.x*0.01);
float4 Multiply0=lerp(Tex2D1,Tex2D2,Splat0);
float4 Tex2D5=tex2D(_normal1,(IN.uv_normal1.xyxy).xy);
float4 Tex2D0=tex2D(_normal2,(IN.uv_normal2.xyxy).xy);
float4 Lerp1=lerp(Tex2D5,Tex2D0,Splat0);
float4 UnpackNormal0=float4(UnpackNormal(Lerp1).xyz, 1.0);
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Multiply0;
o.Normal = UnpackNormal0;

                o.Normal = normalize(o.Normal);
            }
        ENDCG
    }
    Fallback "Diffuse"
}
