// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Per pixel bumped refraction.
// Uses a normal map to distort the image behind, and
// an additional texture to tint the color.

Shader "FX/Glass/Stained BumpDistort" {
Properties {
    _BumpAmt  ("Distortion", range (0,128)) = 10
    _MainTex ("Tint Color (RGB)", 2D) = "white" {}
    _BumpMap ("Normalmap", 2D) = "bump" {}
}

Category {

    // We must be transparent, so other objects are drawn before this one.
    Tags { "Queue"="Transparent" "RenderType"="Opaque" }


    SubShader {

        // This pass grabs the screen behind the object into a texture.
        // We can access the result in the next pass as _GrabTexture
        GrabPass {
            Name "BASE"
            Tags { "LightMode" = "Always" }
        }

        // Main pass: Take the texture grabbed above and use the bumpmap to perturb it
        // on to the screen
        Pass {
            Name "BASE"
            Tags { "LightMode" = "Always" }

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#include "UnityCG.cginc"

struct appdata_t {
    float4 vertex : POSITION;
    float2 texcoord: TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f {
    float4 vertex : SV_POSITION;
    float4 uvgrab : TEXCOORD0;
    float2 uvbump : TEXCOORD1;
    float2 uvmain : TEXCOORD2;
    UNITY_VERTEX_OUTPUT_STEREO
    UNITY_FOG_COORDS(3)
};

float _BumpAmt;
float4 _BumpMap_ST;
float4 _MainTex_ST;

v2f vert (appdata_t v)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    o.vertex = UnityObjectToClipPos(v.vertex);
    #if UNITY_UV_STARTS_AT_TOP
    float scale = -1.0;
    #else
    float scale = 1.0;
    #endif
    o.uvgrab = ComputeGrabScreenPos(o.vertex);
    o.uvbump = TRANSFORM_TEX( v.texcoord, _BumpMap );
    o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
    UNITY_TRANSFER_FOG(o,o.vertex);
    return o;
}

UNITY_DECLARE_SCREENSPACE_TEXTURE(_GrabTexture);
float4 _GrabTexture_TexelSize;
sampler2D _BumpMap;
sampler2D _MainTex;

half4 frag (v2f i) : SV_Target
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
    // calculate perturbed coordinates
    half2 bump = UnpackNormal(tex2D( _BumpMap, i.uvbump )).rg; // we could optimize this by just reading the x & y without reconstructing the Z
    float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy;
    i.uvgrab.xy = offset * UNITY_Z_0_FAR_FROM_CLIPSPACE(i.uvgrab.z) + i.uvgrab.xy;

    half4 col = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture, i.uvgrab.xy / i.uvgrab.w);
    half4 tint = tex2D(_MainTex, i.uvmain);
    col *= tint;
    UNITY_APPLY_FOG(i.fogCoord, col);

    return col;
}
ENDCG
        }
    }

    // ------------------------------------------------------------------
    // Fallback for older cards and Unity non-Pro

    SubShader {
        Blend DstColor Zero
        Pass {
            Name "BASE"
            SetTexture [_MainTex] { combine texture }
        }
    }
}

}
