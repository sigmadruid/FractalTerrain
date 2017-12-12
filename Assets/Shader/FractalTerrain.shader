Shader "Custom/FractalTerrain"
{
	Properties
	{
		_TerrainMap ("TerrainMap", 2D) = "white" {}
		_TexR ("_TexR", 2D) = "white" {}
		_TexG ("_TexG", 2D) = "white" {}
		_TexB ("_TexB", 2D) = "white" {}
		_TexA ("_TexA", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "LightMode"="ForwardBase"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct a2v
			{
				float4 pos : POSITION;
				float2 uvMap : TEXCOORD0;
				float2 uvTex : TEXCOORD1;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uvMap : TEXCOORD0;
				float2 uvTex : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				float3 worldLight : TEXCOORD3;
			};

			sampler2D _TerrainMap;
			sampler2D _TexR;
			sampler2D _TexG;
			sampler2D _TexB;
			sampler2D _TexA;
			
			v2f vert (a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uvMap = v.uvMap;
				o.uvTex = v.uvTex;
				o.worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
				float3 worldPos = mul(unity_ObjectToWorld, v.pos);
				o.worldLight = normalize(UnityWorldSpaceLightDir(worldPos));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 terrain = tex2D(_TerrainMap, i.uvMap);

				fixed3 colR = tex2D(_TexR, i.uvTex).rgb;
				fixed3 colG = tex2D(_TexG, i.uvTex).rgb;
				fixed3 colB = tex2D(_TexB, i.uvTex).rgb;
				fixed3 colA = tex2D(_TexA, i.uvTex).rgb;

				fixed3 terrainColor = colR * terrain.r + colG * terrain.g + colB * terrain.b + colA * terrain.a;

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

				fixed3 diffuse = _LightColor0.rgb * terrainColor.rgb * (0.5 + 0.5 * dot(i.worldNormal, i.worldLight));

				return fixed4(diffuse, 1);
			}
			ENDCG
		}
	}
}
