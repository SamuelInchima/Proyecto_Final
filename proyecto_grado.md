This file is a merged representation of a subset of the codebase, containing files not matching ignore patterns, combined into a single document by Repomix.

# File Summary

## Purpose
This file contains a packed representation of a subset of the repository's contents that is considered the most important context.
It is designed to be easily consumable by AI systems for analysis, code review,
or other automated processes.

## File Format
The content is organized as follows:
1. This summary section
2. Repository information
3. Directory structure
4. Repository files (if enabled)
5. Multiple file entries, each consisting of:
  a. A header with the file path (## File: path/to/file)
  b. The full contents of the file in a code block

## Usage Guidelines
- This file should be treated as read-only. Any changes should be made to the
  original repository files, not this packed version.
- When processing this file, use the file path to distinguish
  between different files in the repository.
- Be aware that this file may contain sensitive information. Handle it with
  the same level of security as you would the original repository.

## Notes
- Some files may have been excluded based on .gitignore rules and Repomix's configuration
- Binary files are not included in this packed representation. Please refer to the Repository Structure section for a complete list of file paths, including binary files
- Files matching these patterns are excluded: **/*.meta, **/*.prefab, **/*.unity, **/*.asset, Packages/**, ProjectSettings/**
- Files matching patterns in .gitignore are excluded
- Files matching default ignore patterns are excluded
- Files are sorted by Git change count (files with more changes are at the bottom)

# Directory Structure
```
Assets/
  Scripts/
    GestorMenuPrincipal.cs
    GestorPenalizacion.cs
    GestorPhishing.cs
    GestorServicioCliente.cs
  TextMesh Pro/
    Documentation/
      TextMesh Pro User Guide 2016.pdf
    Fonts/
      LiberationSans - OFL.txt
      LiberationSans.ttf
    Resources/
      Fonts & Materials/
        LiberationSans SDF - Drop Shadow.mat
        LiberationSans SDF - Outline.mat
      LineBreaking Following Characters.txt
      LineBreaking Leading Characters.txt
    Shaders/
      TMP_Bitmap-Custom-Atlas.shader
      TMP_Bitmap-Mobile.shader
      TMP_Bitmap.shader
      TMP_SDF Overlay.shader
      TMP_SDF SSD.shader
      TMP_SDF-Mobile Masking.shader
      TMP_SDF-Mobile Overlay.shader
      TMP_SDF-Mobile SSD.shader
      TMP_SDF-Mobile.shader
      TMP_SDF-Surface-Mobile.shader
      TMP_SDF-Surface.shader
      TMP_SDF.shader
      TMP_Sprite.shader
      TMPro_Mobile.cginc
      TMPro_Properties.cginc
      TMPro_Surface.cginc
      TMPro.cginc
    Sprites/
      EmojiOne Attribution.txt
      EmojiOne.json
      EmojiOne.png
.gitignore
.vsconfig
```

# Files

## File: Assets/Scripts/GestorPhishing.cs
```csharp
using UnityEngine;
using UnityEngine.UI; // Librería OBLIGATORIA para poder controlar componentes 'Image' de la interfaz (UI) de Unity.

// [System.Serializable] permite que esta "cajita de datos" sea visible en el Inspector de Unity.
[System.Serializable]
public class CasoPhishing
{
    [Tooltip("Solo sirve para que te guíes en el Inspector (ej. 'Nivel 1: SMS Falso' o 'Nivel 2: Correo RRHH')")]
    public string nombreDelCaso; // Un texto opcional de guía para organizarte en el editor.

    [Tooltip("Arrastra aquí la imagen del correo, SMS o pantalla creada por el diseñador")]
    public Sprite imagenDelMensaje; // 'Sprite' es el archivo de imagen 2D que vamos a mostrar.

    [Tooltip("Marca la casilla (True) si es Phishing/Peligroso. Déjala vacía (False) si es Seguro/Legítimo.")]
    public bool esPhishing; // Guarda Verdadero (true) si es una trampa, o Falso (false) si es real.
}

public class GestorPhishing : MonoBehaviour
{
    // ==========================================
    // VARIABLES DE LA INTERFAZ (UI)
    // ==========================================
    [Header("Elementos en Pantalla")]
    // 'Image' es el componente en el Canvas (como un portarretratos vacío) donde pondremos nuestros 'Sprites' (las fotos).
    public Image contenedorImagenPantalla;

    [Header("Conexión con otros sistemas")]
    public GestorPenalizacion scriptPenalizacion; // Referencia a tu script que apaga los 3 candados.

    // ==========================================
    // DATOS DE LOS 5 NIVELES
    // ==========================================
    [Header("Lista de los 5 Casos (Correos, SMS, etc.)")]
    public CasoPhishing[] nivelesPhishing; // Array (lista) que guardará las 5 imágenes y sus respuestas.
    private int nivelActual = 0;           // Lleva la cuenta de en qué imagen vamos (empieza en 0).

    // MÉTODO START: Se ejecuta una sola vez al iniciar el juego.
    void Start()
    {
        // Cargamos la primera imagen de nuestra lista (posición 0).
        CargarImagenNivel(0);
    }

    // ==========================================
    // MÉTODO PARA CAMBIAR LA IMAGEN
    // ==========================================

    // Esta función busca la imagen en nuestra lista y la coloca en el contenedor de la pantalla.
    public void CargarImagenNivel(int indice)
    {
        // Actualizamos nuestro contador con el nivel actual.
        nivelActual = indice;

        // '.sprite' es la propiedad del componente Image que nos permite cambiar el dibujo que se ve en pantalla.
        contenedorImagenPantalla.sprite = nivelesPhishing[nivelActual].imagenDelMensaje;
    }

    // ==========================================
    // MÉTODOS PARA LOS BOTONES (VERDE Y ROJO)
    // ==========================================

    // 1. Esta función se conectará al BOTÓN VERDE ("Seguro")
    public void BotonVerdeSeguro()
    {
        // Si el jugador presiona "Seguro", revisamos si en este nivel 'esPhishing' es Falso (false).
        if (nivelesPhishing[nivelActual].esPhishing == false)
        {
            // ¡Acertó! El mensaje o correo era legítimo de NexoBank.
            Debug.Log("¡Correcto! El mensaje era seguro. Pasando al siguiente caso...");
            AvanzarSiguienteCaso();
        }
        else
        {
            // ¡Falló! Era un ataque (correo falso, SMS con virus, etc.) y el jugador confió en él.
            Debug.Log("¡Error! Era una trampa de Phishing. Pierdes un candado de seguridad.");
            scriptPenalizacion.RestarCandado();
        }
    }

    // 2. Esta función se conectará al BOTÓN ROJO ("Peligroso / Reportar")
    public void BotonRojoPeligroso()
    {
        // Si el jugador presiona "Peligroso", revisamos si en este nivel 'esPhishing' es Verdadero (true).
        if (nivelesPhishing[nivelActual].esPhishing == true)
        {
            // ¡Acertó! Detectó las pistas sospechosas en la imagen (mala ortografía, link falso, etc.).
            Debug.Log("¡Excelente! Detectaste el intento de Phishing. Pasando al siguiente caso...");
            AvanzarSiguienteCaso();
        }
        else
        {
            // ¡Falló! Reportó un mensaje o correo real de la empresa por equivocación.
            Debug.Log("¡Error! Reportaste un mensaje oficial de NexoBank. Pierdes un candado.");
            scriptPenalizacion.RestarCandado();
        }
    }

    // ==========================================
    // MÉTODO INTERNO PARA AVANZAR DE NIVEL
    // ==========================================

    private void AvanzarSiguienteCaso()
    {
        // Sumamos 1 para pasar a la siguiente imagen de la lista.
        nivelActual++;

        // Verificamos si todavía quedan imágenes por revisar en la lista (menos de 5).
        if (nivelActual < nivelesPhishing.Length)
        {
            // Mostramos la nueva imagen en pantalla.
            CargarImagenNivel(nivelActual);
        }
        else
        {
            // Si ya superó las 5 imágenes, encendemos la pantalla de victoria.
            Debug.Log("¡Felicidades! Superaste los 5 niveles del Detector de Phishing.");

            // Llamamos al GestorPenalizacion para detener el reloj y mostrar estadísticas de candados
            scriptPenalizacion.VictoriaPhishing();
        }
    }
}
```

## File: Assets/TextMesh Pro/Fonts/LiberationSans - OFL.txt
```
Digitized data copyright (c) 2010 Google Corporation
	with Reserved Font Arimo, Tinos and Cousine.
Copyright (c) 2012 Red Hat, Inc.
	with Reserved Font Name Liberation.

This Font Software is licensed under the SIL Open Font License, Version 1.1.
This license is copied below, and is also available with a FAQ at: http://scripts.sil.org/OFL

-----------------------------------------------------------
SIL OPEN FONT LICENSE Version 1.1 - 26 February 2007
-----------------------------------------------------------

PREAMBLE
The goals of the Open Font License (OFL) are to stimulate worldwide development of collaborative font projects, to support the font creation efforts of academic and linguistic communities, and to provide a free and open framework in which fonts may be shared and improved in partnership with others.

The OFL allows the licensed fonts to be used, studied, modified and redistributed freely as long as they are not sold by themselves. The fonts, including any derivative works, can be bundled, embedded, redistributed and/or sold with any software provided that any reserved names are not used by derivative works. The fonts and derivatives, however, cannot be released under any other type of license. The requirement for fonts to remain under this license does not apply to any document created using the fonts or their derivatives.

DEFINITIONS
"Font Software" refers to the set of files released by the Copyright Holder(s) under this license and clearly marked as such. This may include source files, build scripts and documentation.

"Reserved Font Name" refers to any names specified as such after the copyright statement(s).

"Original Version" refers to the collection of Font Software components as distributed by the Copyright Holder(s).

"Modified Version" refers to any derivative made by adding to, deleting, or substituting -- in part or in whole -- any of the components of the Original Version, by changing formats or by porting the Font Software to a new environment.

"Author" refers to any designer, engineer, programmer, technical writer or other person who contributed to the Font Software.

PERMISSION & CONDITIONS
Permission is hereby granted, free of charge, to any person obtaining a copy of the Font Software, to use, study, copy, merge, embed, modify, redistribute, and sell modified and unmodified copies of the Font Software, subject to the following conditions:

1) Neither the Font Software nor any of its individual components, in Original or Modified Versions, may be sold by itself.

2) Original or Modified Versions of the Font Software may be bundled, redistributed and/or sold with any software, provided that each copy contains the above copyright notice and this license. These can be included either as stand-alone text files, human-readable headers or in the appropriate machine-readable metadata fields within text or binary files as long as those fields can be easily viewed by the user.

3) No Modified Version of the Font Software may use the Reserved Font Name(s) unless explicit written permission is granted by the corresponding Copyright Holder. This restriction only applies to the primary font name as presented to the users.

4) The name(s) of the Copyright Holder(s) or the Author(s) of the Font Software shall not be used to promote, endorse or advertise any Modified Version, except to acknowledge the contribution(s) of the Copyright Holder(s) and the Author(s) or with their explicit written permission.

5) The Font Software, modified or unmodified, in part or in whole, must be distributed entirely under this license, and must not be distributed under any other license. The requirement for fonts to remain under this license does not apply to any document created using the Font Software.

TERMINATION
This license becomes null and void if any of the above conditions are not met.

DISCLAIMER
THE FONT SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT OF COPYRIGHT, PATENT, TRADEMARK, OR OTHER RIGHT. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, INCLUDING ANY GENERAL, SPECIAL, INDIRECT, INCIDENTAL, OR CONSEQUENTIAL DAMAGES, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF THE USE OR INABILITY TO USE THE FONT SOFTWARE OR FROM OTHER DEALINGS IN THE FONT SOFTWARE.
```

## File: Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Drop Shadow.mat
```
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!21 &2100000
Material:
  serializedVersion: 6
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_Name: LiberationSans SDF - Drop Shadow
  m_Shader: {fileID: 4800000, guid: fe393ace9b354375a9cb14cdbbc28be4, type: 3}
  m_ShaderKeywords: OUTLINE_ON UNDERLAY_ON
  m_LightmapFlags: 5
  m_EnableInstancingVariants: 0
  m_DoubleSidedGI: 0
  m_CustomRenderQueue: -1
  stringTagMap: {}
  disabledShaderPasses: []
  m_SavedProperties:
    serializedVersion: 3
    m_TexEnvs:
    - _BumpMap:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _Cube:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _FaceTex:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _MainTex:
        m_Texture: {fileID: 28684132378477856, guid: 8f586378b4e144a9851e7b34d9b748ee,
          type: 2}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _OutlineTex:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    m_Floats:
    - _Ambient: 0.5
    - _Bevel: 0.5
    - _BevelClamp: 0
    - _BevelOffset: 0
    - _BevelRoundness: 0
    - _BevelWidth: 0
    - _BumpFace: 0
    - _BumpOutline: 0
    - _ColorMask: 15
    - _Diffuse: 0.5
    - _DiffusePower: 1
    - _FaceDilate: 0.1
    - _FaceUVSpeedX: 0
    - _FaceUVSpeedY: 0
    - _GlowInner: 0.05
    - _GlowOffset: 0
    - _GlowOuter: 0.05
    - _GlowPower: 0.75
    - _GradientScale: 10
    - _LightAngle: 3.1416
    - _MaskSoftnessX: 0
    - _MaskSoftnessY: 0
    - _OutlineSoftness: 0
    - _OutlineUVSpeedX: 0
    - _OutlineUVSpeedY: 0
    - _OutlineWidth: 0.1
    - _PerspectiveFilter: 0.875
    - _Reflectivity: 10
    - _ScaleRatioA: 0.9
    - _ScaleRatioB: 0.73125
    - _ScaleRatioC: 0.64125
    - _ScaleX: 1
    - _ScaleY: 1
    - _ShaderFlags: 0
    - _Sharpness: 0
    - _SpecularPower: 2
    - _Stencil: 0
    - _StencilComp: 8
    - _StencilOp: 0
    - _StencilReadMask: 255
    - _StencilWriteMask: 255
    - _TextureHeight: 1024
    - _TextureWidth: 1024
    - _UnderlayDilate: 0
    - _UnderlayOffsetX: 0.5
    - _UnderlayOffsetY: -0.5
    - _UnderlaySoftness: 0.05
    - _VertexOffsetX: 0
    - _VertexOffsetY: 0
    - _WeightBold: 0.75
    - _WeightNormal: 0
    m_Colors:
    - _ClipRect: {r: -32767, g: -32767, b: 32767, a: 32767}
    - _Color: {r: 1, g: 1, b: 1, a: 1}
    - _EnvMatrixRotation: {r: 0, g: 0, b: 0, a: 0}
    - _FaceColor: {r: 1, g: 1, b: 1, a: 1}
    - _GlowColor: {r: 0, g: 1, b: 0, a: 0.5}
    - _MaskCoord: {r: 0, g: 0, b: 32767, a: 32767}
    - _OutlineColor: {r: 0, g: 0, b: 0, a: 1}
    - _ReflectFaceColor: {r: 0, g: 0, b: 0, a: 1}
    - _ReflectOutlineColor: {r: 0, g: 0, b: 0, a: 1}
    - _SpecularColor: {r: 1, g: 1, b: 1, a: 1}
    - _UnderlayColor: {r: 0, g: 0, b: 0, a: 0.5}
```

## File: Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Outline.mat
```
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!21 &2100000
Material:
  serializedVersion: 6
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_Name: LiberationSans SDF - Outline
  m_Shader: {fileID: 4800000, guid: fe393ace9b354375a9cb14cdbbc28be4, type: 3}
  m_ShaderKeywords: OUTLINE_ON
  m_LightmapFlags: 5
  m_EnableInstancingVariants: 0
  m_DoubleSidedGI: 0
  m_CustomRenderQueue: -1
  stringTagMap: {}
  disabledShaderPasses: []
  m_SavedProperties:
    serializedVersion: 3
    m_TexEnvs:
    - _BumpMap:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _Cube:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _FaceTex:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _MainTex:
        m_Texture: {fileID: 28684132378477856, guid: 8f586378b4e144a9851e7b34d9b748ee,
          type: 2}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    - _OutlineTex:
        m_Texture: {fileID: 0}
        m_Scale: {x: 1, y: 1}
        m_Offset: {x: 0, y: 0}
    m_Floats:
    - _Ambient: 0.5
    - _Bevel: 0.5
    - _BevelClamp: 0
    - _BevelOffset: 0
    - _BevelRoundness: 0
    - _BevelWidth: 0
    - _BumpFace: 0
    - _BumpOutline: 0
    - _ColorMask: 15
    - _Diffuse: 0.5
    - _FaceDilate: 0.1
    - _FaceUVSpeedX: 0
    - _FaceUVSpeedY: 0
    - _GlowInner: 0.05
    - _GlowOffset: 0
    - _GlowOuter: 0.05
    - _GlowPower: 0.75
    - _GradientScale: 10
    - _LightAngle: 3.1416
    - _MaskSoftnessX: 0
    - _MaskSoftnessY: 0
    - _OutlineSoftness: 0
    - _OutlineUVSpeedX: 0
    - _OutlineUVSpeedY: 0
    - _OutlineWidth: 0.1
    - _PerspectiveFilter: 0.875
    - _Reflectivity: 10
    - _ScaleRatioA: 0.9
    - _ScaleRatioB: 0.73125
    - _ScaleRatioC: 0.64125
    - _ScaleX: 1
    - _ScaleY: 1
    - _ShaderFlags: 0
    - _Sharpness: 0
    - _SpecularPower: 2
    - _Stencil: 0
    - _StencilComp: 8
    - _StencilOp: 0
    - _StencilReadMask: 255
    - _StencilWriteMask: 255
    - _TextureHeight: 1024
    - _TextureWidth: 1024
    - _UnderlayDilate: 0
    - _UnderlayOffsetX: 0
    - _UnderlayOffsetY: 0
    - _UnderlaySoftness: 0
    - _VertexOffsetX: 0
    - _VertexOffsetY: 0
    - _WeightBold: 0.75
    - _WeightNormal: 0
    m_Colors:
    - _ClipRect: {r: -32767, g: -32767, b: 32767, a: 32767}
    - _EnvMatrixRotation: {r: 0, g: 0, b: 0, a: 0}
    - _FaceColor: {r: 1, g: 1, b: 1, a: 1}
    - _GlowColor: {r: 0, g: 1, b: 0, a: 0.5}
    - _MaskCoord: {r: 0, g: 0, b: 32767, a: 32767}
    - _OutlineColor: {r: 0, g: 0, b: 0, a: 1}
    - _ReflectFaceColor: {r: 0, g: 0, b: 0, a: 1}
    - _ReflectOutlineColor: {r: 0, g: 0, b: 0, a: 1}
    - _SpecularColor: {r: 1, g: 1, b: 1, a: 1}
    - _UnderlayColor: {r: 0, g: 0, b: 0, a: 0.5}
```

## File: Assets/TextMesh Pro/Resources/LineBreaking Following Characters.txt
```
)]｝〕〉》」』】〙〗〟’”｠»ヽヾーァィゥェォッャュョヮヵヶぁぃぅぇぉっゃゅょゎゕゖㇰㇱㇲㇳㇴㇵㇶㇷㇸㇹㇺㇻㇼㇽㇾㇿ々〻‐゠–〜?!‼⁇⁈⁉・、%,.:;。！？］）：；＝}¢°"†‡℃〆％，．
```

## File: Assets/TextMesh Pro/Resources/LineBreaking Leading Characters.txt
```
([｛〔〈《「『【〘〖〝‘“｟«$—…‥〳〴〵\［（{£¥"々〇＄￥￦ #
```

## File: Assets/TextMesh Pro/Shaders/TMP_Bitmap-Custom-Atlas.shader
```
Shader "TextMeshPro/Bitmap Custom Atlas" {

Properties {
	_MainTex		("Font Atlas", 2D) = "white" {}
	_FaceTex		("Font Texture", 2D) = "white" {}
	[HDR]_FaceColor	("Text Color", Color) = (1,1,1,1)

	_VertexOffsetX	("Vertex OffsetX", float) = 0
	_VertexOffsetY	("Vertex OffsetY", float) = 0
	_MaskSoftnessX	("Mask SoftnessX", float) = 0
	_MaskSoftnessY	("Mask SoftnessY", float) = 0

	_ClipRect("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
	_Padding		("Padding", float) = 0

	_StencilComp("Stencil Comparison", Float) = 8
	_Stencil("Stencil ID", Float) = 0
	_StencilOp("Stencil Operation", Float) = 0
	_StencilWriteMask("Stencil Write Mask", Float) = 255
	_StencilReadMask("Stencil Read Mask", Float) = 255

	_CullMode("Cull Mode", Float) = 0
	_ColorMask("Color Mask", Float) = 15
}

SubShader{

	Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

	Stencil
	{
		Ref[_Stencil]
		Comp[_StencilComp]
		Pass[_StencilOp]
		ReadMask[_StencilReadMask]
		WriteMask[_StencilWriteMask]
	}


	Lighting Off
	Cull [_CullMode]
	ZTest [unity_GUIZTestMode]
	ZWrite Off
	Fog { Mode Off }
	Blend SrcAlpha OneMinusSrcAlpha
	ColorMask[_ColorMask]

	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#pragma multi_compile __ UNITY_UI_CLIP_RECT
		#pragma multi_compile __ UNITY_UI_ALPHACLIP


		#include "UnityCG.cginc"

		struct appdata_t {
			float4 vertex		: POSITION;
			fixed4 color		: COLOR;
			float2 texcoord0	: TEXCOORD0;
			float2 texcoord1	: TEXCOORD1;
		};

		struct v2f {
			float4	vertex		: SV_POSITION;
			fixed4	color		: COLOR;
			float2	texcoord0	: TEXCOORD0;
			float2	texcoord1	: TEXCOORD1;
			float4	mask		: TEXCOORD2;
		};

		uniform	sampler2D 	_MainTex;
		uniform	sampler2D 	_FaceTex;
		uniform float4		_FaceTex_ST;
		uniform	fixed4		_FaceColor;

		uniform float		_VertexOffsetX;
		uniform float		_VertexOffsetY;
		uniform float4		_ClipRect;
		uniform float		_MaskSoftnessX;
		uniform float		_MaskSoftnessY;

		float2 UnpackUV(float uv)
		{
			float2 output;
			output.x = floor(uv / 4096);
			output.y = uv - 4096 * output.x;

			return output * 0.001953125;
		}

		v2f vert (appdata_t v)
		{
			float4 vert = v.vertex;
			vert.x += _VertexOffsetX;
			vert.y += _VertexOffsetY;

			vert.xy += (vert.w * 0.5) / _ScreenParams.xy;

			float4 vPosition = UnityPixelSnap(UnityObjectToClipPos(vert));

			fixed4 faceColor = v.color;
			faceColor *= _FaceColor;

			v2f OUT;
			OUT.vertex = vPosition;
			OUT.color = faceColor;
			OUT.texcoord0 = v.texcoord0;
			OUT.texcoord1 = TRANSFORM_TEX(UnpackUV(v.texcoord1), _FaceTex);
			float2 pixelSize = vPosition.w;
			pixelSize /= abs(float2(_ScreenParams.x * UNITY_MATRIX_P[0][0], _ScreenParams.y * UNITY_MATRIX_P[1][1]));

			// Clamp _ClipRect to 16bit.
			float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
			OUT.mask = float4(vert.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + pixelSize.xy));

			return OUT;
		}

		fixed4 frag (v2f IN) : SV_Target
		{
			fixed4 color = tex2D(_MainTex, IN.texcoord0) * tex2D(_FaceTex, IN.texcoord1) * IN.color;

			// Alternative implementation to UnityGet2DClipping with support for softness.
			#if UNITY_UI_CLIP_RECT
				half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
				color *= m.x * m.y;
			#endif

			#if UNITY_UI_ALPHACLIP
				clip(color.a - 0.001);
			#endif

			return color;
		}
		ENDCG
	}
}

	CustomEditor "TMPro.EditorUtilities.TMP_BitmapShaderGUI"
}
```

## File: Assets/TextMesh Pro/Shaders/TMP_Bitmap-Mobile.shader
```
Shader "TextMeshPro/Mobile/Bitmap" {

Properties {
	_MainTex		("Font Atlas", 2D) = "white" {}
	[HDR]_Color		("Text Color", Color) = (1,1,1,1)
	_DiffusePower	("Diffuse Power", Range(1.0,4.0)) = 1.0

	_VertexOffsetX("Vertex OffsetX", float) = 0
	_VertexOffsetY("Vertex OffsetY", float) = 0
	_MaskSoftnessX("Mask SoftnessX", float) = 0
	_MaskSoftnessY("Mask SoftnessY", float) = 0

	_ClipRect("Clip Rect", vector) = (-32767, -32767, 32767, 32767)

	_StencilComp("Stencil Comparison", Float) = 8
	_Stencil("Stencil ID", Float) = 0
	_StencilOp("Stencil Operation", Float) = 0
	_StencilWriteMask("Stencil Write Mask", Float) = 255
	_StencilReadMask("Stencil Read Mask", Float) = 255

	_CullMode("Cull Mode", Float) = 0
	_ColorMask("Color Mask", Float) = 15
}

SubShader {

	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

	Stencil
	{
		Ref[_Stencil]
		Comp[_StencilComp]
		Pass[_StencilOp]
		ReadMask[_StencilReadMask]
		WriteMask[_StencilWriteMask]
	}


	Lighting Off
	Cull [_CullMode]
	ZTest [unity_GUIZTestMode]
	ZWrite Off
	Fog { Mode Off }
	Blend SrcAlpha OneMinusSrcAlpha
	ColorMask[_ColorMask]

	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest

		#pragma multi_compile __ UNITY_UI_CLIP_RECT
		#pragma multi_compile __ UNITY_UI_ALPHACLIP


		#include "UnityCG.cginc"

		struct appdata_t {
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord0 : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
		};

		struct v2f {
			float4 vertex		: POSITION;
			fixed4 color		: COLOR;
			float2 texcoord0	: TEXCOORD0;
			float4 mask			: TEXCOORD2;
		};

		sampler2D 	_MainTex;
		fixed4		_Color;
		float		_DiffusePower;

		uniform float		_VertexOffsetX;
		uniform float		_VertexOffsetY;
		uniform float4		_ClipRect;
		uniform float		_MaskSoftnessX;
		uniform float		_MaskSoftnessY;

		v2f vert (appdata_t v)
		{
			v2f OUT;
			float4 vert = v.vertex;
			vert.x += _VertexOffsetX;
			vert.y += _VertexOffsetY;

			vert.xy += (vert.w * 0.5) / _ScreenParams.xy;

			OUT.vertex = UnityPixelSnap(UnityObjectToClipPos(vert));
			OUT.color = v.color;
			OUT.color *= _Color;
			OUT.color.rgb *= _DiffusePower;
			OUT.texcoord0 = v.texcoord0;

			float2 pixelSize = OUT.vertex.w;
			//pixelSize /= abs(float2(_ScreenParams.x * UNITY_MATRIX_P[0][0], _ScreenParams.y * UNITY_MATRIX_P[1][1]));

			// Clamp _ClipRect to 16bit.
			float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
			OUT.mask = float4(vert.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + pixelSize.xy));

			return OUT;
		}

		fixed4 frag (v2f IN) : COLOR
		{
			fixed4 color = fixed4(IN.color.rgb, IN.color.a * tex2D(_MainTex, IN.texcoord0).a);

			// Alternative implementation to UnityGet2DClipping with support for softness.
			#if UNITY_UI_CLIP_RECT
				half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
				color *= m.x * m.y;
			#endif

			#if UNITY_UI_ALPHACLIP
				clip(color.a - 0.001);
			#endif

			return color;
		}
		ENDCG
	}
}

SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Lighting Off Cull Off ZTest Always ZWrite Off Fog { Mode Off }
	Blend SrcAlpha OneMinusSrcAlpha
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord0
	}
	Pass {
		SetTexture [_MainTex] {
			constantColor [_Color] combine constant * primary, constant * texture
		}
	}
}

CustomEditor "TMPro.EditorUtilities.TMP_BitmapShaderGUI"
}
```

## File: Assets/TextMesh Pro/Shaders/TMP_Bitmap.shader
```
Shader "TextMeshPro/Bitmap" {

Properties {
	_MainTex		("Font Atlas", 2D) = "white" {}
	_FaceTex		("Font Texture", 2D) = "white" {}
	[HDR]_FaceColor	("Text Color", Color) = (1,1,1,1)

	_VertexOffsetX	("Vertex OffsetX", float) = 0
	_VertexOffsetY	("Vertex OffsetY", float) = 0
	_MaskSoftnessX	("Mask SoftnessX", float) = 0
	_MaskSoftnessY	("Mask SoftnessY", float) = 0

	_ClipRect("Clip Rect", vector) = (-32767, -32767, 32767, 32767)

	_StencilComp("Stencil Comparison", Float) = 8
	_Stencil("Stencil ID", Float) = 0
	_StencilOp("Stencil Operation", Float) = 0
	_StencilWriteMask("Stencil Write Mask", Float) = 255
	_StencilReadMask("Stencil Read Mask", Float) = 255

	_CullMode("Cull Mode", Float) = 0
	_ColorMask("Color Mask", Float) = 15
}

SubShader{

	Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

	Stencil
	{
		Ref[_Stencil]
		Comp[_StencilComp]
		Pass[_StencilOp]
		ReadMask[_StencilReadMask]
		WriteMask[_StencilWriteMask]
	}


	Lighting Off
	Cull [_CullMode]
	ZTest [unity_GUIZTestMode]
	ZWrite Off
	Fog { Mode Off }
	Blend SrcAlpha OneMinusSrcAlpha
	ColorMask[_ColorMask]

	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#pragma multi_compile __ UNITY_UI_CLIP_RECT
		#pragma multi_compile __ UNITY_UI_ALPHACLIP


		#include "UnityCG.cginc"

		struct appdata_t {
			float4 vertex		: POSITION;
			fixed4 color		: COLOR;
			float2 texcoord0	: TEXCOORD0;
			float2 texcoord1	: TEXCOORD1;
		};

		struct v2f {
			float4	vertex		: SV_POSITION;
			fixed4	color		: COLOR;
			float2	texcoord0	: TEXCOORD0;
			float2	texcoord1	: TEXCOORD1;
			float4	mask		: TEXCOORD2;
		};

		uniform	sampler2D 	_MainTex;
		uniform	sampler2D 	_FaceTex;
		uniform float4		_FaceTex_ST;
		uniform	fixed4		_FaceColor;

		uniform float		_VertexOffsetX;
		uniform float		_VertexOffsetY;
		uniform float4		_ClipRect;
		uniform float		_MaskSoftnessX;
		uniform float		_MaskSoftnessY;

		float2 UnpackUV(float uv)
		{
			float2 output;
			output.x = floor(uv / 4096);
			output.y = uv - 4096 * output.x;

			return output * 0.001953125;
		}

		v2f vert (appdata_t v)
		{
			float4 vert = v.vertex;
			vert.x += _VertexOffsetX;
			vert.y += _VertexOffsetY;

			vert.xy += (vert.w * 0.5) / _ScreenParams.xy;

			float4 vPosition = UnityPixelSnap(UnityObjectToClipPos(vert));

			fixed4 faceColor = v.color;
			faceColor *= _FaceColor;

			v2f OUT;
			OUT.vertex = vPosition;
			OUT.color = faceColor;
			OUT.texcoord0 = v.texcoord0;
			OUT.texcoord1 = TRANSFORM_TEX(UnpackUV(v.texcoord1), _FaceTex);
			float2 pixelSize = vPosition.w;
			pixelSize /= abs(float2(_ScreenParams.x * UNITY_MATRIX_P[0][0], _ScreenParams.y * UNITY_MATRIX_P[1][1]));

			// Clamp _ClipRect to 16bit.
			float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
			OUT.mask = float4(vert.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + pixelSize.xy));

			return OUT;
		}

		fixed4 frag (v2f IN) : SV_Target
		{
			fixed4 color = tex2D(_MainTex, IN.texcoord0);
			color = fixed4 (tex2D(_FaceTex, IN.texcoord1).rgb * IN.color.rgb, IN.color.a * color.a);

			// Alternative implementation to UnityGet2DClipping with support for softness.
			#if UNITY_UI_CLIP_RECT
				half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
				color *= m.x * m.y;
			#endif

			#if UNITY_UI_ALPHACLIP
				clip(color.a - 0.001);
			#endif

			return color;
		}
		ENDCG
	}
}

	CustomEditor "TMPro.EditorUtilities.TMP_BitmapShaderGUI"
}
```

## File: Assets/TextMesh Pro/Shaders/TMP_SDF Overlay.shader
```
Shader "TextMeshPro/Distance Field Overlay" {

Properties {
	_FaceTex			("Face Texture", 2D) = "white" {}
	_FaceUVSpeedX		("Face UV Speed X", Range(-5, 5)) = 0.0
	_FaceUVSpeedY		("Face UV Speed Y", Range(-5, 5)) = 0.0
	[HDR]_FaceColor		("Face Color", Color) = (1,1,1,1)
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0

	[HDR]_OutlineColor	("Outline Color", Color) = (0,0,0,1)
	_OutlineTex			("Outline Texture", 2D) = "white" {}
	_OutlineUVSpeedX	("Outline UV Speed X", Range(-5, 5)) = 0.0
	_OutlineUVSpeedY	("Outline UV Speed Y", Range(-5, 5)) = 0.0
	_OutlineWidth		("Outline Thickness", Range(0, 1)) = 0
	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0

	_Bevel				("Bevel", Range(0,1)) = 0.5
	_BevelOffset		("Bevel Offset", Range(-0.5,0.5)) = 0
	_BevelWidth			("Bevel Width", Range(-.5,0.5)) = 0
	_BevelClamp			("Bevel Clamp", Range(0,1)) = 0
	_BevelRoundness		("Bevel Roundness", Range(0,1)) = 0

	_LightAngle			("Light Angle", Range(0.0, 6.2831853)) = 3.1416
	[HDR]_SpecularColor	("Specular", Color) = (1,1,1,1)
	_SpecularPower		("Specular", Range(0,4)) = 2.0
	_Reflectivity		("Reflectivity", Range(5.0,15.0)) = 10
	_Diffuse			("Diffuse", Range(0,1)) = 0.5
	_Ambient			("Ambient", Range(1,0)) = 0.5

	_BumpMap 			("Normal map", 2D) = "bump" {}
	_BumpOutline		("Bump Outline", Range(0,1)) = 0
	_BumpFace			("Bump Face", Range(0,1)) = 0

	_ReflectFaceColor	("Reflection Color", Color) = (0,0,0,1)
	_ReflectOutlineColor("Reflection Color", Color) = (0,0,0,1)
	_Cube 				("Reflection Cubemap", Cube) = "black" { /* TexGen CubeReflect */ }
	_EnvMatrixRotation	("Texture Rotation", vector) = (0, 0, 0, 0)


	[HDR]_UnderlayColor	("Border Color", Color) = (0,0,0, 0.5)
	_UnderlayOffsetX	("Border OffsetX", Range(-1,1)) = 0
	_UnderlayOffsetY	("Border OffsetY", Range(-1,1)) = 0
	_UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
	_UnderlaySoftness	("Border Softness", Range(0,1)) = 0

	[HDR]_GlowColor		("Color", Color) = (0, 1, 0, 0.5)
	_GlowOffset			("Offset", Range(-1,1)) = 0
	_GlowInner			("Inner", Range(0,1)) = 0.05
	_GlowOuter			("Outer", Range(0,1)) = 0.05
	_GlowPower			("Falloff", Range(1, 0)) = 0.75

	_WeightNormal		("Weight Normal", float) = 0
	_WeightBold			("Weight Bold", float) = 0.5

	_ShaderFlags		("Flags", float) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ScaleRatioB		("Scale RatioB", float) = 1
	_ScaleRatioC		("Scale RatioC", float) = 1

	_MainTex			("Font Atlas", 2D) = "white" {}
	_TextureWidth		("Texture Width", float) = 512
	_TextureHeight		("Texture Height", float) = 512
	_GradientScale		("Gradient Scale", float) = 5.0
	_ScaleX				("Scale X", float) = 1.0
	_ScaleY				("Scale Y", float) = 1.0
	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875
	_Sharpness			("Sharpness", Range(-1,1)) = 0

	_VertexOffsetX		("Vertex OffsetX", float) = 0
	_VertexOffsetY		("Vertex OffsetY", float) = 0

	_MaskCoord			("Mask Coordinates", vector) = (0, 0, 32767, 32767)
	_ClipRect			("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
	_MaskSoftnessX		("Mask SoftnessX", float) = 0
	_MaskSoftnessY		("Mask SoftnessY", float) = 0

	_StencilComp		("Stencil Comparison", Float) = 8
	_Stencil			("Stencil ID", Float) = 0
	_StencilOp			("Stencil Operation", Float) = 0
	_StencilWriteMask	("Stencil Write Mask", Float) = 255
	_StencilReadMask	("Stencil Read Mask", Float) = 255

	_CullMode			("Cull Mode", Float) = 0
	_ColorMask			("Color Mask", Float) = 15
}

