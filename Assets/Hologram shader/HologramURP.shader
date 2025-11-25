Shader "Custom/HologramURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (0, 1, 1, 1)
        _Glow ("Glow Intensity", Range(0,5)) = 2
        _ScanSpeed ("Scanline Speed", Range(0,10)) = 3
        _Transparency ("Transparency", Range(0,1)) = 0.7
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Glow;
            float _ScanSpeed;
            float _Transparency;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                float4 tex = tex2D(_MainTex, IN.uv);

                // Scanline effect
                float scan = sin((IN.worldPos.y + _Time.y * _ScanSpeed) * 20) * 0.15;

                // Hologram color + glow
                float3 finalColor = tex.rgb * _Color.rgb + _Glow * 0.15;

                // Alpha with scanlines + transparency
                float alpha = (_Transparency + scan) * _Color.a;

                return float4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
}
