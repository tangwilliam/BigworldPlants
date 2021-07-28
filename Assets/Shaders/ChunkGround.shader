Shader "Will/ChunkGround"
{
    Properties
    {

        _ControlMap("ControlMap",2D) = "white" {}

        _Splat1 ("Layer 1 (R)", 2D) = "black" {}
        _Splat2 ("Layer 2 (G)", 2D) = "black" {}
        _Splat3 ("Layer 3 (R)", 2D) = "black" {}
        _Splat4 ("Layer 4 (R)", 2D) = "black" {}

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" "Queue" = "Geometry" "DisableBatching" = "True" }


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma target 3.0
            #pragma exclude_renderers vulkan xboxone ps4 psp2 n3ds wiiu
            
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

            #pragma multi_compile_fwdbase

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS:NORMAL;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normalWS:TEXCOORD1;
            };

            sampler2D _ControlMap;
            float4 _ControlMap_ST;

            sampler2D _Splat1;
            float4 _Splat1_ST;
            sampler2D _Splat2;
            sampler2D _Splat3;
            sampler2D _Splat4;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _ControlMap);
                o.uv.zw = TRANSFORM_TEX(v.uv, _Splat1);

                o.normalWS = UnityObjectToWorldNormal(v.normalOS);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half4 weights = tex2D(_ControlMap, i.uv);

                half4 color1 = tex2D(_Splat1, i.uv.zw);
                half4 color2 = tex2D(_Splat2, i.uv.zw);
                half4 color3 = tex2D(_Splat3, i.uv.zw);
                half4 color4 = tex2D(_Splat4, i.uv.zw);

                half3 color = (color1 * weights.x + color2 * weights.y + color3 * weights.z + color4 * weights.w).rgb;

                half3 mainLightDir = normalize(_WorldSpaceLightPos0.xyz);
                half NdotL = max(0, dot( mainLightDir, normalize(i.normalWS) ));

                color = ( _LightColor0.rgb * NdotL + unity_AmbientSky.rgb ) * color;
                return half4(color,1.0);
            }
            ENDCG
        }
    }
}
