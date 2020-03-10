Shader "Hidden/ShowAlpha"
{
    SubShader
    {
        Pass
        {
            ZTest Always ZWrite Off Cull Off Fog { Mode Off }
            Blend DstAlpha Zero
            Color (1,1,1,1)
        }
    }
}
