Shader "Supercent/ShadowColor"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _Threshold ("Shadow Threshold", Range(0,2)) = 1
        _ShadowSoftness ("Shadow Smoothness", Range(0.5, 1)) = 0.6
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)
        _RimPower ("Rim Power", Range(0, 1)) = 1
        _RimWeight ("Rim Weight", Range(0, 1)) = 0.5
        _RimColor ("Rim Color", Color) = (0,0,0,1)
        [Toggle] _ENABLE_SLIME_EFFECT ("Enable Slime Effect", Float) = 0 // 0은 비활성화, 1은 활성화
        _StretchValue ("Stretch Value", Range(0,1)) = 1
        _StretchAmplitude ("Stretch Amplitude", Range(0,2)) = 0.5
    }

    CGINCLUDE

    #pragma shader_feature _ENABLE_SLIME_EFFECT_ON

    sampler2D _MainTex;
    sampler2D _BumpMap;
    fixed4 _Color;
    half _Shininess;

    half _Threshold;
    half _ShadowSoftness;
    half3 _ShadowColor;
    half _RimPower;
    half _RimWeight;
    half3 _RimColor;

    half _StretchValue;
    half _StretchAmplitude;

    struct Input
    {
        float2 uv_MainTex;
        float2 uv_BumpMap;
    };

    inline half4 LightingToon(SurfaceOutput s, half3 lightDir, float3 viewDir, half atten)
    {
        #ifndef USING_DIRECTIONAL_LIGHT
        lightDir = normalize(lightDir);
        #endif
        half shadowDot = pow(dot(s.Normal, lightDir) * 0.5 + 0.5, _Threshold);
        float threshold = smoothstep(0.5, _ShadowSoftness, shadowDot);
        half3 diffuseTerm = saturate(threshold * atten);
        half3 diffuse = lerp(_ShadowColor, _LightColor0.rgb, diffuseTerm);

        // float3 rimColor;
        // float rim = abs(dot(viewDir, s.Normal));
        // float invrim = 1 - rim;
        // rimColor = _RimColor * (pow(invrim, 6 * (1 - _RimWeight)) * _RimPower);

        float4 final;
        final.rgb = s.Albedo.rgb * diffuse;
        final.rgb = final.rgb; // + rimColor;
        final.a = s.Alpha;

        return final;
    }

    void surf(Input IN, inout SurfaceOutput o)
    {
        fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
        o.Albedo = tex.rgb * _Color.rgb;
        o.Gloss = tex.a;
        o.Alpha = tex.a * _Color.a;
        o.Specular = _Shininess;
        o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
    }

    void vert(inout appdata_full v)
    {
        #if _ENABLE_SLIME_EFFECT_ON // 키워드가 활성화되었을 때만 실행
        float slimeEffect = smoothstep(0.0, 1.0, sin(_StretchValue) * 0.5 + 0.5); // 0~1 사이로 변환

        // 슬라임처럼 Y축 변형 (X, Z는 반대로 늘어나고 줄어듬)
        v.vertex.y += slimeEffect * _StretchAmplitude; // Y축 늘어나는 정도 적용

        // X, Z는 Y가 늘어날 때 줄어들고, Y가 줄어들 때 늘어남
        float xzScale = 1.0 - slimeEffect * 0.5; // X, Z 크기를 Y 축 변화에 비례해서 줄이고 늘림
        v.vertex.x *= xzScale; // X축 변화 적용
        v.vertex.z *= xzScale; // Z축 변화 적용
        #endif
    }
    ENDCG

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 400

        CGPROGRAM
        #pragma surface surf Toon addshadow vertex:vert
        #pragma multi_compile_instancing
        ENDCG
    }
    Fallback "Diffuse"
}