using UnityEngine;

public class TimeController : MonoBehaviour
{
	private static TimeController _instance;
	[SerializeField]
	private float _slowTime			= 0.1f;

	[SerializeField]
	private float _slowDownDuration	= 1;

	[SerializeField]
	private float _slowHoldDuration	= 2;

	[SerializeField]
	private float _speedUpDuration	= 2;

	private float _initialFixedDelta;

	private float _nextStateChange;

	private float _timeScale;

	public enum TimeState
	{
		Stopping,
		Slow,
		SpeedingUp,
		Regular,
	}

	private TimeState	_currentState;

	public static TimeState State
	{
		get
		{
			return _instance != null
				? _instance._currentState
				: TimeState.Regular;
		}
	}

	public static void SlowTime()
	{
		if (_instance != null)
			_instance.SlowTimeInternal();
	}

	public static void ResumeTime()
	{
		if (_instance != null)
			_instance._currentState = TimeState.Regular;
	}

	private void SlowTimeInternal()
	{
		switch (_currentState)
		{
			case TimeState.Stopping:
				break;
			case TimeState.Slow:
				_nextStateChange	= Time.time + _slowHoldDuration;
			break;
			default:
				_currentState		= TimeState.Stopping;
				_nextStateChange	= Time.time + _slowDownDuration;
				break;
		}
	}

	private void Start()
	{
		_instance			= this;
		_initialFixedDelta	= Time.fixedDeltaTime;
		_currentState		= TimeState.Regular;
	}

	private void Update()
	{
		if (Time.time > _nextStateChange)
		{
			switch (_currentState)
			{
				case TimeState.Stopping:
					_currentState		= TimeState.Slow;
					_nextStateChange	= Time.time + _slowHoldDuration;
					break;
				case TimeState.Slow:
					_currentState		= TimeState.SpeedingUp;
					_nextStateChange	= Time.time + _speedUpDuration;
					break;
				default:
					_currentState		= TimeState.Regular;
					break;
			}
		}
		switch (_currentState)
		{
			case TimeState.Stopping:
				_timeScale = Mathf.Lerp(_timeScale, _slowTime, 1 - (_nextStateChange - Time.time)/_slowDownDuration);
				break;
			case TimeState.Slow:
				_timeScale = _slowTime;
				break;
			case TimeState.SpeedingUp:
				_timeScale = Mathf.Lerp(_timeScale, 1, 1 - (_nextStateChange - Time.time)/_speedUpDuration);
				break;
			default:
				_timeScale = 1;
				break;
		}
		Time.timeScale		= _timeScale;
		Time.fixedDeltaTime	= _initialFixedDelta*_timeScale;
	}
}
