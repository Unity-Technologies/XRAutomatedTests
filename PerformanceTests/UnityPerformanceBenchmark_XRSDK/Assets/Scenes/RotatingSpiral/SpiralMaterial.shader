Shader "Custom/SpiralMaterial" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_HeightFactor ("Height Slope", Range(0, 2)) = 1.0
		_HeightIntercept("Height Intercept", Range(-10, 10)) = 1.0
		_HeightColor("Height Color", Color) = (1, 1, 1, 1)
		_EmissionColor("Emission Color", Color) = (1, 1, 1, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _HeightColor;
		fixed4 _EmissionColor;
		half _HeightFactor;
		half _HeightIntercept;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed heightBlend = saturate((_HeightIntercept - IN.worldPos.y) * _HeightFactor);
			o.Albedo = lerp(c.rgb, 0, heightBlend);
			//o.Albedo = c.rgb + fixed3(heightBlend, heightBlend, heightBlend);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

			o.Emission = lerp(c.rgb * _EmissionColor.rgb, _HeightColor.rgb, heightBlend);

		}
		ENDCG
	}
	FallBack "Diffuse"
}
