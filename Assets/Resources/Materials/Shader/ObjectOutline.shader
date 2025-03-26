Shader "Custom/OutlineWithLighting"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)  // アウトラインの色
        _OutlineWidth ("Outline Width", Range(0.001, 0.03)) = 0.01  // アウトラインの太さ
        _MainTex ("Texture", 2D) = "white" {}  // 通常のモデルのテクスチャ
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        // **1. アウトライン描画**
        Pass
        {
            Name "Outline"
            Cull Front // 裏面のみ描画（アウトライン用）

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION; // **修正: POSITION → SV_POSITION**
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                v.vertex.xyz += norm * _OutlineWidth; // **法線方向に拡大**
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor * 2; // **アウトラインを強調**
            }
            ENDCG
        }

        // **2. 通常のオブジェクト描画（ライトの影響を受ける）**
        Pass
        {
            Name "Regular"
            Tags { "LightMode"="ForwardBase" }
            Cull Back  // 通常のモデル描画

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"  // **ライト処理を追加**

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION; // **修正: POSITION → SV_POSITION**
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1; // **修正: POSITION を削除**
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // **ライトの影響を受けるカラーを計算**
                float3 normal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float diff = max(dot(normal, lightDir), 0); // **拡散光（ディフューズ）**
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 lighting = texColor * diff;  // **ライトの影響を反映**
                return lighting;
            }
            ENDCG
        }
    }
}
