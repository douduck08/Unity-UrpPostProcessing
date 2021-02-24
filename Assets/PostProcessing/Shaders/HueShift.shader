Shader "Hidden/HueShift" {
    SubShader {
        Cull Off ZWrite Off ZTest Always
        Pass {
            HLSLPROGRAM
            #pragma vertex postVert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "PostLib.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _Scalar;

            // ref: https://gist.github.com/mairod/a75e7b44f68110e1576d77419d608786
            float3 FastHueShift(float3 color, float hueAdjust) {
                const float3 k = float3(0.57735, 0.57735, 0.57735);
                half cosAngle = cos(hueAdjust);
                return color * cosAngle + cross(k, color) * sin(hueAdjust) + k * dot(k, color) * (1.0 - cosAngle);
            }

            half4 frag (VaryingsDefault i) : SV_Target {
                float2 uv = i.texcoord;
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                const float twoPI = 6.28318530718;
                color.rgb = FastHueShift(color.rgb, _Scalar * twoPI);
                // color.rgb = Unity_Hue_Radians_float(color.rgb, _Scalar);
                return color;
            }
            ENDHLSL
        }
    }
}
