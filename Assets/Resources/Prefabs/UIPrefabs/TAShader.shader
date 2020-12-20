// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'UnityObjndcolortToWorldNorm' with 'UnityObjndcolortToWorldNormal'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjndcolortToClipPos(*)'

Shader "TAShader"
{
    Properties
    {   
        _StartColor("Ramp起始色", Color) = (1,1,1,1)
        _EndColor("Ramp结束色", Color) = (1,1,1,1)
        [Toggle]_Toggle("沿着UV方向渐变",int) = 1
        _NOVPower("NOV过渡指数", float) = 1
        _NOVBias("NOV过渡偏移", float) = 0 

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
            #include "UnityCG.cginc"

            fixed4 _StartColor;
            fixed4 _EndColor;
            bool _Toggle;
            float _NOVPower;
            float _NOVBias;

            //Translate HSV to RGB
            float3 hsv2rgb(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }
            
            //Translate RGB to HSV
            float3 rgb2hsv(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                float3 hsv = float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
                return hsv;
            }

            struct v2f
            {   
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR0;
            };
            
            v2f vert (appdata_full v)
            {
                v2f o;
                fixed3 startcolor = rgb2hsv(_StartColor);
                fixed3 endcolor = rgb2hsv(_EndColor);
                if(_Toggle)
                {   
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    //Get UV
                    float3 uv = v.texcoord;
                    fixed3 hsv;
                    //Interpolation by uv.x
                    hsv.x = lerp(startcolor.x,endcolor.x,uv.x);
                    hsv.y = lerp(startcolor.y,endcolor.y,uv.x);
                    hsv.z = lerp(startcolor.z,endcolor.z,uv.x);
                    o.color = fixed4(hsv2rgb(hsv),1);
                    return o;
                }
                else
                {
                    o.vertex = UnityObjectToClipPos(v.vertex);

                    float3 normal = v.normal;
                    //Get normal in world coordinate
                    float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                    //Get position in world coordinate
                    float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    //Get view direction towards the vertex
                    float3 viewDir = normalize(worldPos-_WorldSpaceCameraPos.xyz);
                    //Get cosine value of the angle between view direction and normal
                    float3 angle = dot(viewDir,worldNormal);

                    float reflectionFactor = _NOVBias + pow(1+angle,_NOVPower);

                    fixed3 hsv;
                    
                    //Interpolation by cosine value of the angle between view direction and normal
                    /*hsv.x = lerp(startcolor.x,endcolor.x,1-angle);
                    hsv.y = lerp(startcolor.y,endcolor.y,1-angle);
                    hsv.z = lerp(startcolor.z,endcolor.z,1-angle);*/

                    //Update: Interpolation by reflectionFactor
                    hsv.x = lerp(startcolor.x,endcolor.x,1-reflectionFactor);
                    hsv.y = lerp(startcolor.y,endcolor.y,1-reflectionFactor);
                    hsv.z = lerp(startcolor.z,endcolor.z,1-reflectionFactor);

                    o.color = fixed4(hsv2rgb(hsv),1);
                    return o;
                }
                
                
            }

            fixed4 frag (v2f i) : SV_Target
            {   
                return i.color;
            }

            ENDCG
        }
    }
}
