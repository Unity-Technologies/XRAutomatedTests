// Mandelbrot shader
// can be used to simulate variable pixel shader workloads
// sgreen 3/17/2015

Shader "Custom/Mandel" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Iterations("Mandel iterations", Range(1, 10000)) = 64
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float _Iterations;

		float4 Mandel(float2 z)
		{
			float2 c = z;
			int n = (int) _Iterations;
			int i;
			for(i=0; i<n; i++) {
				z = c + float2(z.x*z.x - z.y*z.y, 2.0*z.x*z.y);	// z^2
				if (dot(z, z) > 4.0)
					break;
			}
			if (i >= n) {
				return float4(0, 0, 0, 1);
			} else {
				return float4(sin(i*0.15), sin(i*0.2), cos(i*0.1 + 0.7), 1.0);
			}
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float4 c = Mandel(IN.uv_MainTex*float2(3.0, 2.0) - float2(2.0, 1.0));
			o.Albedo = c.rgb;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
