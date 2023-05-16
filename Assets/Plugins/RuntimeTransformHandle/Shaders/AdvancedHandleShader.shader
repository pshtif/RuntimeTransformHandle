/**
 * Created by Peter @sHTiF Stefcek 20.10.2020
 */

Shader "sHTiF/AdvancedHandleShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _CameraPosition ("Camera Position", Vector) = (0,0,0,0)
        _CameraDistance ("Camera Distance", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }
        LOD 100
        
        ZWrite On
        ZTest Always
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct IN
            {
                float4 vertex : POSITION;
            };

            struct OUT
            {
                float4 vertex : SV_POSITION;
                float4 positionWS : TEXCOORD1;
            };

            float4 _Color;
            float3 _CameraPosition;
            float _CameraDistance;

            OUT vert (IN v)
            {
                OUT o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.positionWS = mul(unity_ObjectToWorld, float4(v.vertex.xyz,1.0));
                
                return o;
            }

            fixed4 frag (OUT i) : SV_Target
            {
                if (distance(i.positionWS, _CameraPosition) > _CameraDistance)
                    return float4(0,0,0,0.1f);
                
                return _Color;
            }
            ENDCG
        }
    }
}
