Shader "Custom/XYZ_UV2" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SecondTex ("Secondary (RGB)", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert nolightmap nofog noshadow noforwardadd novertexlights exclude_path:prepass exclude_path:deferred nometa

        sampler2D _MainTex;
        sampler2D _SecondTex;

        struct Input {
            float2 uv_MainTex;
            float2 uv2_SecondTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            half4 c2 = tex2D (_SecondTex, IN.uv2_SecondTex);
            o.Albedo = lerp(c.rgb, c2.rgb, 0.5);
            o.Alpha = lerp(c.a, c2.a, 0.5);
        }
        ENDCG
    }
}
