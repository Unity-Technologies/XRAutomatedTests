using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
	public static CameraManager Instance { get; private set; }

	[SerializeField]
	private OVRCameraRig _controller;

	[SerializeField]
	private GameObject _tracking;

	public bool IsFirstPerson { get; private set; }
	public bool IsThirdPerson { get { return !IsFirstPerson; } }

	public static Vector3 Right
	{
		get
		{
			return Instance != null
				? Instance.transform.right
				: Vector3.right;
		}
	}

	public static Vector3 Forward
	{
		get
		{
			return Instance != null
				? Instance.transform.forward
				: Vector3.forward;
		}
	}

	public static Vector3 Up
	{
		get
		{
			return Instance != null
				? Instance.transform.up
				: Vector3.up;
		}
	}

	public enum Mode
	{
		FirstPerson,
		FirstPersonStatic,
		ThirdPerson,
		ThirdPersonKite,
		ThirdPersonStatic,
		ThirdPersonRotationAnchored,
		ThirdPersonOverview,
		ThirdPersonTrackHack,
	}

	[System.Serializable]
	public class Settings
	{
		public Mode		CameraMode		= Mode.ThirdPerson;
		public float	Scale			= 1;
		public float	TrackDistance	= 3;
		public float	TrackSpeed		= 2;
		/// <summary>
		/// How fast the camera lerps to the target position (target position is shifting towards the oberving position at a max speed of TrackSpeed).
		/// </summary>
		public float	TrackLerp		= 1;

		/// <summary>
		/// Camera position offset will be this vector normalized * the TrackDistance.
		/// </summary>
		public Vector3	TrackFacing				= Vector3.forward;
		public Vector3	Position				= Vector3.zero;
		/// <summary>
		/// Target rotation for camera.
		/// </summary>
		public Quaternion Rotation				= Quaternion.identity;
		/// <summary>
		/// Whether or not to update the Position right before new settings are pushed.
		/// </summary>
		public bool		KeepPositionUpdated		= true;
		/// <summary>
		/// Whether or not to update the Rotation right before new settings are pushed.
		/// </summary>
		public bool		KeepRotationUpdated		= true;
		/// <summary>
		/// Whether or not to snap back to the previous position when newest settings are popped.
		/// </summary>
		public bool		SnapToPosition			= true;
		/// <summary>
		/// Whether or not to snap back to the previous rotation when newest settings are popped.
		/// </summary>
		public bool		SnapToRotation			= true;
		/// <summary>
		/// Whether or not to rotate the camera yaw to fallow the player.
		/// </summary>
		public bool		RotateYToPlayer			= false;
		/// <summary>
		/// Whether or not to allow the stick to rotate the camera (no effect if RotateYToPlayer is on).
		/// </summary>
		public bool		AllowRotation			= false;

		/// <summary>
		/// Whether or not the right stick affects TrackDistance.
		/// </summary>
		public bool		AllowDistanceControl	= false;

		/// <summary>
		/// Minimum track distance when using AllowDistanceControl.
		/// </summary>
		public float	MinTrackDistance		= 1;
		/// <summary>
		/// Maximum track distance when using AllowDistanceControl.
		/// </summary>
		public float	MaxTrackDistance		= 10;

		public float	RotationSpeed			= 15;
		public float	RotationLerp			= 1;

		/// <summary>
		/// Whether or not anchored rotation should be fixed to SnapRotationInterval.
		/// </summary>
		public bool		SnapAnchorRotation		= false;

		//Whether or not to directly set the anchor position to Position when using a static camera.
		public bool		UsePosition				= false;
		//Whether or not to directly set the anchor rotation to Rotation when using a static camera.
		public bool		UseRotation				= false;

		/// <summary>
		/// If SnapAnchorRotation is on, then the target rotation is restricted to multiples of this number.
		/// </summary>
		public int		SnapRotationInterval	= 90;

		public bool		AnyRotationEnabled { get { return RotateYToPlayer | AllowRotation; } }

		/// <summary>
		/// Whether or not this should be popped by pressing "Cancel".
		/// </summary>
		public bool		Cancelable			= true;

		/// <summary>
		/// Face position (only used x and z) for ThirdPersonRotationAnchored.
		/// </summary>
		public Vector3	AnchorOrigin		= Vector3.zero;

		public void Set(Settings other)
		{
			if (other == null)
				return;

			var type	= GetType();
			var fields	= type.GetFields();
			foreach (var field in fields)
			{
				var val = field.GetValue(other);
				field.SetValue(this, val);
			}
		}
	}

	/// <summary>
	/// Speed at which input-based rotation on a third-person, non-static, non-kite camera rotates.
	/// </summary>
	public	float	AnchoredRotationSpeed		= 90;

	/// <summary>
	/// Maximum angle difference at which a camera with AllowRotation and SnapAnchorRotation can go to its next SnapRotationInterval.
	/// </summary>
	public	float	AnchoredRotationThreshold	= 4;

	private float	_cameraScale;
	private Quaternion _currentTrackRotation;

	public	float	DirtyJumpTime = 1;
	private float	_lastJump;
	public	void	NotifyJump()
	{
		_lastJump = Time.time;
	}

	public void NotifyLand()
	{
		_lastJump = -DirtyJumpTime;
	}

	private Vector3 _currentTrackPos;

	public	Mode	CurrentMode { get { return CurrentSettings.CameraMode; } }

	/// <summary>
	/// Whether or not to make the camera get pushed back faster so that the player can't walk past it.
	/// </summary>
	public	bool	PushZMinus = true;

	public	Settings CurrentSettings
	{
		get
		{
			if (_settings.Count > 0)
				return _settings.Last.Value;
			return _defaultSettings;
		}
	}
	private LinkedList<Settings> _settings = new LinkedList<Settings>();
	[SerializeField]
	private Settings _defaultSettings = new Settings();

	public	GameObject Tracking
	{
		get { return _tracking; }
		set { _tracking = value; }
	}

	public bool UsingInitialSettings { get { return _settings.Count == 0; } }

	public float CameraScale
	{
		get { return _cameraScale; }
		set
		{
			if (_cameraScale != value)
			{
				_cameraScale = value;
				_controller.transform.localScale = _cameraScale * Vector3.one;
			}
		}
	}

	private void FirstPersonUpdate()
	{
		if (transform.parent != Tracking)
		{
			transform.parent		= Tracking.transform;
			transform.localRotation	= Quaternion.identity;
		}

		var settings				= CurrentSettings;
		transform.localPosition		= Vector3.up*settings.TrackDistance;
	}

	public void WarpTo(Vector3 pos, Quaternion rotation)
	{
		transform.position		= pos;
		transform.rotation		= rotation;
		_currentTrackPos		= pos;
		_currentTrackRotation	= rotation;
	}
	

	private void RefreshModeInfo()
	{
		var mode		= CurrentMode;
		IsFirstPerson	= mode.ToString().StartsWith("FirstPerson");
	}

	public void PushSettings(Settings settings, bool transition = true, System.Action onClose =null)
	{
		if (settings != null)
		{
			var cur = CurrentSettings;
			if (cur.KeepPositionUpdated)
				cur.Position = transform.position; //Keep track of camera position for previous settings.
			if (cur.KeepRotationUpdated)
				cur.Rotation = transform.rotation;

			_settings.AddLast(settings);
			cur = CurrentSettings;
			transform.position	= cur.Position;
			transform.rotation	= cur.Rotation;
			CameraScale			= cur.Scale;

			_currentTrackPos		= cur.Position;
			_currentTrackRotation	= cur.Rotation;

			RefreshModeInfo();

			if (onClose != null)
				onClose();
		}
	}

	public void PopSettings(Settings settings, bool transition = true, System.Action onClose = null)
	{
		if (_settings.Contains(settings))
		{
			_settings.Remove(settings);
			var cur = CurrentSettings;
			if (cur.SnapToPosition)
			{
				transform.position	= cur.Position;
				_currentTrackPos	= cur.Position;
			}

			if (cur.SnapToRotation)
			{
				transform.rotation		= cur.Rotation;
				_currentTrackRotation	= cur.Rotation;
			}

			CameraScale					= cur.Scale;

			RefreshModeInfo();

			if (onClose != null)
				onClose();
		}
	}

	private Vector3 _lastTargetPos;

	private void UpdateCameraPos(ref Vector3 targetPos, Settings settings)
	{
		if (Time.time < _lastJump + DirtyJumpTime)
			targetPos.y = _lastTargetPos.y;

		Vector3 offset		=  targetPos - _currentTrackPos;
//		float offsetToTracking = (_currentTrackPos - Tracking.transform.position).sqrMagnitude;
//		if (PushZMinus && (targetPos - Tracking.transform.position).sqrMagnitude > offsetToTracking && offsetToTracking < settings.TrackDistance*settings.TrackDistance)
//			_currentTrackPos = targetPos;
//		else
			_currentTrackPos 	+= Vector3.ClampMagnitude(Vector3.ClampMagnitude(offset, 1)*settings.TrackSpeed*Time.deltaTime, offset.magnitude);

		transform.position	= Vector3.Lerp(transform.position, _currentTrackPos, settings.TrackLerp*Time.deltaTime);
		Debug.DrawLine(_currentTrackPos, targetPos, Color.yellow);
		Debug.DrawLine(_currentTrackPos, transform.position, Color.green);

		if (settings.RotateYToPlayer || settings.AllowRotation)
		{
			if (settings.RotateYToPlayer)
			{
				offset		= Tracking.transform.position - transform.position;
				offset.y	= 0;

				Quaternion targetRotation	= Quaternion.LookRotation(offset);
				_currentTrackRotation		= Quaternion.RotateTowards(_currentTrackRotation, targetRotation, settings.RotationSpeed*Time.deltaTime);
			}
			else
			{
				_currentTrackRotation *= Quaternion.AngleAxis(Input.GetAxis("RightHorizontal")*settings.RotationSpeed*Time.deltaTime, Vector3.up);
			}

			transform.rotation			= Quaternion.Lerp(transform.rotation, _currentTrackRotation, settings.RotationLerp*Time.deltaTime);
		}
		else
			_currentTrackRotation = transform.rotation;
	}

	private void ThirdPersonGeneralUpdate()
	{
		if (transform.parent != null)
			transform.parent = null;

		var settings = CurrentSettings;
		if (settings.AllowDistanceControl)
		{
			float mag = settings.MaxTrackDistance - settings.MinTrackDistance;
			settings.TrackDistance += mag*Input.GetAxis("RightVertical")*Time.deltaTime;
			settings.TrackDistance = Mathf.Max(Mathf.Min(settings.TrackDistance, settings.MaxTrackDistance), settings.MinTrackDistance);
		}
	}

	private void ThirdPersonUpdate()
	{
		if (Tracking != null)
		{
			var settings = CurrentSettings;
			Vector3 targetPos	=  Tracking.transform.position - settings.TrackFacing.normalized*settings.TrackDistance;
			UpdateCameraPos(ref targetPos, settings);
			_lastTargetPos = targetPos;
		}
	}

	private void ThirdPersonRotationAnchoredUpdate()
	{
		var settings	= CurrentSettings;
		Vector3 offset	= settings.AnchorOrigin - Tracking.transform.position;
		offset.y		= 0;
		offset.Normalize();
		offset.y		= settings.TrackFacing.y;
		Vector3 face	= offset.normalized;
		offset.y		= 0;

		WarpTo(-face*settings.TrackDistance, Quaternion.LookRotation(offset));
	}

	private void ThirdPersonKiteUpdate()
	{
		if (Tracking != null)
		{
			var settings = CurrentSettings;

			Vector3 offset;

			if (settings.AllowRotation)
			{
				offset = -transform.forward;
				offset.y = -settings.TrackFacing.y;
				offset.Normalize();
			}
			else
			{
				Vector3 trackFacingNormal = settings.TrackFacing.normalized;
				offset = _currentTrackPos - Tracking.transform.position;
				offset.y = 0;
				offset.Normalize();
				offset.y = -trackFacingNormal.y;
				offset.Normalize();
			}

			Vector3 targetPos = Tracking.transform.position + offset*settings.TrackDistance;
			UpdateCameraPos(ref targetPos, settings);
			_lastTargetPos = targetPos;
		}
	}

	private void ThirdPersonOverviewUpdate()
	{
		float input			= Input.GetAxis("RightHorizontal");
		float angle			= transform.rotation.eulerAngles.y;

		var settings		= CurrentSettings;

		if (settings.SnapAnchorRotation)
		{
			if (Mathf.Abs(input) > 0.5f && Quaternion.Angle(transform.rotation, _currentTrackRotation) < AnchoredRotationThreshold)
			{
				angle	= Mathf.Round(angle/settings.SnapRotationInterval)*settings.SnapRotationInterval;
				angle	-= Mathf.Sign(input)*settings.SnapRotationInterval;
				_currentTrackRotation = Quaternion.AngleAxis(angle, Vector3.up);
			}
		}
		else
		{
			_currentTrackRotation *= Quaternion.AngleAxis(-input*AnchoredRotationSpeed*Time.deltaTime, Vector3.up);
		}
		//Doing position before rotation because the camera rotation won't get updated by the ovrcontroller until next frame.
		transform.position	= settings.AnchorOrigin - transform.forward*settings.TrackDistance;
		transform.rotation	= Quaternion.Lerp(transform.rotation, _currentTrackRotation, settings.RotationLerp*Time.deltaTime);
	}

	private void ThirdPersonTrackHackUpdate()
	{
		var settings			= CurrentSettings;
		_currentTrackPos		= Vector3.MoveTowards(_currentTrackPos, settings.Position, settings.TrackSpeed*Time.deltaTime);
		_currentTrackRotation	= Quaternion.RotateTowards(_currentTrackRotation, settings.Rotation, settings.RotationSpeed*Time.deltaTime);


		if (settings.UsePosition)
			transform.position		= Vector3.Lerp(transform.position, _currentTrackPos, settings.TrackLerp*Time.deltaTime);
		if (settings.UseRotation)
			transform.rotation			= Quaternion.Lerp(transform.rotation, _currentTrackRotation, settings.RotationLerp*Time.deltaTime);
	}

	private void StaticUpdate()
	{
		var settings = CurrentSettings;

		if (settings.UsePosition)
		{
			transform.position = settings.Position;
			_currentTrackPos = transform.position;
		}

		if (settings.UseRotation)
		{
			transform.rotation = settings.Rotation;
			_currentTrackRotation = transform.rotation;
		}
	}

	private void Update()
	{
		var settings = CurrentSettings;

		CameraScale = settings.Scale;
		
		switch(settings.CameraMode)
		{
			case Mode.FirstPerson:
				FirstPersonUpdate();
				break;
			case Mode.FirstPersonStatic:
				StaticUpdate();
				break;
			case Mode.ThirdPerson:
				ThirdPersonGeneralUpdate();
				ThirdPersonUpdate();
				break;
			case Mode.ThirdPersonKite:
				ThirdPersonGeneralUpdate();
				ThirdPersonKiteUpdate();
				break;
			case Mode.ThirdPersonStatic:
				StaticUpdate();
				ThirdPersonGeneralUpdate();
				break;
			case Mode.ThirdPersonRotationAnchored:
				ThirdPersonGeneralUpdate();
				ThirdPersonRotationAnchoredUpdate();
				break;
			case Mode.ThirdPersonOverview:
				ThirdPersonGeneralUpdate();
				ThirdPersonOverviewUpdate();
				break;
			case Mode.ThirdPersonTrackHack:
				ThirdPersonGeneralUpdate();
				ThirdPersonTrackHackUpdate();
				break;
			default:
				break;
		}

//		float scale = _cameraScale;
//		if (Input.GetKey(KeyCode.I))
//			scale -= Time.deltaTime;
//		if (Input.GetKey(KeyCode.O))
//			scale += Time.deltaTime;
//
//		scale = Mathf.Max(scale, 0.01f);
//
//		CameraScale = scale;
	}

	private void Awake()
	{
		Instance		= this;
	}

	private void Start()
	{
		_controller.transform.localScale = Vector3.one;
		CameraScale		= _defaultSettings.Scale;

		_lastTargetPos	= transform.position;
		_lastJump		= -DirtyJumpTime;

		RefreshModeInfo();

		_currentTrackPos		= transform.position;
		_currentTrackRotation	= transform.rotation;
	}
}
