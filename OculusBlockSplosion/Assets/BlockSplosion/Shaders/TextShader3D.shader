﻿/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.2 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-3.2

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

Shader "GUI/3D Text Shader" 
{ 
	Properties 
	{ 
 	  _MainTex ("Font Texture", 2D) = "white" {} 
	  _Color ("Text Color", Color) = (1,1,1,1) 
	} 

	SubShader 
	{ 
   		Tags { "Queue"="Overlay+1" "IgnoreProjector"="True" "RenderType"="Transparent" } 
   		Lighting Off Cull Off ZWrite Off ZTest Off Fog { Mode Off } 
   		Blend SrcAlpha One 
   		Pass 
   		{ 
      		Color [_Color] 
      		SetTexture [_MainTex] 
      		{ 
         		combine primary, texture * primary 
      		} 
   		} 
	} 
}
