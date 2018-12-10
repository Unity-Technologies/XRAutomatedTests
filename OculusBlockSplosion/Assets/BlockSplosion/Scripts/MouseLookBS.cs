/************************************************************************************

Filename    :   MouseLook.cs
Content     :   This only exists to allow capturing of mouse input within the Unity editor.
Created     :   March 8, 2014
Authors     :   Jonathan E. Wright

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Use of this software is subject to the terms of the Oculus LLC license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

************************************************************************************/

using UnityEngine;
using System.Collections;

//==============================================
// MouseLook
//
// There's almost certainly a better way to do this but it was written on a moment's
// notice before with very little working knowledge of Unity. :)
//==============================================
public class MouseLookBS : MonoBehaviour
{
#if ( UNITY_EDITOR )
	[SerializeField]
	private bool	AllowMouseLook = true;
	
	[SerializeField]
	private bool	InvertPitch = true;
	
	[SerializeField]
	private bool	LockMouse = true;

	[SerializeField]
	private float	MouseSensitivityPitch = 5.0f;
	
	[SerializeField]
	private float	MouseSensitivityYaw = 5.0f;
	
	private Vector3	MouseAngles = new Vector3( 0.0f, 0.0f, 0.0f );
#endif

	private bool	CaptureMouseCursor = false;
	
	public  void	SetCaptureMouseCursor( bool capture ) { CaptureMouseCursor = capture; }
	public 	bool	GetCaptureMouseCursor() { return CaptureMouseCursor; }
	
	void Update()
	{	
#if ( UNITY_EDITOR ) 
		if ( AllowMouseLook )
		{
			if ( LockMouse && CaptureMouseCursor )
			{
#if UNITY_5
				Cursor.visible = false; 
				Cursor.lockState = CursorLockMode.Locked;
#else
				Screen.lockCursor = true;
#endif
			}
			else
			{
#if UNITY_5
				Cursor.visible = true; 
				Cursor.lockState = CursorLockMode.None;
#else
				Screen.lockCursor = false;
#endif
			}

			if ( CaptureMouseCursor )
			{
				float MousePitchDelta = Input.GetAxis( "Mouse Y" ) * MouseSensitivityPitch;
				float MouseYawDelta = Input.GetAxis( "Mouse X" ) * MouseSensitivityYaw * -1.0f;
				if ( InvertPitch )
				{
					MouseYawDelta = -MouseYawDelta;
				}
					
				MouseAngles.x += MousePitchDelta;
				MouseAngles.y += MouseYawDelta;			
				if ( MousePitchDelta > 0.0f )
				{
					if ( MouseAngles.x > 89.5f ) { MouseAngles.x = 89.5f; }
				} else if ( MousePitchDelta < 0.0f )
				{
					if ( MouseAngles.x < -89.5f ) { MouseAngles.x = -89.5f; }
				}
				MouseAngles.y = Mathf.Clamp( MouseAngles.y, -360.0f, 360.0f );

				Quaternion q = Quaternion.Euler( MouseAngles.x, MouseAngles.y, 0.0f );
				OVRCameraRig cameraController = GetComponent< OVRCameraRig >();
				cameraController.trackingSpace.transform.localRotation = q;
			}
		}
#endif
	}
};