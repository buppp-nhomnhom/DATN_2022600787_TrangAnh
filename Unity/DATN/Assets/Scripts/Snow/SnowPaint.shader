Shader "Hidden/SnowPaint"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BrushPos ("Brush Position", Vector) = (0,0,0,0)
        _BrushSize ("Brush Size", Float) = 0.05
        _BrushValue ("Brush Value", Float) = 0
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _BrushPos;
            float _BrushSize;
            float _BrushValue;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float current = tex2D(_MainTex, i.uv).r;
                float dist = distance(i.uv, _BrushPos.xy);

                // Smooth brush edges
                float brush = 1 - smoothstep(_BrushSize * 0.5, _BrushSize, dist);
                float result = lerp(current, _BrushValue, brush);

                return float4(result, result, result, 1);
            }
            ENDCG
        }
    }
}