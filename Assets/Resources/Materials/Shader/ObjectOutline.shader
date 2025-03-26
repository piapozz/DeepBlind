Shader "Custom/OutlineWithLighting"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)  // �A�E�g���C���̐F
        _OutlineWidth ("Outline Width", Range(0.001, 0.03)) = 0.01  // �A�E�g���C���̑���
        _MainTex ("Texture", 2D) = "white" {}  // �ʏ�̃��f���̃e�N�X�`��
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        // **1. �A�E�g���C���`��**
        Pass
        {
            Name "Outline"
            Cull Front // ���ʂ̂ݕ`��i�A�E�g���C���p�j

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
                float4 pos : SV_POSITION; // **�C��: POSITION �� SV_POSITION**
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                v.vertex.xyz += norm * _OutlineWidth; // **�@�������Ɋg��**
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor * 2; // **�A�E�g���C��������**
            }
            ENDCG
        }

        // **2. �ʏ�̃I�u�W�F�N�g�`��i���C�g�̉e�����󂯂�j**
        Pass
        {
            Name "Regular"
            Tags { "LightMode"="ForwardBase" }
            Cull Back  // �ʏ�̃��f���`��

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"  // **���C�g������ǉ�**

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION; // **�C��: POSITION �� SV_POSITION**
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1; // **�C��: POSITION ���폜**
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
                // **���C�g�̉e�����󂯂�J���[���v�Z**
                float3 normal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float diff = max(dot(normal, lightDir), 0); // **�g�U���i�f�B�t���[�Y�j**
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 lighting = texColor * diff;  // **���C�g�̉e���𔽉f**
                return lighting;
            }
            ENDCG
        }
    }
}
