Shader "Grab/Invert Shared" {

    SubShader {
        Tags { "Queue" = "Transparent" }

        GrabPass {
            "_SharedGrabInvertTexture"
        }

        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

#if defined(STEREO_INSTANCING_ON) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
			UNITY_DECLARE_TEX2DARRAY(_SharedGrabInvertTexture);
#else
            sampler2D _SharedGrabInvertTexture;
#endif

			struct appdata {
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
#if defined(STEREO_INSTANCING_ON) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				UNITY_VERTEX_INPUT_INSTANCE_ID
#endif
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float4 uv : TEXCOORD0;
#if defined(STEREO_INSTANCING_ON) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
#endif
			};

            v2f vert(appdata v)
            {
                    v2f o;
#if defined(STEREO_INSTANCING_ON) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_OUTPUT(v2f, o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
#endif
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = ComputeGrabScreenPos(o.vertex);
                    return o;
            }

            half4 frag(v2f i) : COLOR
            {
#if defined(STEREO_INSTANCING_ON) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				return 1 - UNITY_SAMPLE_TEX2DARRAY(_SharedGrabInvertTexture, float3(i.uv.xy / i.uv.w, unity_StereoEyeIndex));
#else
				return 1 - tex2Dproj(_SharedGrabInvertTexture, UNITY_PROJ_COORD(i.uv));
#endif
            }

            ENDCG

        }
    }
}
