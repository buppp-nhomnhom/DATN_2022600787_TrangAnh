Shader "Custom/SnowTerrain"
{
    Properties
    {
        _SnowMask        ("Snow Mask", 2D) = "white" {}
        _LightSnowColor  ("Light Snow Color", Color) = (0.75, 0.75, 0.78, 1)
        _HeavySnowColor  ("Heavy Snow Color", Color) = (1, 1, 1, 1)
        _GroundColor     ("Ground Color", Color) = (1, 1, 1, 1)
        _GroundTex       ("Ground Texture", 2D) = "white" {}
        _GroundTexScale  ("Ground Tex Scale", Float) = 0.1
        _LightSnowHeight ("Light Snow Height", Float) = 0.1
        _HeavySnowHeight ("Heavy Snow Height", Float) = 0.35
        _HeavyZoneMin    ("Heavy Zone Min XZ", Vector) = (0,0,0,0)
        _HeavyZoneMax    ("Heavy Zone Max XZ", Vector) = (1,0,1,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert
        #pragma target 3.0

        sampler2D _SnowMask;
        sampler2D _GroundTex;
        fixed4 _LightSnowColor;
        fixed4 _HeavySnowColor;
        fixed4 _GroundColor;
        float _GroundTexScale;
        float _LightSnowHeight;
        float _HeavySnowHeight;
        float4 _HeavyZoneMin;
        float4 _HeavyZoneMax;

        struct Input
        {
            float2 uv_SnowMask;
            float3 worldPos;
        };

        float IsHeavyZone(float3 wpos)
        {
            float inX = step(_HeavyZoneMin.x, wpos.x)
                      * step(wpos.x, _HeavyZoneMax.x);
            float inZ = step(_HeavyZoneMin.z, wpos.z)
                      * step(wpos.z, _HeavyZoneMax.z);
            return inX * inZ;
        }

        void vert(inout appdata_full v)
        {
            float3 wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
            float2 uv = v.texcoord.xy;
            float snowAmount = tex2Dlod(
                _SnowMask, float4(uv, 0, 0)).r;
            float heavy = IsHeavyZone(wpos);
            float height = lerp(_LightSnowHeight, _HeavySnowHeight, heavy);
            v.vertex.y += snowAmount * height;
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            float snowAmount = tex2D(_SnowMask, IN.uv_SnowMask).r;
            float heavy = IsHeavyZone(IN.worldPos);

            fixed4 snowCol = lerp(_LightSnowColor, _HeavySnowColor, heavy);

            // Sample ground texture theo worldPos
            float2 groundUV = IN.worldPos.xz * _GroundTexScale;
            fixed4 groundTex = tex2D(_GroundTex, groundUV);

            // Blend texture với ground color
            fixed3 groundFinal = groundTex.rgb * _GroundColor.rgb;

            o.Albedo = lerp(groundFinal, snowCol.rgb, snowAmount);
        }
        ENDCG
    }
}