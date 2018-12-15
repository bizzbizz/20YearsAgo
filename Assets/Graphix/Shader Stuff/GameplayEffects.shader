// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "20YA/GameplayEffects"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		//_NOffset("Noise offset", Vector) = (0., 0., 0.)
		_NScale("Noise scale", Vector) = (1., .2, .8, .6)
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "TransparentCutout" }
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off
		LOD 100
		Pass
		{
			CGPROGRAM
			#pragma exclude_renderers gles
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 vertex : SV_POSITION;
				float2 worldPos : TEXCOORD1;
			};

			static const float M_2_PI = 6.28;
			static const float CUTOFF = 0.1;

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _IsHazy = 0.0;
			float _TimeX = 0.0;
			float _Mist = 0.0;

			float3 _NOffset;//noise offset
			float4 _NScale;//noise scale

			float4 _Light[4];//[0].zw = light origin world pos, [1].z = world scale light max distance squared, [1].w = on/off
			float _IsGhost;
			float _Alpha;



			//------------------------------------------------------------
			//	mist'n haze
			//------------------------------------------------------------



			float hash(float n)
			{
				return frac(sin(n)*43758.5453);
			}

			// The noise function returns a value in the range -1.0f -> 1.0f
			float noise(float2 uv)
			{
				uv = _NOffset + _NScale.xy * uv;

				float2 p = floor(uv);
				float2 f = frac(uv);

				f = f*f*(3.0 - 2.0*f);
				float n = p.x + p.y*57.0 ;

				return 
					_NScale.zw * 
					float2(.004, .003) * 
					(1.0 - lerp(
							lerp(
								hash(n + 0.0), hash(n + 1.0), f.x
							),
							lerp(
								hash(n + 57.0), hash(n + 58.0), f.x
							), f.y
						)
					);
			}
			// The getTexColor function returns the color of the specifed pixel from main texture
			inline float3 getTexColor(float2 uv, float2 hazeValue, float2 mistValue)
			{
				return tex2D(_MainTex,
					(1.0 - _IsHazy) * (uv + mistValue) +
					_IsHazy * (uv + hazeValue + mistValue)
				);
			}

			inline float3 getColor(float2 uv)
			{
				float m = _TimeX * M_2_PI;
				float2 hazeValue = (1 + _TimeX * .1) * .002 * float2(sin(uv.y * m), cos(uv.x * m));
				float2 mistValue = noise(uv);
				float3 v1 = getTexColor(uv, hazeValue, 0);
				float3 v2 = getTexColor(uv, hazeValue, mistValue);
				float3 v3 = getTexColor(uv, hazeValue, -mistValue);
				float3 v4 = getTexColor(uv, hazeValue, 2 * mistValue);
				float3 v5 = getTexColor(uv, hazeValue, -2 * mistValue);
				return
					_Mist * ((v1 + v2 + v3 + v4 + v5) / 5)
					+ (1 - _Mist) * v1;
			}

			//------------------------------------------------------------
			//	surreal noise
			//------------------------------------------------------------

			inline float mod(float x, float modu) {
				return x - floor(x * (1.0 / modu)) * modu;
			}

			//inline float noise(float2 p)
			//{
			//	float sample = tex2D(_MainTex, float2(1., 2.*cos(_TimeX))*_TimeX*8. + p*1.).x;
			//	sample *= sample;
			//	return sample;
			//}

			inline float onOff(float a, float b, float c)
			{
				return step(c, sin(_TimeX + a*cos(_TimeX*b)));
			}

			//inline float ramp(float y, float start, float end)
			//{
			//	float inside = step(start, y) - step(end, y);
			//	float fact = (y - start) / (end - start)*inside;
			//	return (1. - fact) * inside;
			//}

			//inline float stripes(float2 uv)
			//{
			//	float noi = noise(uv*float2(0.5, 1.) + float2(1., 3.));
			//	return ramp(mod(uv.y*4. + _TimeX / 2. + sin(_TimeX + sin(_TimeX*0.63)), 1.), 0.5, 0.6)*noi;
			//}

			//inline float4 getVideo(float2 uv)
			//{
			//	float2 look = uv;
			//	float window = 1. / (1. + 20.*(look.y - mod(_TimeX / 4., 1.))*(look.y - mod(_TimeX / 4., 1.)));
			//	look.x = look.x + sin(look.y*10. + _TimeX) / 50.*onOff(4., 4., .3)*(1. + cos(_TimeX*80.))*window;
			//	float4 video = tex2D(_MainTex, look);
			//	return video;
			//}






			//------------------------------------------------------------
			// flashlight
			//------------------------------------------------------------


			bool pnpoly(float2 pos)
			{
				bool c = false;
				c = c ^ (((_Light[0].y > pos.y) != (_Light[3].y > pos.y)) && (pos.x < (_Light[3].x - _Light[0].x) * (pos.y - _Light[0].y) / (_Light[3].y - _Light[0].y) + _Light[0].x));
				c = c ^ (((_Light[1].y > pos.y) != (_Light[0].y > pos.y)) && (pos.x < (_Light[0].x - _Light[1].x) * (pos.y - _Light[1].y) / (_Light[0].y - _Light[1].y) + _Light[1].x));
				c = c ^ (((_Light[2].y > pos.y) != (_Light[1].y > pos.y)) && (pos.x < (_Light[1].x - _Light[2].x) * (pos.y - _Light[2].y) / (_Light[1].y - _Light[2].y) + _Light[2].x));
				c = c ^ (((_Light[3].y > pos.y) != (_Light[2].y > pos.y)) && (pos.x < (_Light[2].x - _Light[3].x) * (pos.y - _Light[3].y) / (_Light[2].y - _Light[3].y) + _Light[3].x));
				return c;
			}
			float flLight(float2 pos)
			{
				bool flHit = pnpoly(pos);
				float sqrDist = (_Light[0].z - pos.x)*(_Light[0].z - pos.x) + (_Light[0].w - pos.y)*(_Light[0].w - pos.y);
				return flHit ?
					clamp(1.0 - sqrDist / _Light[1].z,0.0,1.0)
					: 0.0;
			}

			//------------------------------------------------------------
			//	shader funcs
			//------------------------------------------------------------

			v2f vert(appdata v)
			{
				v2f o;

				o.normal = v.normal;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;

				float2 look = uv;
				float oo = onOff(4., 4., .3);
				float m = mod(_TimeX / 4., 1.);
				float window = 1. / (1. + 20.*(look.y - m)*(look.y - m));
				look.x = look.x + sin(look.y*10. + _TimeX) / 20.* oo *(1. + cos(_TimeX*80.))*window;
				float4 videoa = tex2D(_MainTex, look);

				if (videoa.a < CUTOFF) discard;

				float3 video = videoa;
				float vigAmt = (1 - oo) * 3. + .3*sin(_TimeX + 5.*cos(_TimeX*5.));
				float vignette = (1. - vigAmt*(uv.y - .5)*(uv.y - .5))*(1. - vigAmt*(uv.x - .5)*(uv.x - .5));
				//video += stripes(uv);
				//video += noise(uv*2.) / 2.;
				video *= vignette;
				//video *= (12. + mod(uv.y*30. + _TimeX, 1.)) / 13.;

				float fl = flLight(i.worldPos) * _Light[1].w;

				float3 col = getColor(uv);

				float3 color =
					(1.0 - _IsGhost) * (0.5 + fl) * col
					+ _IsGhost * video;
				float alpha =
					(1.0 - _IsGhost)
					+ _IsGhost * (0.8 - fl / 3.0)*_Alpha;
				return fixed4(color,alpha);
			}

			ENDCG
		}
	}
}