Shader "Will/TestInstancing"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 0.5)
        _Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
        _MinLightness ("MinLightness", Range(0, 1)) = 0.2
        
        _IndexForLightmap ("Index for Lightmap", Vector) = (0, 0, 128.0, 128.0) //xy是当前mesh包围盒在世界空间的起点xz坐标，zw是当前mesh包围盒的宽高(均以米为单位)。以普通128地块举例：terrian_40_41是( 39*128, 40*128,128.0,128.0 )
        
        _VertUVForLightmap ("Vert UV for Lightmap", Vector) = (0, 1, 0, 1) //处理顶点的UV范围不是0-1的情况。普通的128地块:(0,1,0,1), 如果是浮空岛等特殊mesh，则传入UV的边界值(uMin,uMax,vMin,vMax)
        
        _IndexForBasemap ("Index for Basemap", Vector) = (0, 0, 128.0, 128.0) //同_IndexForLightmap。浮空岛等因为baseMap能够向普通的128地块一样根据坐标的到正确的baseMap，所以这里浮空岛跟普通128地块一样传入所属的地块索引值，如当前的草属于地块terrian_40_41，则是( 39*128, 40*128,128.0,128.0 )
        _BaseMap ("BaseMap", 2D) = "white" { }
        
        _LightMap ("LightMap", 2D) = "white" { }// 对应地块Terrain Mesh的lightmap
        _LightMapUV ("Lightmap UV", Vector) = (0, 0, 0, 0) // 这个值由代码实时设置，是所属地块或者浮空岛由Unity分配的Lightmap上的TilingScale
        
        [Space]
        [Toggle] _UseLightmapIntensity ("UseLightmapIntensity", Float) = 0.0 // _USELIGHTMAPINTENSITY_ON
        
        [Space]
        [Toggle] _WaveEffect ("启用摆动效果。(顶点色a通道越白，摆幅越大)", Float) = 1.0 // _WAVEEFFECT_ON
        _WaveSpeed ("WaveSpeed", Range(0, 10)) = 1
        _WindStrength ("WindStrength", Range(0, 2)) = 1
        _WindDirection ("WindDirection", Range(0, 360)) = 0
        _PhaseScale ("PhaseScale", Range(0, 1)) = 1
    }
    
    SubShader
    {
        Tags { "IgnoreProjector" = "True" }
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" "Queue" = "Geometry" "DisableBatching" = "True" }
            
            Blend Off
            
            CGPROGRAM
            
            #pragma target 4.5
            #pragma exclude_renderers vulkan xboxone ps4 psp2 n3ds wiiu
            
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

            #pragma multi_compile_fwdbase
            #pragma multi_compile_instancing

			// 使用这个方法，在 vertexShader之前设置好 unity_ObjectToWorld。 
			// 另外也可以使用： VertexOutput vert(VertexInput v, uint instanceID: SV_InstanceID) 通过 SV_InstanceID 在vertexShader中访问id
            #pragma instancing_options procedural:setup 
            
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile _ _WAVEEFFECT_ON // 摆动效果
            #pragma multi_compile _USELIGHTMAPINTENSITY_ON
            
            #pragma skip_variants DIRLIGHTMAP_COMBINED DYNAMICLIGHTMAP_ON FOG_EXP FOG_EXP2 FOG_LINEAR  VERTEXLIGHT_ON DIRECTIONAL_COOKIE POINT_COOKIE LIGHTMAP_ON LIGHTPROBE_SH LIGHTMAP_SHADOW_MIXING SHADOWS_SHADOWMASK
            // 除了自定义的 shader keywords 外，剩下的还在生效的 keywords 有：DIRECTIONAL
            #pragma skip_variants SHADOWS_CUBE SHADOWS_DEPTH
            
            uniform sampler2D _BaseMap;
			float4 _BaseMap_ST;
            
            uniform float4 _IndexForLightmap;
            uniform float4 _VertUVForLightmap;
            uniform float4 _IndexForBasemap;
            
            uniform half4 _Color;
            uniform half _Cutoff;
            uniform half _MinLightness;
            
            uniform float4 _LightMapUV;
            
            #if _WAVEEFFECT_ON
                uniform half _WindStrength;
                uniform half _WindDirection;
                uniform half _WaveSpeed;
                uniform half _PhaseScale;
            #endif
            
            struct VertexInput
            {
                float4 vertex: POSITION;
                float4 uv0: TEXCOORD0; // 把单个草quad的world space x,z坐标放在 uv0.zw 中，用来处理草的摆动相位
                float2 uv1: TEXCOORD1;
                float2 uv2: TEXCOORD2;
                half3 normal: NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                #if _WAVEEFFECT_ON
                    half4 vertexColor: COLOR;
                #endif
            };
            struct VertexOutput
            {
                float4 pos: SV_POSITION;
                float2 uv0: TEXCOORD0;
                float2 uv1: TEXCOORD1;
                float4 worldPos: TEXCOORD2;
                half4 vertexColor: COLOR;
                float2 uv2: TEXCOORD4;
                
            };
            
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                
			// struct GPUItemStruct
			// {
			// 	float3 Position;
			// 	float4x4 Matrix;
			// };
			// StructuredBuffer<GPUItemStruct> posVisibleBuffer;

            StructuredBuffer<float3> posBuffer_;
                
 #endif
            
            void setup()
            {
 #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                    
               	float3 position = posBuffer_[unity_InstanceID];

			  unity_ObjectToWorld._11_21_31_41 = float4(1, 0, 0, 0);
			  unity_ObjectToWorld._12_22_32_42 = float4(0, 1, 0, 0);
			  unity_ObjectToWorld._13_23_33_43 = float4(0, 0, 1, 0);
			  unity_ObjectToWorld._14_24_34_44 = float4(position.xyz,1);
                    
 #endif
            }
            
			VertexOutput vert(VertexInput v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                VertexOutput o = (VertexOutput)0;

                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                
                // UV for Lightmap
                float2 localUV = float2(o.worldPos.x - _IndexForLightmap.x, o.worldPos.z - _IndexForLightmap.y) / _IndexForLightmap.zw; // v.uv2;
                localUV = float2(lerp(_VertUVForLightmap.x, _VertUVForLightmap.y, localUV.x), lerp(_VertUVForLightmap.z, _VertUVForLightmap.w, localUV.y));
                localUV.xy = clamp(localUV.xy, 0, 1);
                o.uv1 = localUV.xy * _LightMapUV.xy + _LightMapUV.zw; // 不烘焙，使用地表的Lightmap_ST
                
                // UV for Basemap
                localUV = float2(o.worldPos.x - _IndexForBasemap.x, o.worldPos.z - _IndexForBasemap.y) / _IndexForBasemap.zw; // v.uv2;
                localUV.xy = clamp(localUV.xy, 0, 1);
                o.uv2.xy = localUV;
                
                // for Clip tex
                o.uv0.xy = TRANSFORM_TEX(v.uv0.xy, _BaseMap);
                
                return o;
            }
            
            //------------------------------------------------------------------------
            // Fragment
            half4 frag(VertexOutput i): SV_Target
            {
                
                half4 albedo = tex2D(_BaseMap, i.uv2.xy);
                
                half3 finalRGB = albedo;
                
                half3 skyLightColor = _LightColor0.rgb;
                half3 skyLightDir = normalize(_WorldSpaceLightPos0.xyz);
                
                // albedo.a存的是当前草下方地表的NdotL
				half NdotL = pow(albedo.a, 1 / 2.2); // 转换到线性空间，其实可以在离线存NdotL的时候将 pow()也算好。为了暗示必须做伽马矫正特意将pow()计算放在这里
				
				finalRGB = ( _LightColor0.rgb * NdotL + unity_AmbientSky.rgb ) * albedo.rgb; // 这个计算与地表一致，才能保证与地表融合
                
                // finalRGB += (i.uv0.y * i.uv0.y * i.uv0.y * 0.3); // 让草顶端有个高光效果
                              
                return half4(finalRGB, 1);
            }
            ENDCG
            
        }
    }
    FallBack Off
}