SubShader {

	Tags
  {
		"Queue"="Overlay"
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
	}

	Stencil
	{
		Ref [_Stencil]
		Comp [_StencilComp]
		Pass [_StencilOp]
		ReadMask [_StencilReadMask]
		WriteMask [_StencilWriteMask]
	}

	Cull [_CullMode]
	ZWrite Off
	Lighting Off
	Fog { Mode Off }
	ZTest Always
	Blend One OneMinusSrcAlpha
	ColorMask [_ColorMask]

	Pass {
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex VertShader
		#pragma fragment PixShader
		#pragma shader_feature __ BEVEL_ON
		#pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER
		#pragma shader_feature __ GLOW_ON

		#pragma multi_compile __ UNITY_UI_CLIP_RECT
		#pragma multi_compile __ UNITY_UI_ALPHACLIP

		#include "UnityCG.cginc"
		#include "UnityUI.cginc"
		#include "TMPro_Properties.cginc"
		#include "TMPro.cginc"

		struct vertex_t {
			UNITY_VERTEX_INPUT_INSTANCE_ID
			float4	position		: POSITION;
			float3	normal			: NORMAL;
			fixed4	color			: COLOR;
			float2	texcoord0		: TEXCOORD0;
			float2	texcoord1		: TEXCOORD1;
		};


		struct pixel_t {
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
			float4	position		: SV_POSITION;
			fixed4	color			: COLOR;
			float2	atlas			: TEXCOORD0;		// Atlas
			float4	param			: TEXCOORD1;		// alphaClip, scale, bias, weight
			float4	mask			: TEXCOORD2;		// Position in object space(xy), pixel Size(zw)
			float3	viewDir			: TEXCOORD3;

		#if (UNDERLAY_ON || UNDERLAY_INNER)
			float4	texcoord2		: TEXCOORD4;		// u,v, scale, bias
			fixed4	underlayColor	: COLOR1;
		#endif
			float4 textures			: TEXCOORD5;
		};

		// Used by Unity internally to handle Texture Tiling and Offset.
		float4 _FaceTex_ST;
		float4 _OutlineTex_ST;

		pixel_t VertShader(vertex_t input)
		{
			pixel_t output;

			UNITY_INITIALIZE_OUTPUT(pixel_t, output);
			UNITY_SETUP_INSTANCE_ID(input);
			UNITY_TRANSFER_INSTANCE_ID(input,output);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

			float bold = step(input.texcoord1.y, 0);

			float4 vert = input.position;
			vert.x += _VertexOffsetX;
			vert.y += _VertexOffsetY;

			float4 vPosition = UnityObjectToClipPos(vert);

			float2 pixelSize = vPosition.w;
			pixelSize /= float2(_ScaleX, _ScaleY) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
			float scale = rsqrt(dot(pixelSize, pixelSize));
			scale *= abs(input.texcoord1.y) * _GradientScale * (_Sharpness + 1);
			if (UNITY_MATRIX_P[3][3] == 0) scale = lerp(abs(scale) * (1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(input.normal.xyz), normalize(WorldSpaceViewDir(vert)))));

			float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
			weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;

			float bias =(.5 - weight) + (.5 / scale);

			float alphaClip = (1.0 - _OutlineWidth*_ScaleRatioA - _OutlineSoftness*_ScaleRatioA);

		#if GLOW_ON
			alphaClip = min(alphaClip, 1.0 - _GlowOffset * _ScaleRatioB - _GlowOuter * _ScaleRatioB);
		#endif

			alphaClip = alphaClip / 2.0 - ( .5 / scale) - weight;

		#if (UNDERLAY_ON || UNDERLAY_INNER)
			float4 underlayColor = _UnderlayColor;
			underlayColor.rgb *= underlayColor.a;

			float bScale = scale;
			bScale /= 1 + ((_UnderlaySoftness*_ScaleRatioC) * bScale);
			float bBias = (0.5 - weight) * bScale - 0.5 - ((_UnderlayDilate * _ScaleRatioC) * 0.5 * bScale);

			float x = -(_UnderlayOffsetX * _ScaleRatioC) * _GradientScale / _TextureWidth;
			float y = -(_UnderlayOffsetY * _ScaleRatioC) * _GradientScale / _TextureHeight;
			float2 bOffset = float2(x, y);
		#endif

			// Generate UV for the Masking Texture
			float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
			float2 maskUV = (vert.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

			// Support for texture tiling and offset
			float2 textureUV = UnpackUV(input.texcoord1.x);
			float2 faceUV = TRANSFORM_TEX(textureUV, _FaceTex);
			float2 outlineUV = TRANSFORM_TEX(textureUV, _OutlineTex);


			output.position = vPosition;
			output.color = input.color;
			output.atlas =	input.texcoord0;
			output.param =	float4(alphaClip, scale, bias, weight);
			output.mask = half4(vert.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + pixelSize.xy));
			output.viewDir =	mul((float3x3)_EnvMatrix, _WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, vert).xyz);
			#if (UNDERLAY_ON || UNDERLAY_INNER)
			output.texcoord2 = float4(input.texcoord0 + bOffset, bScale, bBias);
			output.underlayColor =	underlayColor;
			#endif
			output.textures = float4(faceUV, outlineUV);

			return output;
		}


		fixed4 PixShader(pixel_t input) : SV_Target
		{
			UNITY_SETUP_INSTANCE_ID(input);

			float c = tex2D(_MainTex, input.atlas).a;

		#ifndef UNDERLAY_ON
			clip(c - input.param.x);
		#endif

			float	scale	= input.param.y;
			float	bias	= input.param.z;
			float	weight	= input.param.w;
			float	sd = (bias - c) * scale;

			float outline = (_OutlineWidth * _ScaleRatioA) * scale;
			float softness = (_OutlineSoftness * _ScaleRatioA) * scale;

			half4 faceColor = _FaceColor;
			half4 outlineColor = _OutlineColor;

			faceColor.rgb *= input.color.rgb;

			faceColor *= tex2D(_FaceTex, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y);
			outlineColor *= tex2D(_OutlineTex, input.textures.zw + float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.y);

			faceColor = GetColor(sd, faceColor, outlineColor, outline, softness);

		#if BEVEL_ON
			float3 dxy = float3(0.5 / _TextureWidth, 0.5 / _TextureHeight, 0);
			float3 n = GetSurfaceNormal(input.atlas, weight, dxy);

			float3 bump = UnpackNormal(tex2D(_BumpMap, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y)).xyz;
			bump *= lerp(_BumpFace, _BumpOutline, saturate(sd + outline * 0.5));
			n = normalize(n- bump);

			float3 light = normalize(float3(sin(_LightAngle), cos(_LightAngle), -1.0));

			float3 col = GetSpecular(n, light);
			faceColor.rgb += col*faceColor.a;
			faceColor.rgb *= 1-(dot(n, light)*_Diffuse);
			faceColor.rgb *= lerp(_Ambient, 1, n.z*n.z);

			fixed4 reflcol = texCUBE(_Cube, reflect(input.viewDir, -n));
			faceColor.rgb += reflcol.rgb * lerp(_ReflectFaceColor.rgb, _ReflectOutlineColor.rgb, saturate(sd + outline * 0.5)) * faceColor.a;
		#endif

		#if UNDERLAY_ON
			float d = tex2D(_MainTex, input.texcoord2.xy).a * input.texcoord2.z;
			faceColor += input.underlayColor * saturate(d - input.texcoord2.w) * (1 - faceColor.a);
		#endif

		#if UNDERLAY_INNER
			float d = tex2D(_MainTex, input.texcoord2.xy).a * input.texcoord2.z;
			faceColor += input.underlayColor * (1 - saturate(d - input.texcoord2.w)) * saturate(1 - sd) * (1 - faceColor.a);
		#endif

		#if GLOW_ON
			float4 glowColor = GetGlowColor(sd, scale);
			faceColor.rgb += glowColor.rgb * glowColor.a;
		#endif

		// Alternative implementation to UnityGet2DClipping with support for softness.
		#if UNITY_UI_CLIP_RECT
			half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * input.mask.zw);
			faceColor *= m.x * m.y;
		#endif

		#if UNITY_UI_ALPHACLIP
			clip(faceColor.a - 0.001);
		#endif

			return faceColor * input.color.a;
		}

		ENDCG
	}
}

