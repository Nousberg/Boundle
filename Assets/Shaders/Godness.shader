Shader "Custom/PulsatingOutline"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,1,1,1)
        _OutlineWidth ("Outline Width", Range(0.001, 0.1)) = 0.02
        _PulseSpeed ("Pulse Speed", Range(0.1, 5.0)) = 2.0
        _NoiseScale ("Noise Scale", Range(1, 10)) = 5.0
        _WaveAmplitude ("Wave Amplitude", Range(0.01, 0.5)) = 0.1
        _WaveFrequency ("Wave Frequency", Range(0.1, 5.0)) = 1.0
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float _OutlineWidth;
            float _PulseSpeed;
            float _NoiseScale;
            float _WaveAmplitude;
            float _WaveFrequency;
            float4 _OutlineColor;

            float noise(float3 pos)
            {
                return frac(sin(dot(pos, float3(12.9898, 78.233, 45.164))) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                v2f o;
                float3 normal = UnityObjectToWorldNormal(v.normal);
                float pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;
                float noiseEffect = noise(v.vertex.xyz * _NoiseScale);
                float outlineOffset = _OutlineWidth * (pulse + noiseEffect);
                
                float wave = sin(_Time.y * _WaveFrequency + v.vertex.x * 2.0) * _WaveAmplitude;
                v.vertex.y += wave;
                
                v.vertex.xyz += normal * outlineOffset;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}
