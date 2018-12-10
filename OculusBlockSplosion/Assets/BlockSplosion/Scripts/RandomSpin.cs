using UnityEngine;
using System.Collections;

public class RandomSpin : MonoBehaviour
{
	private Vector3	Axis;
	private float	SpinRate = 0.0f;
	
	[SerializeField]
	private	float	MinSpinRate = 15.0f;
	
	[SerializeField]
	private	float	MaxSpinRate = 60.0f;
	
	[SerializeField]
	private float	ScaleRange = 0.5f;
	
	[SerializeField]
	private float	MinScale = 0.75f;
	
	[SerializeField]
	private float	ScaleRateOfChange = 90.0f;
	
	private float	ScaleAngle = 0.0f;	// this increases linearly as input for Sine()
	
	[SerializeField]
	private float	ScaleRateRateOfChange = 33.0f;
	
	private float 	ScaleRateAngle = 0.0f;
	
	private void Awake()
	{
		Axis = new Vector3( Random.value, Random.value, Random.value );
		Axis.Normalize();
		transform.localRotation = Quaternion.AngleAxis( 0.0f, Axis );
		SpinRate = ( Random.value * ( MaxSpinRate - MinSpinRate ) ) + MinSpinRate;
	}
	
	private void Update()
	{
		Quaternion q = Quaternion.AngleAxis( SpinRate * Time.deltaTime, Axis );
		transform.localRotation *= q;
		
		if ( ScaleRange > 0.0f )
		{
			const float DEG2RAD = Mathf.PI / 180.0f;
			float curScaleRateOfChange = ScaleRateOfChange;
			if ( ScaleRateRateOfChange > 0.0f )
			{
				ScaleRateAngle += ScaleRateRateOfChange * Time.deltaTime;
				if ( ScaleRateAngle > 360.0f )
				{
					ScaleRateAngle -= 360.0f;
				}
				curScaleRateOfChange = ScaleRateOfChange * ( Mathf.Sin( ScaleRateAngle * DEG2RAD ) + 1.0f );
			}
		
			ScaleAngle += curScaleRateOfChange * Time.deltaTime;
			if ( ScaleAngle > 360.0f ) 
			{
				ScaleAngle -= 360.0f;
			}	
			
			float scale = MinScale + ( ( Mathf.Sin( ScaleAngle * DEG2RAD ) + 1.0f ) * ScaleRange ); 
			transform.localScale = new Vector3( scale, scale, scale );
		}
	}
};