Fallback "TextMeshPro/Mobile/Distance Field"
CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
```

## File: Assets/TextMesh Pro/Shaders/TMP_SDF SSD.shader
```
Shader "TextMeshPro/Distance Field SSD" {

Properties {
    _FaceTex            ("Face Texture", 2D) = "white" {}
    _FaceUVSpeedX       ("Face UV Speed X", Range(-5, 5)) = 0.0
    _FaceUVSpeedY       ("Face UV Speed Y", Range(-5, 5)) = 0.0
    [HDR]_FaceColor     ("Face Color", Color) = (1,1,1,1)
    _FaceDilate         ("Face Dilate", Range(-1,1)) = 0

    [HDR]_OutlineColor  ("Outline Color", Color) = (0,0,0,1)
    _OutlineTex         ("Outline Texture", 2D) = "white" {}
    _OutlineUVSpeedX    ("Outline UV Speed X", Range(-5, 5)) = 0.0
    _OutlineUVSpeedY    ("Outline UV Speed Y", Range(-5, 5)) = 0.0
    _OutlineWidth       ("Outline Thickness", Range(0, 1)) = 0
    _OutlineSoftness    ("Outline Softness", Range(0,1)) = 0

    _Bevel              ("Bevel", Range(0,1)) = 0.5
    _BevelOffset        ("Bevel Offset", Range(-0.5,0.5)) = 0
    _BevelWidth         ("Bevel Width", Range(-.5,0.5)) = 0
    _BevelClamp         ("Bevel Clamp", Range(0,1)) = 0
    _BevelRoundness     ("Bevel Roundness", Range(0,1)) = 0

    _LightAngle         ("Light Angle", Range(0.0, 6.2831853)) = 3.1416
    [HDR]_SpecularColor ("Specular", Color) = (1,1,1,1)
    _SpecularPower      ("Specular", Range(0,4)) = 2.0
    _Reflectivity       ("Reflectivity", Range(5.0,15.0)) = 10
    _Diffuse            ("Diffuse", Range(0,1)) = 0.5
    _Ambient            ("Ambient", Range(1,0)) = 0.5

    _BumpMap            ("Normal map", 2D) = "bump" {}
    _BumpOutline        ("Bump Outline", Range(0,1)) = 0
    _BumpFace           ("Bump Face", Range(0,1)) = 0

    _ReflectFaceColor   ("Reflection Color", Color) = (0,0,0,1)
    _ReflectOutlineColor("Reflection Color", Color) = (0,0,0,1)
    _Cube               ("Reflection Cubemap", Cube) = "black" { /* TexGen CubeReflect */ }
    _EnvMatrixRotation  ("Texture Rotation", vector) = (0, 0, 0, 0)


    [HDR]_UnderlayColor ("Border Color", Color) = (0,0,0, 0.5)
    _UnderlayOffsetX    ("Border OffsetX", Range(-1,1)) = 0
    _UnderlayOffsetY    ("Border OffsetY", Range(-1,1)) = 0
    _UnderlayDilate     ("Border Dilate", Range(-1,1)) = 0
    _UnderlaySoftness   ("Border Softness", Range(0,1)) = 0

    [HDR]_GlowColor     ("Color", Color) = (0, 1, 0, 0.5)
    _GlowOffset         ("Offset", Range(-1,1)) = 0
    _GlowInner          ("Inner", Range(0,1)) = 0.05
    _GlowOuter          ("Outer", Range(0,1)) = 0.05
    _GlowPower          ("Falloff", Range(1, 0)) = 0.75

    _WeightNormal       ("Weight Normal", float) = 0
    _WeightBold         ("Weight Bold", float) = 0.5

    _ShaderFlags        ("Flags", float) = 0
    _ScaleRatioA        ("Scale RatioA", float) = 1
    _ScaleRatioB        ("Scale RatioB", float) = 1
    _ScaleRatioC        ("Scale RatioC", float) = 1

    _MainTex            ("Font Atlas", 2D) = "white" {}
    _TextureWidth       ("Texture Width", float) = 512
    _TextureHeight      ("Texture Height", float) = 512
    _GradientScale      ("Gradient Scale", float) = 5.0
    _ScaleX             ("Scale X", float) = 1.0
    _ScaleY             ("Scale Y", float) = 1.0
    _PerspectiveFilter  ("Perspective Correction", Range(0, 1)) = 0.875
    _Sharpness          ("Sharpness", Range(-1,1)) = 0

    _VertexOffsetX      ("Vertex OffsetX", float) = 0
    _VertexOffsetY      ("Vertex OffsetY", float) = 0

    _MaskCoord          ("Mask Coordinates", vector) = (0, 0, 32767, 32767)
    _ClipRect           ("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
    _MaskSoftnessX      ("Mask SoftnessX", float) = 0
    _MaskSoftnessY      ("Mask SoftnessY", float) = 0

    _StencilComp        ("Stencil Comparison", Float) = 8
    _Stencil            ("Stencil ID", Float) = 0
    _StencilOp          ("Stencil Operation", Float) = 0
    _StencilWriteMask   ("Stencil Write Mask", Float) = 255
    _StencilReadMask    ("Stencil Read Mask", Float) = 255

    _CullMode           ("Cull Mode", Float) = 0
    _ColorMask          ("Color Mask", Float) = 15
}

SubShader {
    Tags
    {
        "Queue" = "Transparent"
        "IgnoreProjector" = "True"
        "RenderType" = "Transparent"
    }

    Stencil
    {
        Ref[_Stencil]
        Comp[_StencilComp]
        Pass[_StencilOp]
        ReadMask[_StencilReadMask]
        WriteMask[_StencilWriteMask]
    }

    Cull[_CullMode]
    ZWrite Off
    Lighting Off
    Fog { Mode Off }
    ZTest[unity_GUIZTestMode]
    Blend One OneMinusSrcAlpha
    ColorMask[_ColorMask]

    Pass {
        CGPROGRAM
        #pragma target 3.0
        #pragma vertex VertShader
        #pragma fragment PixShader
        #pragma shader_feature __ BEVEL_ON
        #pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER
        #pragma shader_feature __ GLOW_ON
        #pragma shader_feature __ FORCE_LINEAR

        #pragma multi_compile __ UNITY_UI_CLIP_RECT
        #pragma multi_compile __ UNITY_UI_ALPHACLIP

        #include "UnityCG.cginc"
        #include "UnityUI.cginc"
        #include "TMPro_Properties.cginc"
        #include "TMPro.cginc"

        struct vertex_t {
            UNITY_VERTEX_INPUT_INSTANCE_ID
            float4	position        : POSITION;
            float3	normal          : NORMAL;
            float4	color           : COLOR;
            float2	texcoord0       : TEXCOORD0;
            float2	texcoord1       : TEXCOORD1;
        };


        struct pixel_t {
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
            float4	position        : SV_POSITION;
            float4	color           : COLOR;
            float2	atlas           : TEXCOORD0;
            float	weight          : TEXCOORD1;
            float2	mask            : TEXCOORD2;		// Position in object space(xy)
            float3	viewDir         : TEXCOORD3;

        #if (UNDERLAY_ON || UNDERLAY_INNER)
            float2	texcoord2       : TEXCOORD4;
            float4	underlayColor   : COLOR1;
        #endif
            float4 textures         : TEXCOORD5;
        };

        // Used by Unity internally to handle Texture Tiling and Offset.
        float4 _FaceTex_ST;
        float4 _OutlineTex_ST;

        float4 SRGBToLinear(float4 rgba) {
            return float4(lerp(rgba.rgb / 12.92f, pow((rgba.rgb + 0.055f) / 1.055f, 2.4f), step(0.04045f, rgba.rgb)), rgba.a);
        }

        pixel_t VertShader(vertex_t input)
        {
            pixel_t output;

            UNITY_INITIALIZE_OUTPUT(pixel_t, output);
            UNITY_SETUP_INSTANCE_ID(input);
            UNITY_TRANSFER_INSTANCE_ID(input,output);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

            float bold = step(input.texcoord1.y, 0);

            float4 vert = input.position;
            vert.x += _VertexOffsetX;
            vert.y += _VertexOffsetY;

            float4 vPosition = UnityObjectToClipPos(vert);

            float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
            weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;

        #if (UNDERLAY_ON || UNDERLAY_INNER)
            float4 underlayColor = _UnderlayColor;
            underlayColor.rgb *= underlayColor.a;

            float x = -(_UnderlayOffsetX * _ScaleRatioC) * _GradientScale / _TextureWidth;
            float y = -(_UnderlayOffsetY * _ScaleRatioC) * _GradientScale / _TextureHeight;
            float2 bOffset = float2(x, y);
        #endif

            // Generate UV for the Masking Texture
            float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);

            // Support for texture tiling and offset
            float2 textureUV = UnpackUV(input.texcoord1.x);
            float2 faceUV = TRANSFORM_TEX(textureUV, _FaceTex);
            float2 outlineUV = TRANSFORM_TEX(textureUV, _OutlineTex);

            float4 color = input.color;
        #if (FORCE_LINEAR && !UNITY_COLORSPACE_GAMMA)
            color = SRGBToLinear(input.color);
        #endif

            output.position = vPosition;
            output.color = color;
            output.atlas = input.texcoord0;
            output.weight = weight;
            output.mask = half2(vert.xy * 2 - clampedRect.xy - clampedRect.zw);
            output.viewDir = mul((float3x3)_EnvMatrix, _WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, vert).xyz);
        #if (UNDERLAY_ON || UNDERLAY_INNER)
            output.texcoord2 = input.texcoord0 + bOffset;
            output.underlayColor = underlayColor;
        #endif
            output.textures = float4(faceUV, outlineUV);

            return output;
        }


        fixed4 PixShader(pixel_t input) : SV_Target
        {
            UNITY_SETUP_INSTANCE_ID(input);

            float c = tex2D(_MainTex, input.atlas).a;

            float2 pixelSize = float2(ddx(input.atlas.y), ddy(input.atlas.y));
            pixelSize *= _TextureWidth * .75;
            float scale = rsqrt(dot(pixelSize, pixelSize)) * _GradientScale * (_Sharpness + 1);

            float weight = input.weight;
            float bias = (.5 - weight) + (.5 / scale);
            float sd = (bias - c) * scale;

            float outline = (_OutlineWidth * _ScaleRatioA) * scale;
            float softness = (_OutlineSoftness * _ScaleRatioA) * scale;

            half4 faceColor = _FaceColor;
            half4 outlineColor = _OutlineColor;

            faceColor.rgb *= input.color.rgb;

            faceColor *= tex2D(_FaceTex, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y);
            outlineColor *= tex2D(_OutlineTex, input.textures.zw + float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.y);

            faceColor = GetColor(sd, faceColor, outlineColor, outline, softness);

        #if BEVEL_ON
            float3 dxy = float3(0.5 / _TextureWidth, 0.5 / _TextureHeight, 0);
            float3 n = GetSurfaceNormal(input.atlas, weight, dxy);

            float3 bump = UnpackNormal(tex2D(_BumpMap, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y)).xyz;
            bump *= lerp(_BumpFace, _BumpOutline, saturate(sd + outline * 0.5));
            n = normalize(n - bump);

            float3 light = normalize(float3(sin(_LightAngle), cos(_LightAngle), -1.0));

            float3 col = GetSpecular(n, light);
            faceColor.rgb += col * faceColor.a;
            faceColor.rgb *= 1 - (dot(n, light) * _Diffuse);
            faceColor.rgb *= lerp(_Ambient, 1, n.z * n.z);

            fixed4 reflcol = texCUBE(_Cube, reflect(input.viewDir, -n));
            faceColor.rgb += reflcol.rgb * lerp(_ReflectFaceColor.rgb, _ReflectOutlineColor.rgb, saturate(sd + outline * 0.5)) * faceColor.a;
        #endif

        #if (UNDERLAY_ON || UNDERLAY_INNER)
            float bScale = scale;
            bScale /= 1 + ((_UnderlaySoftness * _ScaleRatioC) * bScale);
            float bBias = (0.5 - weight) * bScale - 0.5 - ((_UnderlayDilate * _ScaleRatioC) * 0.5 * bScale);
        #endif

        #if UNDERLAY_ON
            float d = tex2D(_MainTex, input.texcoord2.xy).a * bScale;
            faceColor += input.underlayColor * saturate(d - bBias) * (1 - faceColor.a);
        #endif

        #if UNDERLAY_INNER
            float d = tex2D(_MainTex, input.texcoord2.xy).a * bScale;
            faceColor += input.underlayColor * (1 - saturate(d - bBias)) * saturate(1 - sd) * (1 - faceColor.a);
        #endif

        #if GLOW_ON
            float4 glowColor = GetGlowColor(sd, scale);
            faceColor.rgb += glowColor.rgb * glowColor.a;
        #endif

            // Alternative implementation to UnityGet2DClipping with support for softness.
        #if UNITY_UI_CLIP_RECT
            float2 maskZW = 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + (1 / scale));
            half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * maskZW);
            faceColor *= m.x * m.y;
        #endif

        #if UNITY_UI_ALPHACLIP
            clip(faceColor.a - 0.001);
        #endif

            return faceColor * input.color.a;
            }

            ENDCG
        }
}

Fallback "TextMeshPro/Mobile/Distance Field"
CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
```

## File: Assets/TextMesh Pro/Shaders/TMP_SDF-Mobile Masking.shader
```
// Simplified SDF shader:
// - No Shading Option (bevel / bump / env map)
// - No Glow Option
// - Softness is applied on both side of the outline

