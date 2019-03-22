Shader "Test/BlendModesAlpha" {
    Properties {
        _MainTex ("Main", 2D) = "white" {}
        _MySrcBlend ("Src", Int) = 1
        _MyDstBlend ("Dst", Int) = 0
        _MySrcBlendA ("SrcA", Int) = 1
        _MyDstBlendA ("DstA", Int) = 0
    }

    SubShader {
        Pass {
            Lighting Off
            Blend [_MySrcBlend] [_MyDstBlend], [_MySrcBlendA] [_MyDstBlendA]
            SetTexture [_MainTex] { combine texture }
        }
    }
}
