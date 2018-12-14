using UnityEngine;
using System.Collections;

public class TitleText : MonoBehaviour
{
	[SerializeField]
	private TextMesh	MyTextMesh;

	[SerializeField]
	private float		TitleDuration = 15.0f;

	[SerializeField]
	private bool		ScaleIn = false;

	[SerializeField]
	private float		ScaleDelay = 0.0f;

	[SerializeField]
	private float		ScaleRate = 0.2f;

	[SerializeField]
	private float		ScaleMax = 1.0f;

	private float		StartTime;
	private float		ScaleFactor;

	private void Awake()
	{
		StartTime = 0.0f;
		ScaleFactor = 1.0f;
		ScaleMax = 1.0f;
	}

	private void Start()
	{
		StartTime = Time.time;
		if ( ScaleIn )  
		{
			ScaleFactor = 0.01f;
			transform.localScale = new Vector3( ScaleFactor, ScaleFactor, ScaleFactor );
		}
	}

	private void Update()
	{
		if ( ScaleIn && Time.time > StartTime + ScaleDelay ) 
		{
			ScaleFactor = ScaleFactor + ScaleRate * Time.deltaTime;
			if ( ScaleFactor > ScaleMax ) 
			{
				ScaleIn = false;
				ScaleFactor = ScaleMax;
			}
			transform.localScale = new Vector3( ScaleFactor, ScaleFactor, ScaleFactor );
		}

		if ( Time.time > StartTime + TitleDuration && MyTextMesh.text != "" ) 
		{
			MyTextMesh.text = "";
		}
	}
}