Shader "TextMeshPro/Mobile/Distance Field - Masking" {

Properties {
	[HDR]_FaceColor		("Face Color", Color) = (1,1,1,1)
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0

	[HDR]_OutlineColor	("Outline Color", Color) = (0,0,0,1)
	_OutlineWidth		("Outline Thickness", Range(0,1)) = 0
	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0

	[HDR]_UnderlayColor	("Border Color", Color) = (0,0,0,.5)
	_UnderlayOffsetX 	("Border OffsetX", Range(-1,1)) = 0
	_UnderlayOffsetY 	("Border OffsetY", Range(-1,1)) = 0
	_UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
	_UnderlaySoftness 	("Border Softness", Range(0,1)) = 0

	_WeightNormal		("Weight Normal", float) = 0
	_WeightBold			("Weight Bold", float) = .5

	_ShaderFlags		("Flags", float) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ScaleRatioB		("Scale RatioB", float) = 1
	_ScaleRatioC		("Scale RatioC", float) = 1

	_MainTex			("Font Atlas", 2D) = "white" {}
	_TextureWidth		("Texture Width", float) = 512
	_TextureHeight		("Texture Height", float) = 512
	_GradientScale		("Gradient Scale", float) = 5
	_ScaleX				("Scale X", float) = 1
	_ScaleY				("Scale Y", float) = 1
	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875
	_Sharpness			("Sharpness", Range(-1,1)) = 0

	_VertexOffsetX		("Vertex OffsetX", float) = 0
	_VertexOffsetY		("Vertex OffsetY", float) = 0

	_ClipRect			("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
	_MaskSoftnessX		("Mask SoftnessX", float) = 0
	_MaskSoftnessY		("Mask SoftnessY", float) = 0
	_MaskTex			("Mask Texture", 2D) = "white" {}
	_MaskInverse		("Inverse", float) = 0
	_MaskEdgeColor		("Edge Color", Color) = (1,1,1,1)
	_MaskEdgeSoftness	("Edge Softness", Range(0, 1)) = 0.01
	_MaskWipeControl	("Wipe Position", Range(0, 1)) = 0.5

	_StencilComp		("Stencil Comparison", Float) = 8
	_Stencil			("Stencil ID", Float) = 0
	_StencilOp			("Stencil Operation", Float) = 0
	_StencilWriteMask	("Stencil Write Mask", Float) = 255
	_StencilReadMask	("Stencil Read Mask", Float) = 255

	_CullMode			("Cull Mode", Float) = 0
	_ColorMask			("Color Mask", Float) = 15
}

SubShader {
	Tags
	{
		"Queue"="Transparent"
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
	}


	Stencil
	{
		Ref [_Stencil]
		Comp [_StencilComp]
		Pass [_StencilOp]
		ReadMask [_StencilReadMask]
		WriteMask [_StencilWriteMask]
	}

	Cull [_CullMode]
	ZWrite Off
	Lighting Off
	Fog { Mode Off }
	ZTest [unity_GUIZTestMode]
	Blend One OneMinusSrcAlpha
	ColorMask [_ColorMask]

	Pass {
		CGPROGRAM
		#pragma vertex VertShader
		#pragma fragment PixShader
		#pragma shader_feature __ OUTLINE_ON
		#pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER

		#pragma multi_compile __ UNITY_UI_CLIP_RECT
		#pragma multi_compile __ UNITY_UI_ALPHACLIP


		#include "UnityCG.cginc"
		#include "UnityUI.cginc"
		#include "TMPro_Properties.cginc"

		struct vertex_t {
			float4	vertex			: POSITION;
			float3	normal			: NORMAL;
			fixed4	color			: COLOR;
			float2	texcoord0		: TEXCOORD0;
			float2	texcoord1		: TEXCOORD1;
		};

		struct pixel_t {
			float4	vertex			: SV_POSITION;
			fixed4	faceColor		: COLOR;
			fixed4	outlineColor	: COLOR1;
			float4	texcoord0		: TEXCOORD0;			// Texture UV, Mask UV
			half4	param			: TEXCOORD1;			// Scale(x), BiasIn(y), BiasOut(z), Bias(w)
			half4	mask			: TEXCOORD2;			// Position in clip space(xy), Softness(zw)
		#if (UNDERLAY_ON | UNDERLAY_INNER)
			float4	texcoord1		: TEXCOORD3;			// Texture UV, alpha, reserved
			half2	underlayParam	: TEXCOORD4;			// Scale(x), Bias(y)
		#endif
		};

		float _MaskWipeControl;
		float _MaskEdgeSoftness;
		fixed4 _MaskEdgeColor;
		bool _MaskInverse;

		pixel_t VertShader(vertex_t input)
		{
			float bold = step(input.texcoord1.y, 0);

			float4 vert = input.vertex;
			vert.x += _VertexOffsetX;
			vert.y += _VertexOffsetY;
			float4 vPosition = UnityObjectToClipPos(vert);

			float2 pixelSize = vPosition.w;
			pixelSize /= float2(_ScaleX, _ScaleY) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

			float scale = rsqrt(dot(pixelSize, pixelSize));
			scale *= abs(input.texcoord1.y) * _GradientScale * (_Sharpness + 1);
			if(UNITY_MATRIX_P[3][3] == 0) scale = lerp(abs(scale) * (1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(input.normal.xyz), normalize(WorldSpaceViewDir(vert)))));

			float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
			weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;

			float layerScale = scale;

			scale /= 1 + (_OutlineSoftness * _ScaleRatioA * scale);
			float bias = (0.5 - weight) * scale - 0.5;
			float outline = _OutlineWidth * _ScaleRatioA * 0.5 * scale;

			float opacity = input.color.a;
					#if (UNDERLAY_ON | UNDERLAY_INNER)
					opacity = 1.0;
					#endif

			fixed4 faceColor = fixed4(input.color.rgb, opacity) * _FaceColor;
			faceColor.rgb *= faceColor.a;

			fixed4 outlineColor = _OutlineColor;
			outlineColor.a *= opacity;
			outlineColor.rgb *= outlineColor.a;
			outlineColor = lerp(faceColor, outlineColor, sqrt(min(1.0, (outline * 2))));

		#if (UNDERLAY_ON | UNDERLAY_INNER)

			layerScale /= 1 + ((_UnderlaySoftness * _ScaleRatioC) * layerScale);
			float layerBias = (.5 - weight) * layerScale - .5 - ((_UnderlayDilate * _ScaleRatioC) * .5 * layerScale);

			float x = -(_UnderlayOffsetX * _ScaleRatioC) * _GradientScale / _TextureWidth;
			float y = -(_UnderlayOffsetY * _ScaleRatioC) * _GradientScale / _TextureHeight;
			float2 layerOffset = float2(x, y);
		#endif

			// Generate UV for the Masking Texture
			float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
			float2 maskUV = (vert.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

			// Structure for pixel shader
			pixel_t output = {
				vPosition,
				faceColor,
				outlineColor,
				float4(input.texcoord0.x, input.texcoord0.y, maskUV.x, maskUV.y),
				half4(scale, bias - outline, bias + outline, bias),
				half4(vert.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + pixelSize.xy)),
			#if (UNDERLAY_ON | UNDERLAY_INNER)
				float4(input.texcoord0 + layerOffset, input.color.a, 0),
				half2(layerScale, layerBias),
			#endif
			};

			return output;
		}


		// PIXEL SHADER
		fixed4 PixShader(pixel_t input) : SV_Target
		{
			half d = tex2D(_MainTex, input.texcoord0.xy).a * input.param.x;
			half4 c = input.faceColor * saturate(d - input.param.w);

		#ifdef OUTLINE_ON
			c = lerp(input.outlineColor, input.faceColor, saturate(d - input.param.z));
			c *= saturate(d - input.param.y);
		#endif

		#if UNDERLAY_ON
			d = tex2D(_MainTex, input.texcoord1.xy).a * input.underlayParam.x;
			c += float4(_UnderlayColor.rgb * _UnderlayColor.a, _UnderlayColor.a) * saturate(d - input.underlayParam.y) * (1 - c.a);
		#endif

		#if UNDERLAY_INNER
			half sd = saturate(d - input.param.z);
			d = tex2D(_MainTex, input.texcoord1.xy).a * input.underlayParam.x;
			c += float4(_UnderlayColor.rgb * _UnderlayColor.a, _UnderlayColor.a) * (1 - saturate(d - input.underlayParam.y)) * sd * (1 - c.a);
		#endif

		// Alternative implementation to UnityGet2DClipping with support for softness.
		//#if UNITY_UI_CLIP_RECT
			half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * input.mask.zw);
			c *= m.x * m.y;
		//#endif

		float a = abs(_MaskInverse - tex2D(_MaskTex, input.texcoord0.zw).a);
		float t = a + (1 - _MaskWipeControl) * _MaskEdgeSoftness - _MaskWipeControl;
		a = saturate(t / _MaskEdgeSoftness);
		c.rgb = lerp(_MaskEdgeColor.rgb*c.a, c.rgb, a);
		c *= a;

		#if (UNDERLAY_ON | UNDERLAY_INNER)
			c *= input.texcoord1.z;
		#endif

    #if UNITY_UI_ALPHACLIP
			clip(c.a - 0.001);
		#endif

			return c;
		}
		ENDCG
	}
}

CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
```

## File: Assets/TextMesh Pro/Shaders/TMP_SDF-Mobile Overlay.shader
```
// Simplified SDF shader:
// - No Shading Option (bevel / bump / env map)
// - No Glow Option
// - Softness is applied on both side of the outline

Shader "TextMeshPro/Mobile/Distance Field Overlay" {

Properties {
	[HDR]_FaceColor		("Face Color", Color) = (1,1,1,1)
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0

	[HDR]_OutlineColor	("Outline Color", Color) = (0,0,0,1)
	_OutlineWidth		("Outline Thickness", Range(0,1)) = 0
	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0

	[HDR]_UnderlayColor	("Border Color", Color) = (0,0,0,.5)
	_UnderlayOffsetX 	("Border OffsetX", Range(-1,1)) = 0
	_UnderlayOffsetY 	("Border OffsetY", Range(-1,1)) = 0
	_UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
	_UnderlaySoftness 	("Border Softness", Range(0,1)) = 0

	_WeightNormal		("Weight Normal", float) = 0
	_WeightBold			("Weight Bold", float) = .5

	_ShaderFlags		("Flags", float) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ScaleRatioB		("Scale RatioB", float) = 1
	_ScaleRatioC		("Scale RatioC", float) = 1

	_MainTex			("Font Atlas", 2D) = "white" {}
	_TextureWidth		("Texture Width", float) = 512
	_TextureHeight		("Texture Height", float) = 512
	_GradientScale		("Gradient Scale", float) = 5
	_ScaleX				("Scale X", float) = 1
	_ScaleY				("Scale Y", float) = 1
	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875
	_Sharpness			("Sharpness", Range(-1,1)) = 0

	_VertexOffsetX		("Vertex OffsetX", float) = 0
	_VertexOffsetY		("Vertex OffsetY", float) = 0

	_ClipRect			("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
	_MaskSoftnessX		("Mask SoftnessX", float) = 0
	_MaskSoftnessY		("Mask SoftnessY", float) = 0

	_StencilComp		("Stencil Comparison", Float) = 8
	_Stencil			("Stencil ID", Float) = 0
	_StencilOp			("Stencil Operation", Float) = 0
	_StencilWriteMask	("Stencil Write Mask", Float) = 255
	_StencilReadMask	("Stencil Read Mask", Float) = 255

	_CullMode			("Cull Mode", Float) = 0
	_ColorMask			("Color Mask", Float) = 15
}

SubShader {
	Tags
  {
		"Queue"="Overlay"
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
	}


	Stencil
	{
		Ref [_Stencil]
		Comp [_StencilComp]
		Pass [_StencilOp]
		ReadMask [_StencilReadMask]
		WriteMask [_StencilWriteMask]
	}

	Cull [_CullMode]
	ZWrite Off
	Lighting Off
	Fog { Mode Off }
	ZTest Always
	Blend One OneMinusSrcAlpha
	ColorMask [_ColorMask]

	Pass {
		CGPROGRAM
		#pragma vertex VertShader
		#pragma fragment PixShader
		#pragma shader_feature __ OUTLINE_ON
		#pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER

		#pragma multi_compile __ UNITY_UI_CLIP_RECT
		#pragma multi_compile __ UNITY_UI_ALPHACLIP

		#include "UnityCG.cginc"
		#include "UnityUI.cginc"
		#include "TMPro_Properties.cginc"

		struct vertex_t {
			UNITY_VERTEX_INPUT_INSTANCE_ID
			float4	vertex			: POSITION;
			float3	normal			: NORMAL;
			fixed4	color			: COLOR;
			float2	texcoord0		: TEXCOORD0;
			float2	texcoord1		: TEXCOORD1;
		};

		struct pixel_t {
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
			float4	vertex			: SV_POSITION;
			fixed4	faceColor		: COLOR;
			fixed4	outlineColor	: COLOR1;
			float4	texcoord0		: TEXCOORD0;			// Texture UV, Mask UV
			half4	param			: TEXCOORD1;			// Scale(x), BiasIn(y), BiasOut(z), Bias(w)
			half4	mask			: TEXCOORD2;			// Position in clip space(xy), Softness(zw)
		#if (UNDERLAY_ON | UNDERLAY_INNER)
			float4	texcoord1		: TEXCOORD3;			// Texture UV, alpha, reserved
			half2	underlayParam	: TEXCOORD4;			// Scale(x), Bias(y)
		#endif
		};


		pixel_t VertShader(vertex_t input)
		{
			pixel_t output;

			UNITY_INITIALIZE_OUTPUT(pixel_t, output);
			UNITY_SETUP_INSTANCE_ID(input);
			UNITY_TRANSFER_INSTANCE_ID(input, output);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

			float bold = step(input.texcoord1.y, 0);

			float4 vert = input.vertex;
			vert.x += _VertexOffsetX;
			vert.y += _VertexOffsetY;
			float4 vPosition = UnityObjectToClipPos(vert);

			float2 pixelSize = vPosition.w;
			pixelSize /= float2(_ScaleX, _ScaleY) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

			float scale = rsqrt(dot(pixelSize, pixelSize));
			scale *= abs(input.texcoord1.y) * _GradientScale * (_Sharpness + 1);
			if(UNITY_MATRIX_P[3][3] == 0) scale = lerp(abs(scale) * (1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(input.normal.xyz), normalize(WorldSpaceViewDir(vert)))));

			float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
			weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;

			float layerScale = scale;

			scale /= 1 + (_OutlineSoftness * _ScaleRatioA * scale);
			float bias = (0.5 - weight) * scale - 0.5;
			float outline = _OutlineWidth * _ScaleRatioA * 0.5 * scale;

			float opacity = input.color.a;
		#if (UNDERLAY_ON | UNDERLAY_INNER)
				opacity = 1.0;
		#endif

			fixed4 faceColor = fixed4(input.color.rgb, opacity) * _FaceColor;
			faceColor.rgb *= faceColor.a;

			fixed4 outlineColor = _OutlineColor;
			outlineColor.a *= opacity;
			outlineColor.rgb *= outlineColor.a;
			outlineColor = lerp(faceColor, outlineColor, sqrt(min(1.0, (outline * 2))));

		#if (UNDERLAY_ON | UNDERLAY_INNER)
			layerScale /= 1 + ((_UnderlaySoftness * _ScaleRatioC) * layerScale);
			float layerBias = (.5 - weight) * layerScale - .5 - ((_UnderlayDilate * _ScaleRatioC) * .5 * layerScale);

			float x = -(_UnderlayOffsetX * _ScaleRatioC) * _GradientScale / _TextureWidth;
			float y = -(_UnderlayOffsetY * _ScaleRatioC) * _GradientScale / _TextureHeight;
			float2 layerOffset = float2(x, y);
		#endif

			// Generate UV for the Masking Texture
			float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
			float2 maskUV = (vert.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

			// Populate structure for pixel shader
			output.vertex = vPosition;
			output.faceColor = faceColor;
			output.outlineColor = outlineColor;
			output.texcoord0 = float4(input.texcoord0.x, input.texcoord0.y, maskUV.x, maskUV.y);
			output.param = half4(scale, bias - outline, bias + outline, bias);
			output.mask = half4(vert.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + pixelSize.xy));
			#if (UNDERLAY_ON || UNDERLAY_INNER)
			output.texcoord1 = float4(input.texcoord0 + layerOffset, input.color.a, 0);
			output.underlayParam = half2(layerScale, layerBias);
			#endif

			return output;
		}


		// PIXEL SHADER
		fixed4 PixShader(pixel_t input) : SV_Target
		{
			UNITY_SETUP_INSTANCE_ID(input);

			half d = tex2D(_MainTex, input.texcoord0.xy).a * input.param.x;
			half4 c = input.faceColor * saturate(d - input.param.w);

		#ifdef OUTLINE_ON
			c = lerp(input.outlineColor, input.faceColor, saturate(d - input.param.z));
			c *= saturate(d - input.param.y);
		#endif

		#if UNDERLAY_ON
			d = tex2D(_MainTex, input.texcoord1.xy).a * input.underlayParam.x;
			c += float4(_UnderlayColor.rgb * _UnderlayColor.a, _UnderlayColor.a) * saturate(d - input.underlayParam.y) * (1 - c.a);
		#endif

		#if UNDERLAY_INNER
			half sd = saturate(d - input.param.z);
			d = tex2D(_MainTex, input.texcoord1.xy).a * input.underlayParam.x;
			c += float4(_UnderlayColor.rgb * _UnderlayColor.a, _UnderlayColor.a) * (1 - saturate(d - input.underlayParam.y)) * sd * (1 - c.a);
		#endif

		// Alternative implementation to UnityGet2DClipping with support for softness.
		#if UNITY_UI_CLIP_RECT
			half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * input.mask.zw);
			c *= m.x * m.y;
		#endif

		#if (UNDERLAY_ON | UNDERLAY_INNER)
			c *= input.texcoord1.z;
		#endif

    #if UNITY_UI_ALPHACLIP
			clip(c.a - 0.001);
		#endif

			return c;
		}
		ENDCG
	}
}

CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
```

## File: Assets/TextMesh Pro/Shaders/TMP_SDF-Mobile SSD.shader
```
// Simplified SDF shader:
// - No Shading Option (bevel / bump / env map)
// - No Glow Option
// - Softness is applied on both side of the outline

