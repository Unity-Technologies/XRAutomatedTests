Shader "Unlit/Mesh"
{
	Properties
	{

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
                float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
                float t = step(i.uv.y, 0.5);
                fixed4 topColor = fixed4(0,1,0,1);
                fixed4 bottomColor = fixed4(1, 0, 0, 1);
				// sample the texture
                fixed4 col = lerp(topColor, bottomColor, t);

				return col;
			}
			ENDCG
		}
	}
}
