using UnityEngine;
using System.Collections;

public class ReleaseConstraintsOnHit : ExplodeOnHit
{
	[SerializeField]
	private float _forceThreshold = 1;

	private bool _released;

	protected override bool OnSplode()
	{
		if (!_released && (HitByExplosion || _splodeForce > _forceThreshold))
		{
			GetComponent<Rigidbody>().constraints	= RigidbodyConstraints.None;
			_released				= true;
		}
		return false;
	}
}