Shader "TextMeshPro/Mobile/Distance Field SSD" {

Properties {
	[HDR]_FaceColor		("Face Color", Color) = (1,1,1,1)
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0

	[HDR]_OutlineColor	("Outline Color", Color) = (0,0,0,1)
	_OutlineWidth		("Outline Thickness", Range(0,1)) = 0
	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0

	[HDR]_UnderlayColor		("Border Color", Color) = (0,0,0,.5)
	_UnderlayOffsetX 	("Border OffsetX", Range(-1,1)) = 0
	_UnderlayOffsetY 	("Border OffsetY", Range(-1,1)) = 0
	_UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
	_UnderlaySoftness 	("Border Softness", Range(0,1)) = 0

	_WeightNormal		("Weight Normal", float) = 0
	_WeightBold			("Weight Bold", float) = .5

	_ShaderFlags		("Flags", float) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ScaleRatioB		("Scale RatioB", float) = 1
	_ScaleRatioC		("Scale RatioC", float) = 1

	_MainTex			("Font Atlas", 2D) = "white" {}
	_TextureWidth		("Texture Width", float) = 512
	_TextureHeight		("Texture Height", float) = 512
	_GradientScale		("Gradient Scale", float) = 5
	_ScaleX				("Scale X", float) = 1
	_ScaleY				("Scale Y", float) = 1
	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875
	_Sharpness			("Sharpness", Range(-1,1)) = 0

	_VertexOffsetX		("Vertex OffsetX", float) = 0
	_VertexOffsetY		("Vertex OffsetY", float) = 0

	_ClipRect			("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
	_MaskSoftnessX		("Mask SoftnessX", float) = 0
	_MaskSoftnessY		("Mask SoftnessY", float) = 0
	_MaskTex			("Mask Texture", 2D) = "white" {}
	_MaskInverse		("Inverse", float) = 0
	_MaskEdgeColor		("Edge Color", Color) = (1,1,1,1)
	_MaskEdgeSoftness	("Edge Softness", Range(0, 1)) = 0.01
	_MaskWipeControl	("Wipe Position", Range(0, 1)) = 0.5

	_StencilComp		("Stencil Comparison", Float) = 8
	_Stencil			("Stencil ID", Float) = 0
	_StencilOp			("Stencil Operation", Float) = 0
	_StencilWriteMask	("Stencil Write Mask", Float) = 255
	_StencilReadMask	("Stencil Read Mask", Float) = 255

    _CullMode           ("Cull Mode", Float) = 0
	_ColorMask			("Color Mask", Float) = 15
}

SubShader {
	Tags {
		"Queue"="Transparent"
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
	}

	Stencil
	{
		Ref [_Stencil]
		Comp [_StencilComp]
		Pass [_StencilOp]
		ReadMask [_StencilReadMask]
		WriteMask [_StencilWriteMask]
	}

	Cull [_CullMode]
	ZWrite Off
	Lighting Off
	Fog { Mode Off }
	ZTest [unity_GUIZTestMode]
	Blend One OneMinusSrcAlpha
	ColorMask [_ColorMask]

	Pass {
		CGPROGRAM
		#pragma vertex VertShader
		#pragma fragment PixShader
		#pragma shader_feature __ OUTLINE_ON
		#pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER

		#pragma multi_compile __ UNITY_UI_CLIP_RECT
		#pragma multi_compile __ UNITY_UI_ALPHACLIP

		#include "UnityCG.cginc"
		#include "UnityUI.cginc"
		#include "TMPro_Properties.cginc"

		#include "TMPro_Mobile.cginc"

		ENDCG
	}
}

CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
```

## File: Assets/TextMesh Pro/Shaders/TMP_SDF-Mobile.shader
```
// Simplified SDF shader:
// - No Shading Option (bevel / bump / env map)
// - No Glow Option
// - Softness is applied on both side of the outline

Shader "TextMeshPro/Mobile/Distance Field" {

Properties {
	[HDR]_FaceColor     ("Face Color", Color) = (1,1,1,1)
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0

	[HDR]_OutlineColor	("Outline Color", Color) = (0,0,0,1)
	_OutlineWidth		("Outline Thickness", Range(0,1)) = 0
	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0

	[HDR]_UnderlayColor	("Border Color", Color) = (0,0,0,.5)
	_UnderlayOffsetX 	("Border OffsetX", Range(-1,1)) = 0
	_UnderlayOffsetY 	("Border OffsetY", Range(-1,1)) = 0
	_UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
	_UnderlaySoftness 	("Border Softness", Range(0,1)) = 0

	_WeightNormal		("Weight Normal", float) = 0
	_WeightBold			("Weight Bold", float) = .5

	_ShaderFlags		("Flags", float) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ScaleRatioB		("Scale RatioB", float) = 1
	_ScaleRatioC		("Scale RatioC", float) = 1

	_MainTex			("Font Atlas", 2D) = "white" {}
	_TextureWidth		("Texture Width", float) = 512
	_TextureHeight		("Texture Height", float) = 512
	_GradientScale		("Gradient Scale", float) = 5
	_ScaleX				("Scale X", float) = 1
	_ScaleY				("Scale Y", float) = 1
	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875
	_Sharpness			("Sharpness", Range(-1,1)) = 0

	_VertexOffsetX		("Vertex OffsetX", float) = 0
	_VertexOffsetY		("Vertex OffsetY", float) = 0

	_ClipRect			("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
	_MaskSoftnessX		("Mask SoftnessX", float) = 0
	_MaskSoftnessY		("Mask SoftnessY", float) = 0

	_StencilComp		("Stencil Comparison", Float) = 8
	_Stencil			("Stencil ID", Float) = 0
	_StencilOp			("Stencil Operation", Float) = 0
	_StencilWriteMask	("Stencil Write Mask", Float) = 255
	_StencilReadMask	("Stencil Read Mask", Float) = 255

	_CullMode			("Cull Mode", Float) = 0
	_ColorMask			("Color Mask", Float) = 15
}

SubShader {
	Tags
	{
		"Queue"="Transparent"
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
	}


	Stencil
	{
		Ref [_Stencil]
		Comp [_StencilComp]
		Pass [_StencilOp]
		ReadMask [_StencilReadMask]
		WriteMask [_StencilWriteMask]
	}

	Cull [_CullMode]
	ZWrite Off
	Lighting Off
	Fog { Mode Off }
	ZTest [unity_GUIZTestMode]
	Blend One OneMinusSrcAlpha
	ColorMask [_ColorMask]

	Pass {
		CGPROGRAM
		#pragma vertex VertShader
		#pragma fragment PixShader
		#pragma shader_feature __ OUTLINE_ON
		#pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER

		#pragma multi_compile __ UNITY_UI_CLIP_RECT
		#pragma multi_compile __ UNITY_UI_ALPHACLIP

		#include "UnityCG.cginc"
		#include "UnityUI.cginc"
		#include "TMPro_Properties.cginc"

		struct vertex_t {
			UNITY_VERTEX_INPUT_INSTANCE_ID
			float4	vertex			: POSITION;
			float3	normal			: NORMAL;
			fixed4	color			: COLOR;
			float2	texcoord0		: TEXCOORD0;
			float2	texcoord1		: TEXCOORD1;
		};

		struct pixel_t {
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
			float4	vertex			: SV_POSITION;
			fixed4	faceColor		: COLOR;
			fixed4	outlineColor	: COLOR1;
			float4	texcoord0		: TEXCOORD0;			// Texture UV, Mask UV
			half4	param			: TEXCOORD1;			// Scale(x), BiasIn(y), BiasOut(z), Bias(w)
			half4	mask			: TEXCOORD2;			// Position in clip space(xy), Softness(zw)
			#if (UNDERLAY_ON | UNDERLAY_INNER)
			float4	texcoord1		: TEXCOORD3;			// Texture UV, alpha, reserved
			half2	underlayParam	: TEXCOORD4;			// Scale(x), Bias(y)
			#endif
		};


		pixel_t VertShader(vertex_t input)
		{
			pixel_t output;

			UNITY_INITIALIZE_OUTPUT(pixel_t, output);
			UNITY_SETUP_INSTANCE_ID(input);
			UNITY_TRANSFER_INSTANCE_ID(input, output);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

			float bold = step(input.texcoord1.y, 0);

			float4 vert = input.vertex;
			vert.x += _VertexOffsetX;
			vert.y += _VertexOffsetY;
			float4 vPosition = UnityObjectToClipPos(vert);

			float2 pixelSize = vPosition.w;
			pixelSize /= float2(_ScaleX, _ScaleY) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

			float scale = rsqrt(dot(pixelSize, pixelSize));
			scale *= abs(input.texcoord1.y) * _GradientScale * (_Sharpness + 1);
			if(UNITY_MATRIX_P[3][3] == 0) scale = lerp(abs(scale) * (1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(input.normal.xyz), normalize(WorldSpaceViewDir(vert)))));

			float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
			weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;

			float layerScale = scale;

			scale /= 1 + (_OutlineSoftness * _ScaleRatioA * scale);
			float bias = (0.5 - weight) * scale - 0.5;
			float outline = _OutlineWidth * _ScaleRatioA * 0.5 * scale;

			float opacity = input.color.a;
			#if (UNDERLAY_ON | UNDERLAY_INNER)
			opacity = 1.0;
			#endif

			fixed4 faceColor = fixed4(input.color.rgb, opacity) * _FaceColor;
			faceColor.rgb *= faceColor.a;

			fixed4 outlineColor = _OutlineColor;
			outlineColor.a *= opacity;
			outlineColor.rgb *= outlineColor.a;
			outlineColor = lerp(faceColor, outlineColor, sqrt(min(1.0, (outline * 2))));

			#if (UNDERLAY_ON | UNDERLAY_INNER)
			layerScale /= 1 + ((_UnderlaySoftness * _ScaleRatioC) * layerScale);
			float layerBias = (.5 - weight) * layerScale - .5 - ((_UnderlayDilate * _ScaleRatioC) * .5 * layerScale);

			float x = -(_UnderlayOffsetX * _ScaleRatioC) * _GradientScale / _TextureWidth;
			float y = -(_UnderlayOffsetY * _ScaleRatioC) * _GradientScale / _TextureHeight;
			float2 layerOffset = float2(x, y);
			#endif

			// Generate UV for the Masking Texture
			float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
			float2 maskUV = (vert.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

			// Populate structure for pixel shader
			output.vertex = vPosition;
			output.faceColor = faceColor;
			output.outlineColor = outlineColor;
			output.texcoord0 = float4(input.texcoord0.x, input.texcoord0.y, maskUV.x, maskUV.y);
			output.param = half4(scale, bias - outline, bias + outline, bias);
			output.mask = half4(vert.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + pixelSize.xy));
			#if (UNDERLAY_ON || UNDERLAY_INNER)
			output.texcoord1 = float4(input.texcoord0 + layerOffset, input.color.a, 0);
			output.underlayParam = half2(layerScale, layerBias);
			#endif

			return output;
		}


		// PIXEL SHADER
		fixed4 PixShader(pixel_t input) : SV_Target
		{
			UNITY_SETUP_INSTANCE_ID(input);

			half d = tex2D(_MainTex, input.texcoord0.xy).a * input.param.x;
			half4 c = input.faceColor * saturate(d - input.param.w);

			#ifdef OUTLINE_ON
			c = lerp(input.outlineColor, input.faceColor, saturate(d - input.param.z));
			c *= saturate(d - input.param.y);
			#endif

			#if UNDERLAY_ON
			d = tex2D(_MainTex, input.texcoord1.xy).a * input.underlayParam.x;
			c += float4(_UnderlayColor.rgb * _UnderlayColor.a, _UnderlayColor.a) * saturate(d - input.underlayParam.y) * (1 - c.a);
			#endif

			#if UNDERLAY_INNER
			half sd = saturate(d - input.param.z);
			d = tex2D(_MainTex, input.texcoord1.xy).a * input.underlayParam.x;
			c += float4(_UnderlayColor.rgb * _UnderlayColor.a, _UnderlayColor.a) * (1 - saturate(d - input.underlayParam.y)) * sd * (1 - c.a);
			#endif

			// Alternative implementation to UnityGet2DClipping with support for softness.
			#if UNITY_UI_CLIP_RECT
			half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * input.mask.zw);
			c *= m.x * m.y;
			#endif

			#if (UNDERLAY_ON | UNDERLAY_INNER)
			c *= input.texcoord1.z;
			#endif

			#if UNITY_UI_ALPHACLIP
			clip(c.a - 0.001);
			#endif

			return c;
		}
		ENDCG
	}
}

CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
```

## File: Assets/TextMesh Pro/Shaders/TMP_SDF-Surface-Mobile.shader
```
// Simplified version of the SDF Surface shader :
// - No support for Bevel, Bump or envmap
// - Diffuse only lighting
// - Fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "TextMeshPro/Mobile/Distance Field (Surface)" {

Properties {
	_FaceTex			("Fill Texture", 2D) = "white" {}
	[HDR]_FaceColor		("Fill Color", Color) = (1,1,1,1)
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0

	[HDR]_OutlineColor	("Outline Color", Color) = (0,0,0,1)
	_OutlineTex			("Outline Texture", 2D) = "white" {}
	_OutlineWidth		("Outline Thickness", Range(0, 1)) = 0
	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0

	[HDR]_GlowColor		("Color", Color) = (0, 1, 0, 0.5)
	_GlowOffset			("Offset", Range(-1,1)) = 0
	_GlowInner			("Inner", Range(0,1)) = 0.05
	_GlowOuter			("Outer", Range(0,1)) = 0.05
	_GlowPower			("Falloff", Range(1, 0)) = 0.75

	_WeightNormal		("Weight Normal", float) = 0
	_WeightBold			("Weight Bold", float) = 0.5

	// Should not be directly exposed to the user
	_ShaderFlags		("Flags", float) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ScaleRatioB		("Scale RatioB", float) = 1
	_ScaleRatioC		("Scale RatioC", float) = 1

	_MainTex			("Font Atlas", 2D) = "white" {}
	_TextureWidth		("Texture Width", float) = 512
	_TextureHeight		("Texture Height", float) = 512
	_GradientScale		("Gradient Scale", float) = 5.0
	_ScaleX				("Scale X", float) = 1.0
	_ScaleY				("Scale Y", float) = 1.0
	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875
	_Sharpness			("Sharpness", Range(-1,1)) = 0

	_VertexOffsetX		("Vertex OffsetX", float) = 0
	_VertexOffsetY		("Vertex OffsetY", float) = 0

	_CullMode			("Cull Mode", Float) = 0
	//_MaskCoord		("Mask Coords", vector) = (0,0,0,0)
	//_MaskSoftness		("Mask Softness", float) = 0
}

SubShader {

	Tags {
		"Queue"="Transparent"
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
	}

	LOD 300
	Cull [_CullMode]

	CGPROGRAM
	#pragma surface PixShader Lambert alpha:blend vertex:VertShader noforwardadd nolightmap nodirlightmap
	#pragma target 3.0
	#pragma shader_feature __ GLOW_ON

	#include "TMPro_Properties.cginc"
	#include "TMPro.cginc"

	half _FaceShininess;
	half _OutlineShininess;

	struct Input
	{
		fixed4	color		: COLOR;
		float2	uv_MainTex;
		float2	uv2_FaceTex;
		float2  uv2_OutlineTex;
		float2	param;					// Weight, Scale
		float3	viewDirEnv;
	};

	#include "TMPro_Surface.cginc"

	ENDCG

	// Pass to render object as a shadow caster
	Pass
	{
		Name "Caster"
		Tags { "LightMode" = "ShadowCaster" }
		Offset 1, 1

		Fog {Mode Off}
		ZWrite On ZTest LEqual Cull Off

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_shadowcaster
		#include "UnityCG.cginc"

		struct v2f {
			V2F_SHADOW_CASTER;
			float2	uv			: TEXCOORD1;
			float2	uv2			: TEXCOORD3;
			float	alphaClip	: TEXCOORD2;
		};

		uniform float4 _MainTex_ST;
		uniform float4 _OutlineTex_ST;
		float _OutlineWidth;
		float _FaceDilate;
		float _ScaleRatioA;

		v2f vert( appdata_base v )
		{
			v2f o;
			TRANSFER_SHADOW_CASTER(o)
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.uv2 = TRANSFORM_TEX(v.texcoord, _OutlineTex);
			o.alphaClip = o.alphaClip = (1.0 - _OutlineWidth * _ScaleRatioA - _FaceDilate * _ScaleRatioA) / 2;
			return o;
		}

		uniform sampler2D _MainTex;

		float4 frag(v2f i) : COLOR
		{
			fixed4 texcol = tex2D(_MainTex, i.uv).a;
			clip(texcol.a - i.alphaClip);
			SHADOW_CASTER_FRAGMENT(i)
		}
		ENDCG
	}
}

CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
```

## File: Assets/TextMesh Pro/Shaders/TMP_SDF-Surface.shader
```
Shader "TextMeshPro/Distance Field (Surface)" {

Properties {
	_FaceTex			("Fill Texture", 2D) = "white" {}
	_FaceUVSpeedX		("Face UV Speed X", Range(-5, 5)) = 0.0
	_FaceUVSpeedY		("Face UV Speed Y", Range(-5, 5)) = 0.0
	[HDR]_FaceColor		("Fill Color", Color) = (1,1,1,1)
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0

	[HDR]_OutlineColor	("Outline Color", Color) = (0,0,0,1)
	_OutlineTex			("Outline Texture", 2D) = "white" {}
	_OutlineUVSpeedX	("Outline UV Speed X", Range(-5, 5)) = 0.0
	_OutlineUVSpeedY	("Outline UV Speed Y", Range(-5, 5)) = 0.0
	_OutlineWidth		("Outline Thickness", Range(0, 1)) = 0
	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0

	_Bevel				("Bevel", Range(0,1)) = 0.5
	_BevelOffset		("Bevel Offset", Range(-0.5,0.5)) = 0
	_BevelWidth			("Bevel Width", Range(-.5,0.5)) = 0
	_BevelClamp			("Bevel Clamp", Range(0,1)) = 0
	_BevelRoundness		("Bevel Roundness", Range(0,1)) = 0

	_BumpMap 			("Normalmap", 2D) = "bump" {}
	_BumpOutline		("Bump Outline", Range(0,1)) = 0.5
	_BumpFace			("Bump Face", Range(0,1)) = 0.5

	_ReflectFaceColor	    ("Face Color", Color) = (0,0,0,1)
	_ReflectOutlineColor	("Outline Color", Color) = (0,0,0,1)
	_Cube 					("Reflection Cubemap", Cube) = "black" { /* TexGen CubeReflect */ }
	_EnvMatrixRotation  	("Texture Rotation", vector) = (0, 0, 0, 0)
	[HDR]_SpecColor		    ("Specular Color", Color) = (0,0,0,1)

	_FaceShininess		("Face Shininess", Range(0,1)) = 0
	_OutlineShininess	("Outline Shininess", Range(0,1)) = 0

	[HDR]_GlowColor		("Color", Color) = (0, 1, 0, 0.5)
	_GlowOffset			("Offset", Range(-1,1)) = 0
	_GlowInner			("Inner", Range(0,1)) = 0.05
	_GlowOuter			("Outer", Range(0,1)) = 0.05
	_GlowPower			("Falloff", Range(1, 0)) = 0.75

	_WeightNormal		("Weight Normal", float) = 0
	_WeightBold			("Weight Bold", float) = 0.5

	// Should not be directly exposed to the user
	_ShaderFlags		("Flags", float) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ScaleRatioB		("Scale RatioB", float) = 1
	_ScaleRatioC		("Scale RatioC", float) = 1

	_MainTex			("Font Atlas", 2D) = "white" {}
	_TextureWidth		("Texture Width", float) = 512
	_TextureHeight		("Texture Height", float) = 512
	_GradientScale		("Gradient Scale", float) = 5.0
	_ScaleX				("Scale X", float) = 1.0
	_ScaleY				("Scale Y", float) = 1.0
	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875
	_Sharpness			("Sharpness", Range(-1,1)) = 0

	_VertexOffsetX		("Vertex OffsetX", float) = 0
	_VertexOffsetY		("Vertex OffsetY", float) = 0

	_CullMode			("Cull Mode", Float) = 0
	//_MaskCoord		("Mask Coords", vector) = (0,0,0,0)
	//_MaskSoftness		("Mask Softness", float) = 0
}

SubShader {

	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

	LOD 300
	Cull [_CullMode]

	CGPROGRAM
	#pragma surface PixShader BlinnPhong alpha:blend vertex:VertShader nolightmap nodirlightmap
	#pragma target 3.0
	#pragma shader_feature __ GLOW_ON
	#pragma glsl

	#include "TMPro_Properties.cginc"
	#include "TMPro.cginc"

	half _FaceShininess;
	half _OutlineShininess;

	struct Input
	{
		fixed4	color			: COLOR;
		float2	uv_MainTex;
		float2	uv2_FaceTex;
		float2  uv2_OutlineTex;
		float2	param;						// Weight, Scale
		float3	viewDirEnv;
	};


	#define BEVEL_ON 1
	#include "TMPro_Surface.cginc"

	ENDCG

	// Pass to render object as a shadow caster
	Pass
	{
		Name "Caster"
		Tags { "LightMode" = "ShadowCaster" }
		Offset 1, 1

		Fog {Mode Off}
		ZWrite On
		ZTest LEqual
		Cull Off

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_shadowcaster
		#include "UnityCG.cginc"

		struct v2f {
			V2F_SHADOW_CASTER;
			float2	uv			: TEXCOORD1;
			float2	uv2			: TEXCOORD3;
			float	alphaClip	: TEXCOORD2;
		};

		uniform float4 _MainTex_ST;
		uniform float4 _OutlineTex_ST;
		float _OutlineWidth;
		float _FaceDilate;
		float _ScaleRatioA;

		v2f vert( appdata_base v )
		{
			v2f o;
			TRANSFER_SHADOW_CASTER(o)
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.uv2 = TRANSFORM_TEX(v.texcoord, _OutlineTex);
			o.alphaClip = (1.0 - _OutlineWidth * _ScaleRatioA - _FaceDilate * _ScaleRatioA) / 2;
			return o;
		}

		uniform sampler2D _MainTex;

		float4 frag(v2f i) : COLOR
		{
			fixed4 texcol = tex2D(_MainTex, i.uv).a;
			clip(texcol.a - i.alphaClip);
			SHADOW_CASTER_FRAGMENT(i)
		}
		ENDCG
	}
}

CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
```

## File: Assets/TextMesh Pro/Shaders/TMP_SDF.shader
```
Shader "TextMeshPro/Distance Field" {

Properties {
	_FaceTex			("Face Texture", 2D) = "white" {}
	_FaceUVSpeedX		("Face UV Speed X", Range(-5, 5)) = 0.0
	_FaceUVSpeedY		("Face UV Speed Y", Range(-5, 5)) = 0.0
	[HDR]_FaceColor		("Face Color", Color) = (1,1,1,1)
	_FaceDilate			("Face Dilate", Range(-1,1)) = 0

	[HDR]_OutlineColor	("Outline Color", Color) = (0,0,0,1)
	_OutlineTex			("Outline Texture", 2D) = "white" {}
	_OutlineUVSpeedX	("Outline UV Speed X", Range(-5, 5)) = 0.0
	_OutlineUVSpeedY	("Outline UV Speed Y", Range(-5, 5)) = 0.0
	_OutlineWidth		("Outline Thickness", Range(0, 1)) = 0
	_OutlineSoftness	("Outline Softness", Range(0,1)) = 0

	_Bevel				("Bevel", Range(0,1)) = 0.5
	_BevelOffset		("Bevel Offset", Range(-0.5,0.5)) = 0
	_BevelWidth			("Bevel Width", Range(-.5,0.5)) = 0
	_BevelClamp			("Bevel Clamp", Range(0,1)) = 0
	_BevelRoundness		("Bevel Roundness", Range(0,1)) = 0

	_LightAngle			("Light Angle", Range(0.0, 6.2831853)) = 3.1416
	[HDR]_SpecularColor	("Specular", Color) = (1,1,1,1)
	_SpecularPower		("Specular", Range(0,4)) = 2.0
	_Reflectivity		("Reflectivity", Range(5.0,15.0)) = 10
	_Diffuse			("Diffuse", Range(0,1)) = 0.5
	_Ambient			("Ambient", Range(1,0)) = 0.5

	_BumpMap 			("Normal map", 2D) = "bump" {}
	_BumpOutline		("Bump Outline", Range(0,1)) = 0
	_BumpFace			("Bump Face", Range(0,1)) = 0

	_ReflectFaceColor	("Reflection Color", Color) = (0,0,0,1)
	_ReflectOutlineColor("Reflection Color", Color) = (0,0,0,1)
	_Cube 				("Reflection Cubemap", Cube) = "black" { /* TexGen CubeReflect */ }
	_EnvMatrixRotation	("Texture Rotation", vector) = (0, 0, 0, 0)


	[HDR]_UnderlayColor	("Border Color", Color) = (0,0,0, 0.5)
	_UnderlayOffsetX	("Border OffsetX", Range(-1,1)) = 0
	_UnderlayOffsetY	("Border OffsetY", Range(-1,1)) = 0
	_UnderlayDilate		("Border Dilate", Range(-1,1)) = 0
	_UnderlaySoftness	("Border Softness", Range(0,1)) = 0

	[HDR]_GlowColor			("Color", Color) = (0, 1, 0, 0.5)
	_GlowOffset			("Offset", Range(-1,1)) = 0
	_GlowInner			("Inner", Range(0,1)) = 0.05
	_GlowOuter			("Outer", Range(0,1)) = 0.05
	_GlowPower			("Falloff", Range(1, 0)) = 0.75

	_WeightNormal		("Weight Normal", float) = 0
	_WeightBold			("Weight Bold", float) = 0.5

	_ShaderFlags		("Flags", float) = 0
	_ScaleRatioA		("Scale RatioA", float) = 1
	_ScaleRatioB		("Scale RatioB", float) = 1
	_ScaleRatioC		("Scale RatioC", float) = 1

	_MainTex			("Font Atlas", 2D) = "white" {}
	_TextureWidth		("Texture Width", float) = 512
	_TextureHeight		("Texture Height", float) = 512
	_GradientScale		("Gradient Scale", float) = 5.0
	_ScaleX				("Scale X", float) = 1.0
	_ScaleY				("Scale Y", float) = 1.0
	_PerspectiveFilter	("Perspective Correction", Range(0, 1)) = 0.875
	_Sharpness			("Sharpness", Range(-1,1)) = 0

	_VertexOffsetX		("Vertex OffsetX", float) = 0
	_VertexOffsetY		("Vertex OffsetY", float) = 0

	_MaskCoord			("Mask Coordinates", vector) = (0, 0, 32767, 32767)
	_ClipRect			("Clip Rect", vector) = (-32767, -32767, 32767, 32767)
	_MaskSoftnessX		("Mask SoftnessX", float) = 0
	_MaskSoftnessY		("Mask SoftnessY", float) = 0

	_StencilComp		("Stencil Comparison", Float) = 8
	_Stencil			("Stencil ID", Float) = 0
	_StencilOp			("Stencil Operation", Float) = 0
	_StencilWriteMask	("Stencil Write Mask", Float) = 255
	_StencilReadMask	("Stencil Read Mask", Float) = 255

	_CullMode			("Cull Mode", Float) = 0
	_ColorMask			("Color Mask", Float) = 15
}

SubShader {

	Tags
	{
		"Queue"="Transparent"
		"IgnoreProjector"="True"
		"RenderType"="Transparent"
	}

	Stencil
	{
		Ref [_Stencil]
		Comp [_StencilComp]
		Pass [_StencilOp]
		ReadMask [_StencilReadMask]
		WriteMask [_StencilWriteMask]
	}

	Cull [_CullMode]
	ZWrite Off
	Lighting Off
	Fog { Mode Off }
	ZTest [unity_GUIZTestMode]
	Blend One OneMinusSrcAlpha
	ColorMask [_ColorMask]

	Pass {
		CGPROGRAM
		#pragma target 3.0
		#pragma vertex VertShader
		#pragma fragment PixShader
		#pragma shader_feature __ BEVEL_ON
		#pragma shader_feature __ UNDERLAY_ON UNDERLAY_INNER
		#pragma shader_feature __ GLOW_ON

		#pragma multi_compile __ UNITY_UI_CLIP_RECT
		#pragma multi_compile __ UNITY_UI_ALPHACLIP

		#include "UnityCG.cginc"
		#include "UnityUI.cginc"
		#include "TMPro_Properties.cginc"
		#include "TMPro.cginc"

		struct vertex_t {
			UNITY_VERTEX_INPUT_INSTANCE_ID
			float4	position		: POSITION;
			float3	normal			: NORMAL;
			fixed4	color			: COLOR;
			float2	texcoord0		: TEXCOORD0;
			float2	texcoord1		: TEXCOORD1;
		};


		struct pixel_t {
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
			float4	position		: SV_POSITION;
			fixed4	color			: COLOR;
			float2	atlas			: TEXCOORD0;		// Atlas
			float4	param			: TEXCOORD1;		// alphaClip, scale, bias, weight
			float4	mask			: TEXCOORD2;		// Position in object space(xy), pixel Size(zw)
			float3	viewDir			: TEXCOORD3;

		#if (UNDERLAY_ON || UNDERLAY_INNER)
			float4	texcoord2		: TEXCOORD4;		// u,v, scale, bias
			fixed4	underlayColor	: COLOR1;
		#endif
			float4 textures			: TEXCOORD5;
		};

		// Used by Unity internally to handle Texture Tiling and Offset.
		float4 _FaceTex_ST;
		float4 _OutlineTex_ST;

		pixel_t VertShader(vertex_t input)
		{
			pixel_t output;

			UNITY_INITIALIZE_OUTPUT(pixel_t, output);
			UNITY_SETUP_INSTANCE_ID(input);
			UNITY_TRANSFER_INSTANCE_ID(input,output);
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

			float bold = step(input.texcoord1.y, 0);

			float4 vert = input.position;
			vert.x += _VertexOffsetX;
			vert.y += _VertexOffsetY;

			float4 vPosition = UnityObjectToClipPos(vert);

			float2 pixelSize = vPosition.w;
			pixelSize /= float2(_ScaleX, _ScaleY) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
			float scale = rsqrt(dot(pixelSize, pixelSize));
			scale *= abs(input.texcoord1.y) * _GradientScale * (_Sharpness + 1);
			if (UNITY_MATRIX_P[3][3] == 0) scale = lerp(abs(scale) * (1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(input.normal.xyz), normalize(WorldSpaceViewDir(vert)))));

			float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
			weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;

			float bias =(.5 - weight) + (.5 / scale);

			float alphaClip = (1.0 - _OutlineWidth * _ScaleRatioA - _OutlineSoftness * _ScaleRatioA);

		#if GLOW_ON
			alphaClip = min(alphaClip, 1.0 - _GlowOffset * _ScaleRatioB - _GlowOuter * _ScaleRatioB);
		#endif

			alphaClip = alphaClip / 2.0 - ( .5 / scale) - weight;

		#if (UNDERLAY_ON || UNDERLAY_INNER)
			float4 underlayColor = _UnderlayColor;
			underlayColor.rgb *= underlayColor.a;

			float bScale = scale;
			bScale /= 1 + ((_UnderlaySoftness*_ScaleRatioC) * bScale);
			float bBias = (0.5 - weight) * bScale - 0.5 - ((_UnderlayDilate * _ScaleRatioC) * 0.5 * bScale);

			float x = -(_UnderlayOffsetX * _ScaleRatioC) * _GradientScale / _TextureWidth;
			float y = -(_UnderlayOffsetY * _ScaleRatioC) * _GradientScale / _TextureHeight;
			float2 bOffset = float2(x, y);
		#endif

			// Generate UV for the Masking Texture
			float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
			float2 maskUV = (vert.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

			// Support for texture tiling and offset
			float2 textureUV = UnpackUV(input.texcoord1.x);
			float2 faceUV = TRANSFORM_TEX(textureUV, _FaceTex);
			float2 outlineUV = TRANSFORM_TEX(textureUV, _OutlineTex);


			output.position = vPosition;
			output.color = input.color;
			output.atlas =	input.texcoord0;
			output.param =	float4(alphaClip, scale, bias, weight);
			output.mask = half4(vert.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + pixelSize.xy));
			output.viewDir =	mul((float3x3)_EnvMatrix, _WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, vert).xyz);
			#if (UNDERLAY_ON || UNDERLAY_INNER)
			output.texcoord2 = float4(input.texcoord0 + bOffset, bScale, bBias);
			output.underlayColor =	underlayColor;
			#endif
			output.textures = float4(faceUV, outlineUV);

			return output;
		}


		fixed4 PixShader(pixel_t input) : SV_Target
		{
			UNITY_SETUP_INSTANCE_ID(input);

			float c = tex2D(_MainTex, input.atlas).a;

		#ifndef UNDERLAY_ON
			clip(c - input.param.x);
		#endif

			float	scale	= input.param.y;
			float	bias	= input.param.z;
			float	weight	= input.param.w;
			float	sd = (bias - c) * scale;

			float outline = (_OutlineWidth * _ScaleRatioA) * scale;
			float softness = (_OutlineSoftness * _ScaleRatioA) * scale;

			half4 faceColor = _FaceColor;
			half4 outlineColor = _OutlineColor;

			faceColor.rgb *= input.color.rgb;

			faceColor *= tex2D(_FaceTex, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y);
			outlineColor *= tex2D(_OutlineTex, input.textures.zw + float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.y);

			faceColor = GetColor(sd, faceColor, outlineColor, outline, softness);

		#if BEVEL_ON
			float3 dxy = float3(0.5 / _TextureWidth, 0.5 / _TextureHeight, 0);
			float3 n = GetSurfaceNormal(input.atlas, weight, dxy);

			float3 bump = UnpackNormal(tex2D(_BumpMap, input.textures.xy + float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.y)).xyz;
			bump *= lerp(_BumpFace, _BumpOutline, saturate(sd + outline * 0.5));
			n = normalize(n- bump);

			float3 light = normalize(float3(sin(_LightAngle), cos(_LightAngle), -1.0));

			float3 col = GetSpecular(n, light);
			faceColor.rgb += col*faceColor.a;
			faceColor.rgb *= 1-(dot(n, light)*_Diffuse);
			faceColor.rgb *= lerp(_Ambient, 1, n.z*n.z);

			fixed4 reflcol = texCUBE(_Cube, reflect(input.viewDir, -n));
			faceColor.rgb += reflcol.rgb * lerp(_ReflectFaceColor.rgb, _ReflectOutlineColor.rgb, saturate(sd + outline * 0.5)) * faceColor.a;
		#endif

		#if UNDERLAY_ON
			float d = tex2D(_MainTex, input.texcoord2.xy).a * input.texcoord2.z;
			faceColor += input.underlayColor * saturate(d - input.texcoord2.w) * (1 - faceColor.a);
		#endif

		#if UNDERLAY_INNER
			float d = tex2D(_MainTex, input.texcoord2.xy).a * input.texcoord2.z;
			faceColor += input.underlayColor * (1 - saturate(d - input.texcoord2.w)) * saturate(1 - sd) * (1 - faceColor.a);
		#endif

		#if GLOW_ON
			float4 glowColor = GetGlowColor(sd, scale);
			faceColor.rgb += glowColor.rgb * glowColor.a;
		#endif

		// Alternative implementation to UnityGet2DClipping with support for softness.
		#if UNITY_UI_CLIP_RECT
			half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * input.mask.zw);
			faceColor *= m.x * m.y;
		#endif

		#if UNITY_UI_ALPHACLIP
			clip(faceColor.a - 0.001);
		#endif

  		return faceColor * input.color.a;
		}

		ENDCG
	}
}

Fallback "TextMeshPro/Mobile/Distance Field"
CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
```

## File: Assets/TextMesh Pro/Shaders/TMP_Sprite.shader
```
Shader "TextMeshPro/Sprite"
{
	Properties
	{
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		
		_CullMode ("Cull Mode", Float) = 0
		_ColorMask ("Color Mask", Float) = 15
		_ClipRect ("Clip Rect", vector) = (-32767, -32767, 32767, 32767)

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull [_CullMode]
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
            Name "Default"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            #pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
			};
			
            sampler2D _MainTex;
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
            float4 _MainTex_ST;

            v2f vert(appdata_t v)
			{
				v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				
                OUT.color = v.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
				
                #ifdef UNITY_UI_CLIP_RECT
					color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				#endif

				#ifdef UNITY_UI_ALPHACLIP
					clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
}
```

## File: Assets/TextMesh Pro/Shaders/TMPro_Mobile.cginc
```
struct vertex_t {
    UNITY_VERTEX_INPUT_INSTANCE_ID
    float4	position		: POSITION;
    float3	normal			: NORMAL;
    float4	color			: COLOR;
    float2	texcoord0		: TEXCOORD0;
    float2	texcoord1		: TEXCOORD1;
};

struct pixel_t {
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
    float4	position		: SV_POSITION;
    float4	faceColor		: COLOR;
    float4	outlineColor	: COLOR1;
    float4	texcoord0		: TEXCOORD0;
    float4	param			: TEXCOORD1;		// weight, scaleRatio
    float2	mask			: TEXCOORD2;
    #if (UNDERLAY_ON || UNDERLAY_INNER)
    float4	texcoord2		: TEXCOORD3;
    float4	underlayColor	: COLOR2;
    #endif
};

float4 SRGBToLinear(float4 rgba) {
    return float4(lerp(rgba.rgb / 12.92f, pow((rgba.rgb + 0.055f) / 1.055f, 2.4f), step(0.04045f, rgba.rgb)), rgba.a);
}

pixel_t VertShader(vertex_t input)
{
    pixel_t output;

    UNITY_INITIALIZE_OUTPUT(pixel_t, output);
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    float bold = step(input.texcoord1.y, 0);

    float4 vert = input.position;
    vert.x += _VertexOffsetX;
    vert.y += _VertexOffsetY;

    float4 vPosition = UnityObjectToClipPos(vert);

    float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
    weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;

    // Generate UV for the Masking Texture
    float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
    float2 maskUV = (vert.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

    float4 color = input.color;
    #if (FORCE_LINEAR && !UNITY_COLORSPACE_GAMMA)
    color = SRGBToLinear(input.color);
    #endif

    float opacity = color.a;
    #if (UNDERLAY_ON | UNDERLAY_INNER)
    opacity = 1.0;
    #endif

    float4 faceColor = float4(color.rgb, opacity) * _FaceColor;
    faceColor.rgb *= faceColor.a;

    float4 outlineColor = _OutlineColor;
    outlineColor.a *= opacity;
    outlineColor.rgb *= outlineColor.a;

    output.position = vPosition;
    output.faceColor = faceColor;
    output.outlineColor = outlineColor;
    output.texcoord0 = float4(input.texcoord0.xy, maskUV.xy);
    output.param = float4(0.5 - weight, 1.3333 * _GradientScale * (_Sharpness + 1) / _TextureWidth, _OutlineWidth * _ScaleRatioA * 0.5, 0);

    float2 mask = float2(0, 0);
    #if UNITY_UI_CLIP_RECT
    mask = vert.xy * 2 - clampedRect.xy - clampedRect.zw;
    #endif
    output.mask = mask;

    #if (UNDERLAY_ON || UNDERLAY_INNER)
    float4 underlayColor = _UnderlayColor;
    underlayColor.rgb *= underlayColor.a;

    float x = -(_UnderlayOffsetX * _ScaleRatioC) * _GradientScale / _TextureWidth;
    float y = -(_UnderlayOffsetY * _ScaleRatioC) * _GradientScale / _TextureHeight;

    output.texcoord2 = float4(input.texcoord0 + float2(x, y), input.color.a, 0);
    output.underlayColor = underlayColor;
    #endif

    return output;
}

float4 PixShader(pixel_t input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);

    float d = tex2D(_MainTex, input.texcoord0.xy).a;

    float2 UV = input.texcoord0.xy;
    float scale = rsqrt(abs(ddx(UV.x) * ddy(UV.y) - ddy(UV.x) * ddx(UV.y))) * input.param.y;

    #if (UNDERLAY_ON | UNDERLAY_INNER)
    float layerScale = scale;
    layerScale /= 1 + ((_UnderlaySoftness * _ScaleRatioC) * layerScale);
    float layerBias = input.param.x * layerScale - .5 - ((_UnderlayDilate * _ScaleRatioC) * .5 * layerScale);
    #endif

    scale /= 1 + (_OutlineSoftness * _ScaleRatioA * scale);

    float4 faceColor = input.faceColor * saturate((d - input.param.x) * scale + 0.5);

    #ifdef OUTLINE_ON
    float4 outlineColor = lerp(input.faceColor, input.outlineColor, sqrt(min(1.0, input.param.z * scale * 2)));
    faceColor = lerp(outlineColor, input.faceColor, saturate((d - input.param.x - input.param.z) * scale + 0.5));
    faceColor *= saturate((d - input.param.x + input.param.z) * scale + 0.5);
    #endif

    #if UNDERLAY_ON
    d = tex2D(_MainTex, input.texcoord2.xy).a * layerScale;
    faceColor += float4(_UnderlayColor.rgb * _UnderlayColor.a, _UnderlayColor.a) * saturate(d - layerBias) * (1 - faceColor.a);
    #endif

    #if UNDERLAY_INNER
    float bias = input.param.x * scale - 0.5;
    float sd = saturate(d * scale - bias - input.param.z);
    d = tex2D(_MainTex, input.texcoord2.xy).a * layerScale;
    faceColor += float4(_UnderlayColor.rgb * _UnderlayColor.a, _UnderlayColor.a) * (1 - saturate(d - layerBias)) * sd * (1 - faceColor.a);
    #endif

    #ifdef MASKING
    float a = abs(_MaskInverse - tex2D(_MaskTex, input.texcoord0.zw).a);
    float t = a + (1 - _MaskWipeControl) * _MaskEdgeSoftness - _MaskWipeControl;
    a = saturate(t / _MaskEdgeSoftness);
    faceColor.rgb = lerp(_MaskEdgeColor.rgb * faceColor.a, faceColor.rgb, a);
    faceColor *= a;
    #endif

    // Alternative implementation to UnityGet2DClipping with support for softness
    #if UNITY_UI_CLIP_RECT
    float2 maskZW = 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + (1 / scale));
    float2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * maskZW);
    faceColor *= m.x * m.y;
    #endif

    #if (UNDERLAY_ON | UNDERLAY_INNER)
    faceColor *= input.texcoord2.z;
    #endif

    #if UNITY_UI_ALPHACLIP
    clip(faceColor.a - 0.001);
    #endif

    return faceColor;
}
```

## File: Assets/TextMesh Pro/Shaders/TMPro_Properties.cginc
```
// UI Editable properties
uniform sampler2D	_FaceTex;					// Alpha : Signed Distance
uniform float		_FaceUVSpeedX;
uniform float		_FaceUVSpeedY;
uniform fixed4		_FaceColor;					// RGBA : Color + Opacity
uniform float		_FaceDilate;				// v[ 0, 1]
uniform float		_OutlineSoftness;			// v[ 0, 1]

