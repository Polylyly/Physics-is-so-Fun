// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Rollthered/Fire"
{
	Properties
	{
		[Header(Refraction)]
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		_FresnelPower("Fresnel Power", Range( 0 , 5)) = 2
		[HideInInspector]_FresnelScale("Fresnel Scale", Range( 0 , 0.3)) = 1.5
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 4.4
		_TessMin( "Tess Min Distance", Float ) = 10
		_TessMax( "Tess Max Distance", Float ) = 25
		_FresnelBias("Fresnel Bias", Range( 0 , 0.2)) = 0.2364706
		[HDR]_Flamecolor2("Flame color 2", Color) = (1,0,0,0)
		[HDR]_FlameColor("Flame Color", Color) = (1,0.8068966,0,0)
		_Y_Mask("Y_Mask", Range( 0 , 5)) = 0
		_FlameHeight("Flame Height", Range( -4 , 4)) = 0
		_Flamenoise("Flame noise", 2D) = "white" {}
		_FlameWave("Flame Wave", 2D) = "white" {}
		_Refraction("Refraction", Range( 0 , 1.5)) = 0
		_v("v", Range( -1 , 1)) = 0
		_u("u", Range( -1 , 1)) = 0
		_Alpha("Alpha", Range( 0 , 1)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+100" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Front
		Blend One One
		
		GrabPass{ "RefractionGrab1" }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		#pragma surface surf Standard keepalpha finalcolor:RefractionF noshadow exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPos;
		};

		uniform sampler2D _FlameWave;
		uniform float _u;
		uniform float _v;
		uniform sampler2D _Flamenoise;
		uniform float4 _Flamenoise_ST;
		uniform float _Y_Mask;
		uniform float _FlameHeight;
		uniform float4 _Flamecolor2;
		uniform float4 _FlameColor;
		uniform float _FresnelBias;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform float _Alpha;
		uniform sampler2D RefractionGrab1;
		uniform float _ChromaticAberration;
		uniform float _Refraction;
		uniform float _TessValue;
		uniform float _TessMin;
		uniform float _TessMax;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float4 transform17 = mul(unity_WorldToObject,float4( float3(0,1,0) , 0.0 ));
			float4 appendResult29 = (float4(_u , _v , 0.0 , 0.0));
			float2 uv0_Flamenoise = v.texcoord.xy * _Flamenoise_ST.xy + _Flamenoise_ST.zw;
			float2 panner24 = ( 1.0 * _Time.y * appendResult29.xy + uv0_Flamenoise);
			float4 lerpResult23 = lerp( float4( 0,0,0,0 ) , transform17 , ( tex2Dlod( _FlameWave, float4( panner24, 0, 0.0) ) * tex2Dlod( _Flamenoise, float4( panner24, 0, 0.0) ) ));
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float clampResult12 = clamp( ( distance( ase_worldNormal.y , _Y_Mask ) - _Y_Mask ) , 0.0 , 1.0 );
			float temp_output_14_0 = ( 1.0 - clampResult12 );
			float4 lerpResult18 = lerp( float4( 0,0,0,0 ) , lerpResult23 , temp_output_14_0);
			v.vertex.xyz += ( lerpResult18 * _FlameHeight ).xyz;
			float3 ase_vertexNormal = v.normal.xyz;
			v.normal = ase_vertexNormal;
		}

		inline float4 Refraction( Input i, SurfaceOutputStandard o, float indexOfRefraction, float chomaticAberration ) {
			float3 worldNormal = o.Normal;
			float4 screenPos = i.screenPos;
			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0;
			#endif
			float halfPosW = screenPos.w * 0.5;
			screenPos.y = ( screenPos.y - halfPosW ) * _ProjectionParams.x * scale + halfPosW;
			#if SHADER_API_D3D9 || SHADER_API_D3D11
				screenPos.w += 0.00000000001;
			#endif
			float2 projScreenPos = ( screenPos / screenPos.w ).xy;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 refractionOffset = ( ( ( ( indexOfRefraction - 1.0 ) * mul( UNITY_MATRIX_V, float4( worldNormal, 0.0 ) ) ) * ( 1.0 / ( screenPos.z + 1.0 ) ) ) * ( 1.0 - dot( worldNormal, worldViewDir ) ) );
			float2 cameraRefraction = float2( refractionOffset.x, -( refractionOffset.y * _ProjectionParams.x ) );
			float4 redAlpha = tex2D( RefractionGrab1, ( projScreenPos + cameraRefraction ) );
			float green = tex2D( RefractionGrab1, ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;
			float blue = tex2D( RefractionGrab1, ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;
			return float4( redAlpha.r, green, blue, redAlpha.a );
		}

		void RefractionF( Input i, SurfaceOutputStandard o, inout half4 color )
		{
			#ifdef UNITY_PASS_FORWARDBASE
			color.rgb = color.rgb + Refraction( i, o, _Refraction, _ChromaticAberration ) * ( 1 - color.a );
			color.a = 1;
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV1 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode1 = ( _FresnelBias + _FresnelScale * pow( 1.0 - fresnelNdotV1, _FresnelPower ) );
			float4 lerpResult7 = lerp( _Flamecolor2 , _FlameColor , fresnelNode1);
			float4 temp_cast_0 = (0.0).xxxx;
			float4 temp_cast_1 = (1.0).xxxx;
			float4 clampResult31 = clamp( lerpResult7 , temp_cast_0 , temp_cast_1 );
			o.Emission = clampResult31.rgb;
			float clampResult12 = clamp( ( distance( ase_worldNormal.y , _Y_Mask ) - _Y_Mask ) , 0.0 , 1.0 );
			float temp_output_14_0 = ( 1.0 - clampResult12 );
			float lerpResult35 = lerp( 0.0 , ( fresnelNode1 * temp_output_14_0 ) , _Alpha);
			o.Alpha = lerpResult35;
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
319;92;911;596;-693.8705;-434.951;1.118101;True;False
Node;AmplifyShaderEditor.RangedFloatNode;26;1596.496,-121.6257;Float;False;Property;_u;u;20;0;Create;True;0;0;False;0;0;-0.138;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;1600.378,-43.48143;Float;False;Property;_v;v;19;0;Create;True;0;0;False;0;0;0.137;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;162.6843,293.8361;Float;False;Property;_Y_Mask;Y_Mask;13;0;Create;True;0;0;False;0;0;5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;25;1415.807,29.60971;Inherit;False;0;22;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;8;153.4688,-60.86494;Inherit;True;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;29;1852.359,53.15722;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DistanceOpNode;10;454.9435,98.99673;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;24;1513.309,159.1272;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector3Node;16;667.9654,1011.149;Float;False;Constant;_Vector0;Vector 0;4;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;11;527.6835,301.6296;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;22;1903.177,405.5273;Inherit;True;Property;_Flamenoise;Flame noise;15;0;Create;True;0;0;False;0;-1;None;6e6cba53deb4f4e41a81667b73a1ca42;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;21;1535.301,420.0704;Inherit;True;Property;_FlameWave;Flame Wave;17;0;Create;True;0;0;False;0;-1;None;e0fd6a4930877cf46b85cf41927c84db;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;3;-564.5,-118.5;Float;False;Property;_FresnelScale;Fresnel Scale;4;1;[HideInInspector];Create;True;0;0;False;0;1.5;0.068;0;0.3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-599.5,-22.5;Float;False;Property;_FresnelPower;Fresnel Power;3;0;Create;True;0;0;False;0;2;5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-602.5,50.5;Float;False;Property;_FresnelBias;Fresnel Bias;10;0;Create;True;0;0;False;0;0.2364706;0;0;0.2;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldToObjectTransfNode;17;903.8168,1040.52;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;2036.29,700.0778;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;12;494.2401,528.3492;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;5;-368.5,217.5;Float;False;Property;_FlameColor;Flame Color;12;1;[HDR];Create;True;0;0;False;0;1,0.8068966,0,0;1,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;1;-220.4357,33.09632;Inherit;True;Standard;TangentNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;6;-400.5,388.5;Float;False;Property;_Flamecolor2;Flame color 2;11;1;[HDR];Create;True;0;0;False;0;1,0,0,0;0.7279412,0.4367647,0,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;14;718.8393,708.61;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;23;1702.964,1089.14;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;7;-0.3349533,505.3345;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;18;1205.065,800.4697;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;19;1310.959,969.3138;Float;False;Property;_FlameHeight;Flame Height;14;0;Create;True;0;0;False;0;0;1;-4;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;837.6491,440.4743;Float;False;Property;_Alpha;Alpha;21;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;957.2208,-229.0979;Float;False;Constant;_Float1;Float 1;11;1;[HideInInspector];Create;True;0;0;False;0;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;719.3638,500.8314;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;1006.347,-113.1986;Float;False;Constant;_Float0;Float 0;11;1;[HideInInspector];Create;True;0;0;False;0;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;35;843.6491,296.4743;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;1319.534,-336.865;Float;False;Property;_Tesselation;Tesselation;16;0;Create;True;0;0;False;0;0;200;10;200;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;865.447,-825.2123;Inherit;False;Property;_Refraction;Refraction;18;0;Create;True;0;0;False;0;0;0.824;0;1.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;1382.959,695.3138;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ClampOpNode;31;973.6744,-712.118;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.9926471,0.9926471,0.9926471,0;False;1;COLOR;0
Node;AmplifyShaderEditor.EdgeLengthTessNode;37;1341.534,-482.865;Inherit;False;1;0;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NormalVertexDataNode;15;993.6229,725.9518;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1184.371,-942.3387;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Rollthered/Fire;False;False;False;False;True;True;True;True;True;False;False;False;False;False;True;False;False;False;False;False;False;Front;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;100;True;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;0;4.4;10;25;False;0.51;False;4;1;False;-1;1;False;-1;0;5;False;-1;10;False;-1;0;False;-1;0;False;-1;1;False;1;0,0,0,0;VertexScale;True;False;Cylindrical;False;Relative;0;;0;-1;1;5;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.053;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;29;0;26;0
WireConnection;29;1;27;0
WireConnection;10;0;8;2
WireConnection;10;1;9;0
WireConnection;24;0;25;0
WireConnection;24;2;29;0
WireConnection;11;0;10;0
WireConnection;11;1;9;0
WireConnection;22;1;24;0
WireConnection;21;1;24;0
WireConnection;17;0;16;0
WireConnection;30;0;21;0
WireConnection;30;1;22;0
WireConnection;12;0;11;0
WireConnection;1;1;2;0
WireConnection;1;2;3;0
WireConnection;1;3;4;0
WireConnection;14;0;12;0
WireConnection;23;1;17;0
WireConnection;23;2;30;0
WireConnection;7;0;6;0
WireConnection;7;1;5;0
WireConnection;7;2;1;0
WireConnection;18;1;23;0
WireConnection;18;2;14;0
WireConnection;13;0;1;0
WireConnection;13;1;14;0
WireConnection;35;1;13;0
WireConnection;35;2;36;0
WireConnection;20;0;18;0
WireConnection;20;1;19;0
WireConnection;31;0;7;0
WireConnection;31;1;34;0
WireConnection;31;2;33;0
WireConnection;37;0;38;0
WireConnection;0;2;31;0
WireConnection;0;8;39;0
WireConnection;0;9;35;0
WireConnection;0;11;20;0
WireConnection;0;12;15;0
ASEEND*/
//CHKSM=D12125618BFF967DB45ED2719A8C09E2BD180F59