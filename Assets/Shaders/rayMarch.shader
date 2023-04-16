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
                float3 direction;
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
                float traveledDist = 0;
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
                    traveledDist += dist;
                }

                march.dist = traveledDist;
                march.stepsTaken = i;
                march.hit = hit;
                march.hitPos = origin + (direction * traveledDist);
                march.direction = direction;
                return march;
            }

            float3 getHitNormal(float3 hitPos) {
                // return direction normal to the surface where hitHappened
                return normalize(hitPos);
            }

            float3 getReflectionDirection(float3 hitPos, float3 incomingDirection) {
                // gives direction where ray should go based on where it hit and the direction it hit from
                // turn the incomingDirection around, then reflect over the normal (draw it out)
                float3 hitNormal = getHitNormal(hitPos);
                float3 ontoNormal = dot(-incomingDirection, hitNormal) * hitNormal;
                float3 toNormal =  ontoNormal + incomingDirection;
                float3 outgoingDirection = -incomingDirection + 2*(toNormal);
                return outgoingDirection;
            }

            marchResult getRayResult(float3 origin, float3 direction) {
                marchResult firstResult = rayMarch(origin, direction);
                if(firstResult.hit){
                    // do reflection
                    float3 reflectDirection = getReflectionDirection(firstResult.hitPos, direction);
                    return rayMarch(firstResult.hitPos + getHitNormal(firstResult.hitPos) * SURFACE_DIST * 1.1, reflectDirection); // move start position slightly away from surface so we're not within the surface dist
                }
                return firstResult;
            }

            float4 skyBoxColor(float3 direction){
                float4 ground = float4(0.2, 0.2, 0.2, 0);
                float4 horizon = float4(0.7, 0.9, 0.9, 0);
                float4 sky = float4(0.25, 0.45, 0.75, 0);

                if(direction.y < -0.05) {
                    return ground;
                }
                else if(direction.y >=-0.05 && direction.y < 0) {
                    float step = smoothstep(-0.05, 0, direction.y);
                    return lerp(ground, horizon, step);
                }
                else if(direction.y >=0 && direction.y < 0.05) {
                    return horizon;
                }
                else if(direction.y >=0.05 && direction.y < 0.4) {
                    float step = smoothstep(0.05, 0.4, direction.y);
                    return lerp(horizon, sky, step);
                }
                return sky;
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
                marchResult result = getRayResult(cameraPosition, direction);
                //return result.dist;
                //return float4(direction, 0);
                if(result.hit == true){
                    return float4(1,0,0,0);
                }
                else {
                    return skyBoxColor(result.direction);
                }
                //return float4(globalCoords.x, globalCoords.y, globalCoords.z, 0);
            }
            ENDCG
        }
    }
}