uniform sampler2D	_OutlineTex;				// RGBA : Color + Opacity
uniform float		_OutlineUVSpeedX;
uniform float		_OutlineUVSpeedY;
uniform fixed4		_OutlineColor;				// RGBA : Color + Opacity
uniform float		_OutlineWidth;				// v[ 0, 1]

uniform float		_Bevel;						// v[ 0, 1]
uniform float		_BevelOffset;				// v[-1, 1]
uniform float		_BevelWidth;				// v[-1, 1]
uniform float		_BevelClamp;				// v[ 0, 1]
uniform float		_BevelRoundness;			// v[ 0, 1]

uniform sampler2D	_BumpMap;					// Normal map
uniform float		_BumpOutline;				// v[ 0, 1]
uniform float		_BumpFace;					// v[ 0, 1]

uniform samplerCUBE	_Cube;						// Cube / sphere map
uniform fixed4 		_ReflectFaceColor;			// RGB intensity
uniform fixed4		_ReflectOutlineColor;
//uniform float		_EnvTiltX;					// v[-1, 1]
//uniform float		_EnvTiltY;					// v[-1, 1]
uniform float3      _EnvMatrixRotation;
uniform float4x4	_EnvMatrix;

uniform fixed4		_SpecularColor;				// RGB intensity
uniform float		_LightAngle;				// v[ 0,Tau]
uniform float		_SpecularPower;				// v[ 0, 1]
uniform float		_Reflectivity;				// v[ 5, 15]
uniform float		_Diffuse;					// v[ 0, 1]
uniform float		_Ambient;					// v[ 0, 1]

