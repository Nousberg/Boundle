Shader "Custom/AdvancedWater"
{
    Properties
    {
        // Surface properties
        _Color ("Water Color", Color) = (0.2, 0.5, 0.7, 0.75)
        _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        _Shininess ("Shininess", Range(0.01, 1)) = 0.078
        
        // Normal maps for waves - packed into one texture
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0, 2)) = 1.0
        
        // Wave animation
        _WaveScale ("Wave Scale", Float) = 1.0
        _WaveSpeed ("Wave Speed", Vector) = (0.1, 0.1, 0.15, 0.07)
        
        // Geometric wave properties
        _WaveAmplitude ("Wave Amplitude", Range(0, 2)) = 0.5
        _WaveFrequency ("Wave Frequency", Range(0, 5)) = 1.0
        _WaveDirection ("Wave Direction", Vector) = (1, 0, 0, 0)
        
        // Refraction
        _RefractionStrength ("Refraction Strength", Range(0, 1)) = 0.2
        
        // Depth settings
        _ShoreColor ("Shore Water Color", Color) = (0.3, 0.7, 0.8, 0.5)
        _DeepColor ("Deep Water Color", Color) = (0.1, 0.2, 0.5, 0.8)
        _DepthFade ("Depth Fade Distance", Float) = 2.0
        
        // Foam
        _FoamTexture ("Foam Texture", 2D) = "white" {}
        _FoamColor ("Foam Color", Color) = (1, 1, 1, 1)
        _FoamAmount ("Foam Amount", Range(0, 3)) = 1.0
        _FoamSpeed ("Foam Speed", Float) = 0.5
        _FoamDistance ("Foam Shore Distance", Float) = 0.5
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        
        GrabPass { "_GrabTexture" }
        
        CGPROGRAM
        #pragma surface surf BlinnPhong alpha:auto vertex:vert
        #pragma target 3.0
        #include "UnityCG.cginc"
        
        // Surface properties
        sampler2D _NormalMap;
        sampler2D _GrabTexture;
        sampler2D _CameraDepthTexture;
        sampler2D _FoamTexture;
        
        float4 _NormalMap_ST;
        float4 _FoamTexture_ST;
        
        half4 _Color;
        half _Shininess;
        half _NormalStrength;
        
        // Wave animation
        float _WaveScale;
        float4 _WaveSpeed;
        
        // Geometric wave properties
        float _WaveAmplitude;
        float _WaveFrequency;
        float4 _WaveDirection;
        
        // Refraction
        float _RefractionStrength;
        
        // Depth settings
        half4 _ShoreColor;
        half4 _DeepColor;
        float _DepthFade;
        
        // Foam
        half4 _FoamColor;
        float _FoamAmount;
        float _FoamSpeed;
        float _FoamDistance;
        
        struct Input
        {
            float2 uv_NormalMap;
            float3 worldPos;
            float4 screenPos;
            float eyeDepth;
        };
        
        // Vertex function for wave animation
        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            
            // Calculate wave displacement based on world position and time
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            float waveX = sin(_WaveFrequency * (worldPos.x + _Time.y) * _WaveDirection.x);
            float waveZ = sin(_WaveFrequency * (worldPos.z + _Time.y) * _WaveDirection.z);
            float waveHeight = _WaveAmplitude * (waveX + waveZ) * 0.5;
            
            // Only move vertices along the Y (up) axis
            v.vertex.y += waveHeight;
            
            // Calculate normal
            float3 modifiedNormal = v.normal;
            modifiedNormal.x += _WaveAmplitude * _WaveFrequency * cos(_WaveFrequency * (worldPos.x + _Time.y)) * _WaveDirection.x;
            modifiedNormal.z += _WaveAmplitude * _WaveFrequency * cos(_WaveFrequency * (worldPos.z + _Time.y)) * _WaveDirection.z;
            v.normal = normalize(modifiedNormal);
            
            // Send vertex eye depth for depth calculations
            o.eyeDepth = -UnityObjectToViewPos(v.vertex).z;
        }
        
        void surf(Input IN, inout SurfaceOutput o)
        {
            // Calculate animated normal maps (waves)
            // Use one normal map but sample it twice with different offsets
            float2 uv1 = IN.uv_NormalMap * _WaveScale + _Time.y * _WaveSpeed.xy;
            float2 uv2 = IN.uv_NormalMap * _WaveScale * 0.5 + _Time.y * _WaveSpeed.zw;
            
            float3 normal1 = UnpackNormal(tex2D(_NormalMap, uv1));
            float3 normal2 = UnpackNormal(tex2D(_NormalMap, uv2));
            
            // Blend normals
            float3 blendedNormal = normalize(float3(normal1.xy + normal2.xy, normal1.z * normal2.z));
            float3 finalNormal = normalize(float3(blendedNormal.xy * _NormalStrength, blendedNormal.z));
            
            // Set surface normal
            o.Normal = finalNormal;
            
            // Calculate refraction
            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            float2 refractionOffset = finalNormal.xy * _RefractionStrength;
            float2 refractionUV = screenUV + refractionOffset;
            float4 refractionColor = tex2D(_GrabTexture, refractionUV);
            
            // Depth calculations
            float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)));
            float depthDifference = sceneZ - IN.eyeDepth;
            float waterDepthFactor = saturate(depthDifference / _DepthFade);
            
            // Calculate base water color based on depth
            half4 waterColor = lerp(_ShoreColor, _DeepColor, waterDepthFactor);
            waterColor.a = _Color.a;
            
            // Generate foam based on proximity to shore/objects
            float foamFactor = saturate(_FoamAmount * (1 - saturate(depthDifference / _FoamDistance)));
            float2 foamUV = IN.uv_NormalMap * _FoamTexture_ST.xy + _FoamTexture_ST.zw + _Time.y * _FoamSpeed * 0.1;
            float4 foamTex = tex2D(_FoamTexture, foamUV);
            float foam = saturate(foamTex.r * foamFactor * 2.0);
            
            // Combine colors: refraction, water color, and foam
            float4 finalColor = lerp(refractionColor, waterColor, waterColor.a);
            finalColor = lerp(finalColor, _FoamColor, foam);
            
            o.Albedo = finalColor.rgb;
            o.Specular = _Shininess;
            o.Gloss = 1.0;
            o.Alpha = lerp(waterColor.a, 1.0, foam);
        }
        ENDCG
    }
    FallBack "Diffuse"
}