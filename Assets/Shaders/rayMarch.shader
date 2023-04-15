Shader "Custom/rayMarch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            #define MAX_STEPS 1000
            #define SURFACE_DIST 0.0001
            #define MAX_DIST 60
            #define RADIUS 2

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            struct marchResult
            {
                float dist;
                int stepsTaken;
                bool hit;
                float3 hitPos;
            };

            // get variables passed from material
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 cameraSettings;
            float4x4 localToWorldMatrix;
            float3 cameraPosition;

            float getDist(float3 p) {
                return length(p) - RADIUS;
            }

            marchResult rayMarch(float3 origin, float3 direction) {
                // gives calculated distance to scene
                marchResult march;
                float3 currentPos = origin;
                float currentDist = 0;
                int i;
                bool hit=false;

                for(i = 0; i < MAX_STEPS; i++) {
                    float dist = getDist(currentPos);

                    if (dist < SURFACE_DIST) {
                        hit = true;
                        break;
                    }
                    if(dist > MAX_DIST){
                        break;
                    }

                    currentPos += direction * dist;
                    currentDist += dist;
                }

                march.dist = currentDist;
                march.stepsTaken = i;
                march.hit = hit;
                march.hitPos = origin + (direction * currentDist);
                return march;
            }

            float4 skyBoxColor(float3 direction){
                float4 ground = float4(0.2, 0.2, 0.2, 0);
                float4 horizon = float4(0.8, 0.8, 0.8, 0);
                float4 sky = float4(0.3, 0.5, 0.8, 0);

                if(direction.y < 0) {
                    return ground;
                }
                else{
                    return sky;
                }
                //else if(direction.y >=0 && direction.y < 0.2) {
                //    float step = smoothstep(0, 0.2, direction.y)
                //    return lerp(ground, horizon, step)
                //}
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);;
                return o;
            }

            // for every pixel on the screen. i.uv is the coordinate on the screen.
            fixed4 frag (v2f i) : SV_Target
            {
                i.uv -= float2(0.5, 0.5);
                // get local coords of pixel (on the projection plane)
                float3 localCoords = float3(i.uv.x * cameraSettings.x, i.uv.y * cameraSettings.y, cameraSettings.z);
                float4 globalCoords = mul(localToWorldMatrix, float4(localCoords, 1));

                float3 direction = normalize(globalCoords.xyz - cameraPosition);
                marchResult result = rayMarch(cameraPosition, direction);
                //return result.dist;
                //return float4(direction, 0);
                if(result.hit == true){
                    return float4(1,0,0,0);
                }
                else {
                    return skyBoxColor(direction);
                }
                //return float4(globalCoords.x, globalCoords.y, globalCoords.z, 0);
            }
            ENDCG
        }
    }
}