uniform fixed4		_UnderlayColor;				// RGBA : Color + Opacity
uniform float		_UnderlayOffsetX;			// v[-1, 1]
uniform float		_UnderlayOffsetY;			// v[-1, 1]
uniform float		_UnderlayDilate;			// v[-1, 1]
uniform float		_UnderlaySoftness;			// v[ 0, 1]

uniform fixed4 		_GlowColor;					// RGBA : Color + Intesity
uniform float 		_GlowOffset;				// v[-1, 1]
uniform float 		_GlowOuter;					// v[ 0, 1]
uniform float 		_GlowInner;					// v[ 0, 1]
uniform float 		_GlowPower;					// v[ 1, 1/(1+4*4)]

// API Editable properties
uniform float 		_ShaderFlags;
uniform float		_WeightNormal;
uniform float		_WeightBold;

uniform float		_ScaleRatioA;
uniform float		_ScaleRatioB;
uniform float		_ScaleRatioC;

uniform float		_VertexOffsetX;
uniform float		_VertexOffsetY;

//uniform float		_UseClipRect;
uniform float		_MaskID;
uniform sampler2D	_MaskTex;
uniform float4		_MaskCoord;
uniform float4		_ClipRect;	// bottom left(x,y) : top right(z,w)
//uniform float		_MaskWipeControl;
//uniform float		_MaskEdgeSoftness;
//uniform fixed4		_MaskEdgeColor;
//uniform bool		_MaskInverse;

uniform float		_MaskSoftnessX;
uniform float		_MaskSoftnessY;

// Font Atlas properties
uniform sampler2D	_MainTex;
uniform float		_TextureWidth;
uniform float		_TextureHeight;
uniform float 		_GradientScale;
uniform float		_ScaleX;
uniform float		_ScaleY;
uniform float		_PerspectiveFilter;
uniform float		_Sharpness;
```

## File: Assets/TextMesh Pro/Shaders/TMPro_Surface.cginc
```
void VertShader(inout appdata_full v, out Input data)
{
	v.vertex.x += _VertexOffsetX;
	v.vertex.y += _VertexOffsetY;

	UNITY_INITIALIZE_OUTPUT(Input, data);

	float bold = step(v.texcoord1.y, 0);

	// Generate normal for backface
	float3 view = ObjSpaceViewDir(v.vertex);
	v.normal *= sign(dot(v.normal, view));

#if USE_DERIVATIVE
	data.param.y = 1;
#else
	float4 vert = v.vertex;
	float4 vPosition = UnityObjectToClipPos(vert);
	float2 pixelSize = vPosition.w;

	pixelSize /= float2(_ScaleX, _ScaleY) * mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy);
	float scale = rsqrt(dot(pixelSize, pixelSize));
	scale *= abs(v.texcoord1.y) * _GradientScale * (_Sharpness + 1);
	scale = lerp(scale * (1 - _PerspectiveFilter), scale, abs(dot(UnityObjectToWorldNormal(v.normal.xyz), normalize(WorldSpaceViewDir(vert)))));
	data.param.y = scale;
#endif

	data.param.x = (lerp(_WeightNormal, _WeightBold, bold) / 4.0 + _FaceDilate) * _ScaleRatioA * 0.5; // 

	v.texcoord1.xy = UnpackUV(v.texcoord1.x);
	data.viewDirEnv = mul((float3x3)_EnvMatrix, WorldSpaceViewDir(v.vertex));
}

void PixShader(Input input, inout SurfaceOutput o)
{

#if USE_DERIVATIVE
	float2 pixelSize = float2(ddx(input.uv_MainTex.y), ddy(input.uv_MainTex.y));
	pixelSize *= _TextureWidth * .75;
	float scale = rsqrt(dot(pixelSize, pixelSize)) * _GradientScale * (_Sharpness + 1);
#else
	float scale = input.param.y;
#endif

	// Signed distance
	float c = tex2D(_MainTex, input.uv_MainTex).a;
	float sd = (.5 - c - input.param.x) * scale + .5;
	float outline = _OutlineWidth*_ScaleRatioA * scale;
	float softness = _OutlineSoftness*_ScaleRatioA * scale;

	// Color & Alpha
	float4 faceColor = _FaceColor;
	float4 outlineColor = _OutlineColor;
	faceColor *= input.color;
	outlineColor.a *= input.color.a;
	faceColor *= tex2D(_FaceTex, float2(input.uv2_FaceTex.x + _FaceUVSpeedX * _Time.y, input.uv2_FaceTex.y + _FaceUVSpeedY * _Time.y));
	outlineColor *= tex2D(_OutlineTex, float2(input.uv2_OutlineTex.x + _OutlineUVSpeedX * _Time.y, input.uv2_OutlineTex.y + _OutlineUVSpeedY * _Time.y));
	faceColor = GetColor(sd, faceColor, outlineColor, outline, softness);
	faceColor.rgb /= max(faceColor.a, 0.0001);

#if BEVEL_ON
	float3 delta = float3(1.0 / _TextureWidth, 1.0 / _TextureHeight, 0.0);

	float4 smp4x = {tex2D(_MainTex, input.uv_MainTex - delta.xz).a,
					tex2D(_MainTex, input.uv_MainTex + delta.xz).a,
					tex2D(_MainTex, input.uv_MainTex - delta.zy).a,
					tex2D(_MainTex, input.uv_MainTex + delta.zy).a };

	// Face Normal
	float3 n = GetSurfaceNormal(smp4x, input.param.x);

	// Bumpmap
	float3 bump = UnpackNormal(tex2D(_BumpMap, input.uv2_FaceTex.xy)).xyz;
	bump *= lerp(_BumpFace, _BumpOutline, saturate(sd + outline * 0.5));
	bump = lerp(float3(0, 0, 1), bump, faceColor.a);
	n = normalize(n - bump);

	// Cubemap reflection
	fixed4 reflcol = texCUBE(_Cube, reflect(input.viewDirEnv, mul((float3x3)unity_ObjectToWorld, n)));
	float3 emission = reflcol.rgb * lerp(_ReflectFaceColor.rgb, _ReflectOutlineColor.rgb, saturate(sd + outline * 0.5)) * faceColor.a;
#else
	float3 n = float3(0, 0, -1);
	float3 emission = float3(0, 0, 0);
#endif
	
#if GLOW_ON
	float4 glowColor = GetGlowColor(sd, scale);
	glowColor.a *= input.color.a;
	emission += glowColor.rgb*glowColor.a;
	faceColor = BlendARGB(glowColor, faceColor);
	faceColor.rgb /= max(faceColor.a, 0.0001);
#endif

	// Set Standard output structure
	o.Albedo = faceColor.rgb;
	o.Normal = -n;
	o.Emission = emission;
	o.Specular = lerp(_FaceShininess, _OutlineShininess, saturate(sd + outline * 0.5));
	o.Gloss = 1;
	o.Alpha = faceColor.a;
}
```

## File: Assets/TextMesh Pro/Shaders/TMPro.cginc
```
float2 UnpackUV(float uv)
{ 
	float2 output;
	output.x = floor(uv / 4096);
	output.y = uv - 4096 * output.x;

	return output * 0.001953125;
}

fixed4 GetColor(half d, fixed4 faceColor, fixed4 outlineColor, half outline, half softness)
{
	half faceAlpha = 1-saturate((d - outline * 0.5 + softness * 0.5) / (1.0 + softness));
	half outlineAlpha = saturate((d + outline * 0.5)) * sqrt(min(1.0, outline));

	faceColor.rgb *= faceColor.a;
	outlineColor.rgb *= outlineColor.a;

	faceColor = lerp(faceColor, outlineColor, outlineAlpha);

	faceColor *= faceAlpha;

	return faceColor;
}

float3 GetSurfaceNormal(float4 h, float bias)
{
	bool raisedBevel = step(1, fmod(_ShaderFlags, 2));

	h += bias+_BevelOffset;

	float bevelWidth = max(.01, _OutlineWidth+_BevelWidth);

  // Track outline
	h -= .5;
	h /= bevelWidth;
	h = saturate(h+.5);

	if(raisedBevel) h = 1 - abs(h*2.0 - 1.0);
	h = lerp(h, sin(h*3.141592/2.0), _BevelRoundness);
	h = min(h, 1.0-_BevelClamp);
	h *= _Bevel * bevelWidth * _GradientScale * -2.0;

	float3 va = normalize(float3(1.0, 0.0, h.y - h.x));
	float3 vb = normalize(float3(0.0, -1.0, h.w - h.z));

	return cross(va, vb);
}

float3 GetSurfaceNormal(float2 uv, float bias, float3 delta)
{
	// Read "height field"
  float4 h = {tex2D(_MainTex, uv - delta.xz).a,
				tex2D(_MainTex, uv + delta.xz).a,
				tex2D(_MainTex, uv - delta.zy).a,
				tex2D(_MainTex, uv + delta.zy).a};

	return GetSurfaceNormal(h, bias);
}

float3 GetSpecular(float3 n, float3 l)
{
	float spec = pow(max(0.0, dot(n, l)), _Reflectivity);
	return _SpecularColor.rgb * spec * _SpecularPower;
}

float4 GetGlowColor(float d, float scale)
{
	float glow = d - (_GlowOffset*_ScaleRatioB) * 0.5 * scale;
	float t = lerp(_GlowInner, (_GlowOuter * _ScaleRatioB), step(0.0, glow)) * 0.5 * scale;
	glow = saturate(abs(glow/(1.0 + t)));
	glow = 1.0-pow(glow, _GlowPower);
	glow *= sqrt(min(1.0, t)); // Fade off glow thinner than 1 screen pixel
	return float4(_GlowColor.rgb, saturate(_GlowColor.a * glow * 2));
}

float4 BlendARGB(float4 overlying, float4 underlying)
{
	overlying.rgb *= overlying.a;
	underlying.rgb *= underlying.a;
	float3 blended = overlying.rgb + ((1-overlying.a)*underlying.rgb);
	float alpha = underlying.a + (1-underlying.a)*overlying.a;
	return float4(blended, alpha);
}
```

## File: Assets/TextMesh Pro/Sprites/EmojiOne Attribution.txt
```
This sample of beautiful emojis are provided by EmojiOne https://www.emojione.com/

