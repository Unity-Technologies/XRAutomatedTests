Shader "SkyboxMesh Background" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
	}
	Category {
	   Tags { "Queue"="Background" }
		ZWrite Off
		Lighting Off
		Fog {Mode Off}
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		SubShader {
			Pass {
				SetTexture [_MainTex] {
					Combine texture
				}
				SetTexture [_MainTex] {
					constantColor [_Color]
					combine previous * constant
				}
			}
		}
	}
}