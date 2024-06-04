/*
 * @Description: Water ripple screen post-processing effect
 * @Author: Duawieh
 * @Email: DuawiehPublic@outlook.com
 * @Date: 2024-06-04 23:20:12
 * @LastEditors: Duawieh
 * @LastEditTime: 2024-06-04 23:35:22
 */
Shader "BattleEffect/ScreenWave" {
    Properties {
        
    }


    /*************
     * TO DO TAG *
     *************/
    SubShader {
        // No culling or depth
        Cull Off 
        ZWrite Off 
        ZTest Always

        Pass {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct a2v {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (a2v v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(1, 1, 1, 1);
            }

            ENDCG
        }
    }
}