Please visit their website to view the complete set of their emojis and review their licensing terms.
```

## File: Assets/TextMesh Pro/Sprites/EmojiOne.json
```json
{"frames": [

{
	"filename": "1f60a.png",
	"frame": {"x":0,"y":0,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
},
{
	"filename": "1f60b.png",
	"frame": {"x":128,"y":0,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
},
{
	"filename": "1f60d.png",
	"frame": {"x":256,"y":0,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
},
{
	"filename": "1f60e.png",
	"frame": {"x":384,"y":0,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
},
{
	"filename": "1f600.png",
	"frame": {"x":0,"y":128,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
},
{
	"filename": "1f601.png",
	"frame": {"x":128,"y":128,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
},
{
	"filename": "1f602.png",
	"frame": {"x":256,"y":128,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
},
{
	"filename": "1f603.png",
	"frame": {"x":384,"y":128,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
},
{
	"filename": "1f604.png",
	"frame": {"x":0,"y":256,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
},
{
	"filename": "1f605.png",
	"frame": {"x":128,"y":256,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
},
{
	"filename": "1f606.png",
	"frame": {"x":256,"y":256,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
},
{
	"filename": "1f609.png",
	"frame": {"x":384,"y":256,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
},
{
	"filename": "1f618.png",
	"frame": {"x":0,"y":384,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
},
{
	"filename": "1f923.png",
	"frame": {"x":128,"y":384,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
},
{
	"filename": "263a.png",
	"frame": {"x":256,"y":384,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
},
{
	"filename": "2639.png",
	"frame": {"x":384,"y":384,"w":128,"h":128},
	"rotated": false,
	"trimmed": false,
	"spriteSourceSize": {"x":0,"y":0,"w":128,"h":128},
	"sourceSize": {"w":128,"h":128},
	"pivot": {"x":0.5,"y":0.5}
}],
"meta": {
	"app": "http://www.codeandweb.com/texturepacker",
	"version": "1.0",
	"image": "EmojiOne.png",
	"format": "RGBA8888",
	"size": {"w":512,"h":512},
	"scale": "1",
	"smartupdate": "$TexturePacker:SmartUpdate:196a26a2e149d875b91ffc8fa3581e76:fc928c7e275404b7e0649307410475cb:424723c3774975ddb2053fd5c4b85f6e$"
}
}
```

## File: .vsconfig
```
{
  "version": "1.0",
  "components": [
    "Microsoft.VisualStudio.Workload.ManagedGame"
  ]
}
```

## File: Assets/Scripts/GestorServicioCliente.cs
```csharp
using UnityEngine;
using TMPro; // Librería necesaria para modificar textos TextMeshPro en Unity 2022.

// [System.Serializable] permite que esta "plantilla" se pueda ver y rellenar en el Inspector de Unity.
// Representa la estructura de un nivel individual de atención al cliente.
[System.Serializable]
public class CasoCliente
{
    // AGREGAR QUE DESAPAREZCA EL PANEL DE NIVEL AL COMPLETAR EL MODULO
    [TextArea(2, 4)] // Hace que la caja de texto en el Inspector sea más grande y cómoda de escribir.
    public string mensajeCliente; // La queja o pregunta que hace el cliente.

    public string textoBoton1;    // Texto para la primera opción de respuesta.
    public string textoBoton2;    // Texto para la segunda opción de respuesta.
    public string textoBoton3;    // Texto para la tercera opción de respuesta.

    [Tooltip("Escribe 1, 2 o 3 según cuál sea el botón con la respuesta correcta")]
    public int respuestaCorrecta; // Guardará el número (1, 2 o 3) del botón correcto en este nivel.
}

public class GestorServicioCliente : MonoBehaviour
{
    // VARIABLES DE INTERFAZ (TEXTOS Y PANELES)
    [Header("Elementos de Texto en Pantalla")]
    public TextMeshProUGUI textoPantallaCliente; // El texto donde se lee el mensaje del cliente.
    public TextMeshProUGUI textoEnBoton1;        // El texto dentro del Botón 1.
    public TextMeshProUGUI textoEnBoton2;        // El texto dentro del Botón 2.
    public TextMeshProUGUI textoEnBoton3;        // El texto dentro del Botón 3.

    [Header("Conexión con otros sistemas")]
    public GestorPenalizacion scriptPenalizacion; // Referencia a tu script anterior para poder restar estrellas.

    // DATOS DE LOS 5 NIVELES
    [Header("Configuración de los 5 Niveles")]
    public CasoCliente[] niveles; // Lista (Array) donde escribiremos los 5 casos desde el Inspector.
    private int nivelActual = 0;  // Lleva la cuenta de en qué nivel vamos (en programación se empieza en 0).

    void Start()
    {
        // Cargamos el primer nivel (posición 0 de la lista) al iniciar.
        CargarNivel(0);
    }

    // MÉTODOS DE LÓGICA DEL JUEGO
    // Esta función actualiza los textos de la pantalla según el nivel en el que estemos.
    // Es 'public' por si deseas llamarla también desde un menú selector de niveles (1 al 5).
    public void CargarNivel(int indiceNivel)
    {
        // Guardamos el número de nivel actual
        nivelActual = indiceNivel;

        // Copiamos los textos de nuestra lista y los ponemos en los textos reales de la pantalla
        textoPantallaCliente.text = niveles[nivelActual].mensajeCliente;
        textoEnBoton1.text = niveles[nivelActual].textoBoton1;
        textoEnBoton2.text = niveles[nivelActual].textoBoton2;
        textoEnBoton3.text = niveles[nivelActual].textoBoton3;
    }

    // Esta función se conectará a los 3 botones de respuesta.
    // Recibe un número (1, 2 o 3) dependiendo de cuál botón presionó el jugador.
    public void SeleccionarRespuesta(int botonPresionado)
    {
        // Comparamos si el botón que presionó el jugador es igual a la respuesta correcta de este nivel
        if (botonPresionado == niveles[nivelActual].respuestaCorrecta)
        {
            Debug.Log("¡Respuesta profesional! Pasando al siguiente caso...");
            AvanzarSiguienteNivel();
        }
        else
        {
            Debug.Log("Respuesta poco profesional. Pierdes una estrella.");
            // Llamamos a la función RestarEstrella() de tu otro script
            scriptPenalizacion.RestarEstrella();
        }
    }

    // Método interno para pasar al siguiente caso o ganar el módulo
    private void AvanzarSiguienteNivel()
    {
        // Sumamos 1 para pasar al siguiente nivel
        nivelActual++;

        // Verificamos si todavía quedan niveles en la lista (por ejemplo, si aún no llegamos al 5)
        if (nivelActual < niveles.Length)
        {
            // Cargamos el nuevo nivel en pantalla
            CargarNivel(nivelActual);
        }
        else
        {
            // Si ya no hay más niveles en la lista, significa que superó los 5 casos con éxito
            Debug.Log("¡Módulo de Atención al Cliente completado!");

            // Llamamos al GestorPenalizacion para detener el reloj y mostrar estadísticas de estrellas
            scriptPenalizacion.VictoriaServicio();
        }
    }
}
```

## File: .gitignore
```
# This .gitignore file should be placed at the root of your Unity project directory
#
# Get latest from https://github.com/github/gitignore/blob/main/Unity.gitignore
#
# Recommended: add any editor/OS/tool-specific ignore rules from the Global/ templates as needed.
# See: https://github.com/github/gitignore/tree/main/Global
#
.utmp/
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
/[Uu]ser[Ss]ettings/
*.log

# By default unity supports Blender asset imports, *.blend1 blender files do not need to be commited to version control.
*.blend1
*.blend1.meta

# MemoryCaptures can get excessive in size.
# They also could contain extremely sensitive data
/[Mm]emoryCaptures/

# Recordings can get excessive in size
/[Rr]ecordings/

# Uncomment this line if you wish to ignore the asset store tools plugin
# /[Aa]ssets/AssetStoreTools*

# Autogenerated Jetbrains Rider plugin
/[Aa]ssets/Plugins/Editor/JetBrains*
# Jetbrains Rider personal-layer settings
*.DotSettings.user

# Visual Studio cache directory
.vs/

#Visual Studio Code
.vscode/

# Gradle cache directory
.gradle/

# Autogenerated VS/MD/Consulo solution and project files
ExportedObj/
.consulo/
*.csproj
*.unityproj
*.sln
*.slnx
*.suo
*.tmp
*.user
*.userprefs
*.pidb
*.booproj
*.svd
*.pdb
*.mdb
*.opendb
*.VC.db

# Unity3D generated meta files
*.pidb.meta
*.pdb.meta
*.mdb.meta

# Unity3D generated file on crash reports
sysinfo.txt

# Mono auto generated files
mono_crash.*

# Builds
*.apk
*.aab
*.unitypackage
*.unitypackage.meta
*.app

# Crashlytics generated file
crashlytics-build.properties

# TestRunner generated files
InitTestScene*.unity*

# Addressables default ignores, before user customizations
/ServerData
/[Aa]ssets/StreamingAssets/aa*
/[Aa]ssets/AddressableAssetsData/link.xml*
/[Aa]ssets/Addressables_Temp*
# By default, Addressables content builds will generate addressables_content_state.bin
# files in platform-specific subfolders, for example:
# /Assets/AddressableAssetsData/OSX/addressables_content_state.bin
/[Aa]ssets/AddressableAssetsData/*/*.bin*

# Visual Scripting auto-generated files
/[Aa]ssets/Unity.VisualScripting.Generated/VisualScripting.Flow/UnitOptions.db
/[Aa]ssets/Unity.VisualScripting.Generated/VisualScripting.Flow/UnitOptions.db.meta
/[Aa]ssets/Unity.VisualScripting.Generated/VisualScripting.Core/Property Providers
/[Aa]ssets/Unity.VisualScripting.Generated/VisualScripting.Core/Property Providers.meta

# Auto-generated scenes by play mode tests
/[Aa]ssets/[Ii]nit[Tt]est[Ss]cene*.unity*

# Auto-generated cache in Assets folder
/[Aa]ssets/[Ss]ceneDependencyCache*
```

## File: Assets/Scripts/GestorMenuPrincipal.cs
```csharp
using UnityEngine; // Esta línea es obligatoria, le dice al script que use las herramientas básicas de Unity.

public class GestorMenuPrincipal : MonoBehaviour
{
    // VARIABLES DE LOS PANELES PRINCIPALES
    // Usamos 'public' para poder ver estas variables en el Inspector de Unity.
    // 'GameObject' representa cualquier objeto en la escena (en este caso, nuestros paneles).

    [Header("Paneles de la Interfaz")]   // Esto solo pone un título en el Inspector de Unity para organizar.
    public GameObject panelMenuPrincipal;   // Aquí va el panel del menú de inicio.
    public GameObject panelSelectorModulos; // Aquí va el panel donde se elige Servicio al Cliente o Phishing.
    public GameObject panelInstrucciones;   // Aquí va el panel con las instrucciones.

    [Header("Paneles de los Módulos")]
    public GameObject panelNivelServicio;  // Aquí va panel del juego de Servicio al Cliente.
    public GameObject panelNivelPhishing;  // Aquí va panel del juego de Phishing.

    [Header("Conexión con los otros Scripts")]
    public GestorPenalizacion scriptPenalizacion;
    public GestorServicioCliente scriptServicio;
    public GestorPhishing scriptPhishing;

    // MÉTODO START
    // Todo lo que esté dentro de Start() se ejecuta una sola vez justo cuando arranca el juego.
    void Start()
    {
        // Al iniciar el juego, queremos asegurarnos de que el jugador solo vea el menú principal.
        // SetActive(true) enciende el objeto, SetActive(false) lo apaga.
        panelMenuPrincipal.SetActive(true);

        // Apagamos los otros dos paneles por si se quedaron encendidos por accidente en el editor.
        panelSelectorModulos.SetActive(false);
        panelInstrucciones.SetActive(false);

        panelNivelServicio.SetActive(false);
        panelNivelPhishing.SetActive(false);
    }

    // MÉTODOS DEL MENÚ PRINCIPAL
    // MÉTODOS PARA LOS BOTONES
    // Usamos 'public void' para que los botones de Unity puedan encontrar y ejecutar estas funciones.

    // 1. Esta función se conectará al botón "Jugar"
    public void BotonJugar()
    {
        // Apagamos el Menú Principal
        panelMenuPrincipal.SetActive(false);
        // Encendemos el panel para elegir el módulo
        panelSelectorModulos.SetActive(true);
    }

    // 2. Esta función se conectará al botón "Instrucciones"
    public void BotonInstrucciones()
    {
        // Apagamos el Menú Principal
        panelMenuPrincipal.SetActive(false);
        // Encendemos el panel de Instrucciones
        panelInstrucciones.SetActive(true);
    }

    // 3. Esta función se conectará a los botones "Volver" (tanto en instrucciones como en el selector)
    public void BotonVolver()
    {
        // No importa en qué pantalla estemos, apagamos ambas por seguridad
        panelInstrucciones.SetActive(false);
        panelSelectorModulos.SetActive(false);

        // y volvemos a encender el Menú Principal
        panelMenuPrincipal.SetActive(true);
    }

    // 4. Esta función se conectará al botón "Salir"
    public void BotonSalir()
    {
        // Mensaje de prueba: Como Application.Quit() no cierra el editor de Unity, 
        // usamos Debug.Log para ver en la consola que el botón sí está funcionando.
        Debug.Log("Saliendo del simulador de NexoBank");

        // Esta es la orden real que cierra el juego cuando ya está exportado como un programa final (.exe).
        Application.Quit();
    }

    // MÉTODOS: SELECTOR DE MÓDULOS
    // 1. Esta función se conectará al botón "Servicio al Cliente" dentro del panel Selector - Al presionar el botón "Servicio al Cliente" en el Selector
    public void BotonModuloServicio()
    {
        // Apagamos el selector para despejar la pantalla
        panelSelectorModulos.SetActive(false);

        // Encendemos el nivel de Atención al Cliente
        panelNivelServicio.SetActive(true);

        // Reiniciamos el módulo al nivel 0, restauramos las 3 estrellas y arrancamos el reloj
        scriptServicio.CargarNivel(0);
        scriptPenalizacion.PrepararModuloServicio();
    }

    // 2. Esta función se conectará al botón "Detector de Phishing" dentro del panel Selector -
    public void BotonModuloPhishing()
    {
        // Apagamos el selector
        panelSelectorModulos.SetActive(false);

        // Encendemos el nivel de Phishing
        panelNivelPhishing.SetActive(true);

        // Reiniciamos el módulo a la imagen 0, restauramos los 3 candados y arrancamos el reloj
        scriptPhishing.CargarImagenNivel(0);
        scriptPenalizacion.PrepararModuloPhishing();
    }

    // 3. Esta función sirve para un botón de "Atrás" o "Volver" dentro de los propios niveles - Se puede volver al selector desde el nivel, desde Game Over o desde Victoria
    public void BotonVolverASelector()
    {
        scriptPenalizacion.DetenerReloj(); // Apagamos el reloj por si salió a mitad de partida

        // Apagamos cualquier nivel en el que estemos
        panelNivelServicio.SetActive(false);
        panelNivelPhishing.SetActive(false);

        scriptPenalizacion.panelGameOver.SetActive(false);
        scriptPenalizacion.panelVictoria.SetActive(false);

        // Y volvemos a encender la pantalla de selección de módulos
        panelSelectorModulos.SetActive(true);
    }
}
```

## File: Assets/Scripts/GestorPenalizacion.cs
```csharp
using TMPro; // Librería necesaria para modificar los textos de la interfaz (reloj, mensajes finales).
using UnityEngine;

public class GestorPenalizacion : MonoBehaviour
{
    // 1. VARIABLES DE VIDAS (ESTRELLAS Y CANDADOS)
    // Usamos corchetes [] para indicar que esto es un "Array" (una lista de varios objetos, no solo uno).
    [Header("Vidas del Módulo de Servicio")]
    public GameObject[] estrellas; // Las 3 estrellas que se ven mientras juegas Servicio al Cliente.
    private int erroresServicio = 0; // Esta variable interna (privada) cuenta cuántas veces se ha equivocado el jugador en los niveles de Servicio al Cliente.

    [Header("Vidas del Módulo de Phishing")]
    public GameObject[] candados; // Los 3 candados que se ven mientras juegas Phishing.
    private int erroresPhishing = 0; // Cuenta los fallos en Phishing.


    // 2. VARIABLES DEL CONTADOR DE TIEMPO
    [Header("Configuración del Cronómetro")]
    public float tiempoPorModulo = 60f; // Tiempo máximo en segundos para completar un módulo (ej. 60 segundos).
    private float tiempoRestante; // Variable interna que irá bajando segundo a segundo.
    private bool relojActivo = false; // Interruptor: true = el reloj corre, false = el reloj se detiene.
    public TextMeshProUGUI textoReloj; // El texto en pantalla donde se verán los segundos.


    // 3. VARIABLES DE VICTORIA Y ESTADÍSTICAS
    [Header("Pantallas de Fin de Juego")]
    public GameObject panelGameOver; // Panel que se activa si llegas a 3 errores o se acaba el tiempo.
    public GameObject panelVictoria; // Panel de victoria donde mostraremos las estadísticas.

    //public GameObject panelNivelServicio;         ESTO ES PARA QUE DESAPAREZCA EL PANEL DEL NIVEL AL SALIR EL PANEL GAME OVER
    //public GameObject panelNivelPhishing;         ESTO ES PARA QUE DESAPAREZCA EL PANEL DEL NIVEL AL SALIR EL PANEL GAME OVER

    [Header("Elementos de Estadísticas (Dentro del Panel Victoria)")]
    public TextMeshProUGUI textoMensajeVictoria; // Texto que cambiará según las vidas con las que termines.
    public GameObject[] iconosVictoria; // 3 iconos (estrellas) dentro del panel de victoria.


    // 4. SÍMBOLOS DE MÓDULO COMPLETADO (ESTÉTICO)
    [Header("Insignias en el Selector de Módulos")]
    public GameObject simboloCompletadoServicio; // Imagen de "Chulito/Completado" junto al botón de Servicio.
    public GameObject simboloCompletadoPhishing; //Misma imagen junto al botón de Phishing.

    void Start()
    {
        // Por seguridad, nos aseguramos de apagar las pantallas de fin de juego al iniciar.
        panelGameOver.SetActive(false);
        panelVictoria.SetActive(false);

        // Apagamos los símbolos de completado al iniciar el juego por primera vez.
        simboloCompletadoServicio.SetActive(false);
        simboloCompletadoPhishing.SetActive(false);

        // Escondemos el objeto de texto del reloj para que no se vea en el Menú Principal.
        textoReloj.gameObject.SetActive(false);
    }

    // MÉTODO UPDATE: Se ejecuta automáticamente en cada fotograma del juego (muchas veces por segundo). - Es el lugar ideal para manejar cronómetros.
    void Update()
    {
        // Solo descontamos tiempo si el interruptor 'relojActivo' está encendido (true).
        if (relojActivo == true)
        {
            // Time.deltaTime es el tiempo exacto en segundos que pasó desde el último fotograma.
            tiempoRestante = tiempoRestante - Time.deltaTime;

            // Convertimos el tiempo con decimales a un número entero (int) para que se vea limpio en pantalla.
            int segundosEnteros = Mathf.CeilToInt(tiempoRestante);
            textoReloj.text = "Tiempo: " + segundosEnteros.ToString() + "s";

            // Si el tiempo llega a cero o menos, el jugador pierde por tiempo agotado.
            if (tiempoRestante <= 0)
            {
                textoReloj.text = "Tiempo: 0s";
                MostrarGameOver();
            }
        }
    }

    // MÉTODOS PARA INICIAR, REINICIAR MÓDULOS Y OCULTAR EL RELOJ
    // Se llama al entrar al módulo de Servicio para preparar las vidas y arrancar el reloj.
    public void PrepararModuloServicio()
    {
        erroresServicio = 0; // Reiniciamos los errores a cero para poder jugar y volver a jugar normalmente.

        // Encendemos de nuevo las 3 estrellas por si se habían apagado en una partida anterior.
        estrellas[0].SetActive(true);
        estrellas[1].SetActive(true);
        estrellas[2].SetActive(true);

        // Reiniciamos las pantallas de victoria/derrota.
        panelGameOver.SetActive(false);
        panelVictoria.SetActive(false);

        // Hacemos visible el texto del reloj en pantalla y arrancamos la cuenta atrás (el cronómetro).
        textoReloj.gameObject.SetActive(true);
        tiempoRestante = tiempoPorModulo;
        relojActivo = true;
    }

    // Se llama al entrar al módulo de Phishing para preparar los candados y arrancar el reloj.
    public void PrepararModuloPhishing()
    {
        erroresPhishing = 0; // Reiniciamos los errores a cero.

        // Encendemos de nuevo los 3 candados.
        candados[0].SetActive(true);
        candados[1].SetActive(true);
        candados[2].SetActive(true);

        // Reiniciamos pantallas.
        panelGameOver.SetActive(false);
        panelVictoria.SetActive(false);

        // Hacemos visible el texto del reloj en pantalla y arrancamos la cuenta atrás.
        textoReloj.gameObject.SetActive(true);
        tiempoRestante = tiempoPorModulo;
        relojActivo = true;
    }

    // Método para detener y esconder el reloj si el jugador sale al menú antes de terminar (o al ganar, perder o volver al selector).
    public void DetenerReloj()
    {
        relojActivo = false; // Detiene el conteo matemático.

        //Apaga el objeto de texto en la interfaz para que desaparezca de la pantalla.
        textoReloj.gameObject.SetActive(false);
    }

    // MÉTODOS PARA RESTAR VIDAS
    // Esta función se conecta a los botones de "Respuesta Incorrecta" en Servicio al Cliente
    public void RestarEstrella()
    {
        // Verificamos si el jugador aún tiene estrellas (es decir, menos de 3 errores)
        if (erroresServicio < 3)
        {
            // Los Arrays en programación empiezan a contar desde el 0.
            // Si erroresServicio es 0, apagará la estrella en la posición 0.
            estrellas[erroresServicio].SetActive(false);

            // Sumamos 1 al contador de errores
            erroresServicio++;

            // Si después de sumar, llegamos a 3 errores, perdimos.
            if (erroresServicio >= 3)
            {
                MostrarGameOver();
            }
        }
    }

    // Esta función se conecta a los botones de "Respuesta Incorrecta" (marcar como seguro un correo falso)
    public void RestarCandado()
    {
        if (erroresPhishing < 3)
        {
            candados[erroresPhishing].SetActive(false);
            erroresPhishing++;

            if (erroresPhishing >= 3)
            {
                MostrarGameOver();
            }
        }
    }

    // MÉTODOS DE VICTORIA Y ESTADÍSTICAS
    // Esta función la llamará el módulo de Servicio al Cliente al superar sus 5 niveles.
    public void VictoriaServicio()
    {
        //Llamamos a DetenerReloj() para que frene el tiempo y además oculte el texto.
        DetenerReloj();

        relojActivo = false; // Detenemos el cronómetro porque ya ganó.
        panelVictoria.SetActive(true); // Mostramos la pantalla de victoria.

        // Encendemos el símbolo estético de "Completado" en el selector de módulos.
        simboloCompletadoServicio.SetActive(true);

        // Calculamos con cuántas estrellas terminó (3 menos los errores cometidos).
        int estrellasFinales = 3 - erroresServicio;
        ActualizarIconosVictoria(estrellasFinales);

        // Mostramos un mensaje diferente según las estrellas conservadas.
        if (estrellasFinales == 3)
        {
            textoMensajeVictoria.text = "¡Desempeño Perfecto (3/3 Estrellas)!\nAtendiste a todos los clientes con total profesionalidad.";
        }
        else if (estrellasFinales == 2)
        {
            textoMensajeVictoria.text = "¡Buen Trabajo (2/3 Estrellas)!\nSuperaste el turno, pero ten más paciencia con los clientes difíciles.";
        }
        else if (estrellasFinales == 1)
        {
            textoMensajeVictoria.text = "¡Al límite (1/3 Estrellas)!\nEstuviste a punto de ser despedido.";
        }
    }

    // Esta función la llamará el módulo de Phishing al superar sus 5 niveles.
    public void VictoriaPhishing()
    {
        // Llamamos a DetenerReloj() para que frene el tiempo y además oculte el texto.
        DetenerReloj();

        relojActivo = false; // Detenemos el cronómetro.
        panelVictoria.SetActive(true); // Mostramos la pantalla de victoria.

        // Encendemos el símbolo estético de "Completado" en el selector de módulos.
        simboloCompletadoPhishing.SetActive(true);

        // Calculamos con cuántos candados terminó.
        int candadosFinales = 3 - erroresPhishing;
        ActualizarIconosVictoria(candadosFinales);

        // Mostramos un mensaje diferente según los candados conservados.
        if (candadosFinales == 3)
        {
            textoMensajeVictoria.text = "¡Seguridad Impecable (3/3 Candados)!\nDescartaste todas las amenazas sin fallar.";
        }
        else if (candadosFinales == 2)
        {
            textoMensajeVictoria.text = "¡Buen Trabajo (2/3 Candados)!\nProtegiste la seguridad de NexoBank, pero tuviste un descuido. Revisa bien los remitentes y detalles.";
        }
        else if (candadosFinales == 1)
        {
            textoMensajeVictoria.text = "¡Alerta de Riesgo (1/3 Candados)!\nSobreviviste por poco. Un error más y NexoBank habría sido hackeado.";
        }
    }

    // Método auxiliar sencillo para encender 1, 2 o 3 iconos en la pantalla de victoria.
    private void ActualizarIconosVictoria(int cantidadVidas)
    {
        // Primero apagamos los 3 iconos de la pantalla de victoria.
        iconosVictoria[0].SetActive(false);
        iconosVictoria[1].SetActive(false);
        iconosVictoria[2].SetActive(false);

        // Encendemos el primero si terminó con al menos 1 vida.
        if (cantidadVidas >= 1)
        {
            iconosVictoria[0].SetActive(true);
        }
        // Encendemos el segundo si terminó con al menos 2 vidas.
        if (cantidadVidas >= 2)
        {
            iconosVictoria[1].SetActive(true);
        }
        // Encendemos el tercero si terminó con las 3 vidas intactas.
        if (cantidadVidas == 3)
        {
            iconosVictoria[2].SetActive(true);
        }
    }

    // Método interno para mostrar la derrota (por 3 errores o por tiempo agotado).
    private void MostrarGameOver()
    {
        //panelNivelServicio.SetActive(false);           ESTO ES PARA QUE DESAPAREZCA EL PANEL DEL NIVEL AL SALIR EL PANEL GAME OVER
        //panelNivelPhishing.SetActive(false);           ESTO ES PARA QUE DESAPAREZCA EL PANEL DEL NIVEL AL SALIR EL PANEL GAME OVER

        // También ocultamos el reloj si el jugador pierde la partida.
        DetenerReloj();
        relojActivo = false; // Detenemos el reloj inmediatamente.
        panelGameOver.SetActive(true); // Mostramos la pantalla de Game Over.
    }
}
```
