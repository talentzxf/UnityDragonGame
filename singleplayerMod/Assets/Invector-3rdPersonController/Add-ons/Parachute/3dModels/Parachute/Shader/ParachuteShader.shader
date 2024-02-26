// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ParachuteShader"
{
	Properties
	{
		_Color("Color", Color) = (1,0,0,0)
		_Albedo("Albedo", 2D) = "white" {}
		_Metallic("Metallic", 2D) = "white" {}
		_MetallicPower("MetallicPower", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Normal("Normal", 2D) = "bump" {}
		_NormalScale("NormalScale", Float) = 0
		_NoiseMaskA("NoiseMask(A)", 2D) = "white" {}
		_NoiseIntensity("NoiseIntensity", Float) = 0
		_NoiseScale("NoiseScale", Float) = 0
		_NoiseSpeed("NoiseSpeed", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _NoiseSpeed;
		uniform float _NoiseScale;
		uniform sampler2D _NoiseMaskA;
		uniform float4 _NoiseMaskA_ST;
		uniform float _NoiseIntensity;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _NormalScale;
		uniform float4 _Color;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform float _MetallicPower;
		uniform float _Smoothness;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float2 temp_cast_0 = (_NoiseSpeed).xx;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float2 panner8 = ( 1.0 * _Time.y * temp_cast_0 + ase_vertex3Pos.xy);
			float simplePerlin2D9 = snoise( panner8*_NoiseScale );
			simplePerlin2D9 = simplePerlin2D9*0.5 + 0.5;
			float2 uv_NoiseMaskA = v.texcoord * _NoiseMaskA_ST.xy + _NoiseMaskA_ST.zw;
			v.vertex.xyz += ( ase_vertexNormal * simplePerlin2D9 * ( v.color.r * tex2Dlod( _NoiseMaskA, float4( uv_NoiseMaskA, 0, 0.0) ).a ) * _NoiseIntensity );
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 Normal56 = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _NormalScale );
			o.Normal = Normal56;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 Albedo57 = ( _Color * tex2D( _Albedo, uv_Albedo ) );
			o.Albedo = Albedo57.rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			float Metallic61 = ( tex2D( _Metallic, uv_Metallic ).a * _MetallicPower );
			o.Metallic = Metallic61;
			float Smoothness66 = _Smoothness;
			o.Smoothness = Smoothness66;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18707
314;460;1358;900;3055.834;804.6137;1.103172;True;False
Node;AmplifyShaderEditor.CommentaryNode;30;-2374.384,-1430.994;Inherit;False;1134.506;1408.941;Comment;12;56;57;51;52;49;59;46;60;62;63;64;66;Base Shader;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;25;-1804.968,342.3305;Inherit;False;1615.718;1009.55;Comment;11;15;13;12;8;3;19;5;11;9;24;6;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-2363.132,-956.8361;Inherit;False;Property;_NormalScale;NormalScale;6;0;Create;True;0;0;False;0;False;0;2.57;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;46;-2305.876,-1380.994;Float;False;Property;_Color;Color;0;0;Create;True;0;0;False;0;False;1,0,0,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;15;-1677.477,392.3305;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;60;-2167.701,-704.89;Inherit;True;Property;_Metallic;Metallic;2;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;49;-2324.384,-1185.183;Inherit;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;False;-1;None;fb641c069dcca834381897ed9b45c6b2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;13;-1742.765,789.2258;Inherit;False;Property;_NoiseSpeed;NoiseSpeed;10;0;Create;True;0;0;False;0;False;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-2161.736,-471.9558;Inherit;False;Property;_MetallicPower;MetallicPower;3;0;Create;True;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1754.968,677.4922;Inherit;False;Property;_NoiseScale;NoiseScale;9;0;Create;True;0;0;False;0;False;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;52;-2171.913,-930.4023;Inherit;True;Property;_Normal;Normal;5;0;Create;True;0;0;False;0;False;52;None;f7f322ea849ea7d41adb6fa8a7d8a3e6;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;8;-1194.643,535.4836;Inherit;False;3;0;FLOAT2;100,100;False;2;FLOAT2;0.22,0.12;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-2166.897,-283.8595;Inherit;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-1956.258,-1249.326;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;3;-1703.784,922.3188;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;19;-1711.258,1137.486;Inherit;True;Property;_NoiseMaskA;NoiseMask(A);7;0;Create;True;0;0;False;0;False;60;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-1853.992,-652.9803;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;61;-1657.281,-669.8752;Inherit;False;Metallic;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;57;-1755.462,-1249.325;Float;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;5;-678.2504,513.2693;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-539.2504,972.2692;Inherit;False;Property;_NoiseIntensity;NoiseIntensity;8;0;Create;True;0;0;False;0;False;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-1783.861,-942.4152;Float;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;66;-1720.377,-449.8828;Inherit;False;Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-919.0116,921.5591;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;9;-964.2204,545.2945;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;39;-140.6848,-245.099;Inherit;False;56;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;40;-154.7551,-355.2021;Inherit;False;57;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-358.2502,671.2693;Inherit;False;4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-142.8584,-139.9471;Inherit;False;61;Metallic;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;67;-126.2009,-13.01116;Inherit;False;66;Smoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;313.9499,-220.8601;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;ParachuteShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;52;5;59;0
WireConnection;8;0;15;0
WireConnection;8;2;13;0
WireConnection;51;0;46;0
WireConnection;51;1;49;0
WireConnection;62;0;60;4
WireConnection;62;1;63;0
WireConnection;61;0;62;0
WireConnection;57;0;51;0
WireConnection;56;0;52;0
WireConnection;66;0;64;0
WireConnection;24;0;3;1
WireConnection;24;1;19;4
WireConnection;9;0;8;0
WireConnection;9;1;12;0
WireConnection;6;0;5;0
WireConnection;6;1;9;0
WireConnection;6;2;24;0
WireConnection;6;3;11;0
WireConnection;0;0;40;0
WireConnection;0;1;39;0
WireConnection;0;3;58;0
WireConnection;0;4;67;0
WireConnection;0;11;6;0
ASEEND*/
//CHKSM=692E70AA96DAE80F966190EF21E11E2633F19C34