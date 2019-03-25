Shader "Test/BlendModes" {
    Properties {
        _MainTex ("Main", 2D) = "white" {}
        _MySrcBlend ("Src", Int) = 1
        _MyDstBlend ("Dst", Int) = 0
    }

    SubShader {
        Pass {
            Lighting Off
            Blend [_MySrcBlend] [_MyDstBlend]
            SetTexture [_MainTex] { combine texture }
        }
    }
}
