using UnityEngine;
using System.Collections;

public class ScaleIn : MonoBehaviour
{
	[SerializeField]
	private float 	_duration	= 0.25f;
	[SerializeField]
	private float 	_maxScale	= 4.0f;
	[SerializeField]
	private float	_startScale	= 0.001f;
	[SerializeField]
	private float	_endScale 	= 1.0f;

	private float	ScaleRange	= Mathf.PI;	// range of scale in radians
	private	float	EndTime		= 0.0f;
	private float	ScaleRate 	= 0.0f;
	private	float	ScaleAngle	= 0.0f;
	
	private void Start()
	{
		ScaleRange = Mathf.PI - ( Mathf.Asin( _endScale / _maxScale ) );
		ScaleRate = ScaleRange / _duration;
		ScaleAngle = 0.0f;
		EndTime = Time.time + _duration;
		transform.localScale = new Vector3( _startScale, _startScale, _startScale );
	}

	void	Update()
	{
		if ( EndTime <= 0.0f )
		{
			return;
		}
		if ( Time.time >= EndTime )
		{
			transform.localScale = new Vector3( _endScale, _endScale, _endScale );
			EndTime = -1.0f;
			return;
		}
		ScaleAngle += ScaleRate * Time.deltaTime;
		float s = Mathf.Sin( ScaleAngle ) * _maxScale;
		transform.localScale = new Vector3( s, s, s );
	}
}
