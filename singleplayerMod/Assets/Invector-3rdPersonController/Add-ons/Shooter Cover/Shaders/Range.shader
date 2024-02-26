// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Range"
{
	Properties
	{
		[HDR]_Color("Color", Color) = (2,2,2,0)
		_FresnelPower("Fresnel Power", Range( 0 , 10)) = 7
		_IntersectIntensity1("Intersect Intensity", Range( 0 , 1)) = 0.2
		[SingleLineTexture]_MaskR("Mask(R)", 2D) = "black" {}
		_MaskTile("MaskTile", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow noambient nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
			float3 worldNormal;
			float3 viewDir;
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _IntersectIntensity1;
		uniform float _FresnelPower;
		uniform sampler2D _MaskR;
		uniform float _MaskTile;


		inline float4 ASESafeNormalize(float4 inVec)
		{
			float dp3 = max( 0.001f , dot( inVec , inVec ) );
			return inVec* rsqrt( dp3);
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 lerpResult47 = lerp( _Color , float4( 0,0,0,0 ) , float4( 0,0,0,0 ));
			o.Emission = lerpResult47.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth9 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth9 = abs( ( screenDepth9 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _IntersectIntensity1 ) );
			float clampResult10 = clamp( distanceDepth9 , 0.0 , 1.0 );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float ShieldRimPower3 = _FresnelPower;
			float fresnelNdotV6 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode6 = ( 0.0 + 2.1 * pow( max( 1.0 - fresnelNdotV6 , 0.0001 ), ShieldRimPower3 ) );
			float dotResult21 = dot( ase_worldNormal , i.viewDir );
			float FaceSign24 = (1.0 + (sign( dotResult21 ) - -1.0) * (0.0 - 1.0) / (1.0 - -1.0));
			float lerpResult25 = lerp( fresnelNode6 , 0.0 , FaceSign24);
			float Fresnel7 = lerpResult25;
			float clampResult16 = clamp( pow( ( ( 1.0 - clampResult10 ) + Fresnel7 ) , 3.02 ) , 0.0 , 1.0 );
			float4 normalizeResult56 = ASESafeNormalize( _ScreenParams );
			float4 temp_output_43_0 = ( float4( (ase_screenPosNorm).xy, 0.0 , 0.0 ) * ( normalizeResult56 * _MaskTile ) );
			float2 uv_TexCoord59 = i.uv_texcoord * temp_output_43_0.xy + temp_output_43_0.xy;
			float temp_output_29_0 = ( ( clampResult16 - 0.0 ) * _Color.a * tex2D( _MaskR, uv_TexCoord59 ).r );
			o.Alpha = temp_output_29_0;
		}

		ENDCG
	}
}
/*ASEBEGIN
Version=18912
363;73;979;553;1347.291;14.27721;1.330045;True;False
Node;AmplifyShaderEditor.CommentaryNode;18;-1367.265,-1228.49;Inherit;False;1094.131;402.4268;Comment;6;24;23;22;21;20;19;Face Sign (0 = Front, 1 = Back);1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;20;-1317.265,-1178.49;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;19;-1292.045,-1010.063;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;1;-1194.423,-645.1381;Inherit;False;1030.896;385.0003;Comment;7;7;6;4;3;2;25;27;OutLine;1,1,1,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;21;-1059.014,-1083.369;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SignOpNode;22;-891.4622,-1071.757;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-1193.95,-596.7231;Float;False;Property;_FresnelPower;Fresnel Power;1;0;Create;True;0;0;0;False;0;False;7;6.12;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;23;-728.9592,-1089.917;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3;-876.6721,-595.1379;Float;False;ShieldRimPower;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;4;-1164.714,-372.7584;Inherit;False;3;ShieldRimPower;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;39;-1736.622,-110.6867;Inherit;False;1638.365;376.0614;Comment;8;8;9;10;11;28;13;15;16;Internal Intersection;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-507.1332,-1093.571;Float;False;FaceSign;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;6;-871.1061,-458.3171;Inherit;False;Standard;TangentNormal;ViewDir;True;True;5;0;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;2.1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1686.622,129.1001;Float;False;Property;_IntersectIntensity1;Intersect Intensity;2;0;Create;True;0;0;0;False;0;False;0.2;0.778;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-676.8978,-370.3525;Inherit;False;24;FaceSign;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;9;-1342.656,115.4455;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;25;-457.5936,-634.8315;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;10;-1032.91,106.3747;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-362.7426,-490.5313;Float;False;Fresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenParams;55;-1015.454,551.0598;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;11;-791.7626,64.09225;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;28;-846.5166,-60.68667;Inherit;False;7;Fresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;40;-1193.892,295.9635;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;46;-691.7341,668.3882;Inherit;False;Property;_MaskTile;MaskTile;4;0;Create;True;0;0;0;False;0;False;0;100;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;56;-722.011,537.3751;Inherit;False;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;-612.1271,-50.10774;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;41;-945.7277,295.8302;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-531.5977,531.1727;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PowerNode;15;-453.8552,-40.71191;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;3.02;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-611.9054,407.198;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ClampOpNode;16;-269.2581,-43.44666;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;59;-404.6226,318.6693;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;53.79314,-398.6827;Inherit;False;Property;_Color;Color;0;1;[HDR];Create;True;0;0;0;False;0;False;2,2,2,0;2,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;30;32.52726,-80.71485;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;44;-144.2514,272.524;Inherit;True;Property;_MaskR;Mask(R);3;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;-1;None;0000000000000000f000000000000000;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;338.0254,229.6911;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;47;443.8041,-265.1938;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;643.8751,-138.4976;Float;False;True;-1;2;;0;0;Unlit;Range;False;False;False;False;True;False;True;True;True;True;True;True;False;False;True;True;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;21;0;20;0
WireConnection;21;1;19;0
WireConnection;22;0;21;0
WireConnection;23;0;22;0
WireConnection;3;0;2;0
WireConnection;24;0;23;0
WireConnection;6;3;4;0
WireConnection;9;0;8;0
WireConnection;25;0;6;0
WireConnection;25;2;27;0
WireConnection;10;0;9;0
WireConnection;7;0;25;0
WireConnection;11;0;10;0
WireConnection;56;0;55;0
WireConnection;13;0;11;0
WireConnection;13;1;28;0
WireConnection;41;0;40;0
WireConnection;52;0;56;0
WireConnection;52;1;46;0
WireConnection;15;0;13;0
WireConnection;43;0;41;0
WireConnection;43;1;52;0
WireConnection;16;0;15;0
WireConnection;59;0;43;0
WireConnection;59;1;43;0
WireConnection;30;0;16;0
WireConnection;44;1;59;0
WireConnection;29;0;30;0
WireConnection;29;1;17;4
WireConnection;29;2;44;1
WireConnection;47;0;17;0
WireConnection;0;2;47;0
WireConnection;0;9;29;0
WireConnection;0;10;29;0
ASEEND*/
//CHKSM=CCCD29953E8D4FDF2ACD7FBB50F0A7F8444D48B1