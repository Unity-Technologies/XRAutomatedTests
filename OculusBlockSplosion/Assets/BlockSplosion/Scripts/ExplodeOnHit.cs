using UnityEngine;
using System.Collections.Generic;

public class ExplodeOnHit : Block
{
	[SerializeField]
	private List<string> _explodeTag = new List<string>();

	[SerializeField]
	private float _explosionRadius	= 8;

	[SerializeField]
	private float _explosionForce	= 20;

	[SerializeField]
	private string _sound			= "Explosion";

	//[SerializeField]
	//private AudioClip	ExplosionSound;

	[SerializeField]
	private bool _slowTime;

	protected float _splodeForce { get; private set; }

	protected override bool OnSplode()
	{
		// Nelno: never slow down time
		//if (_slowTime)
		//	TimeController.SlowTime();
		if (!string.IsNullOrEmpty(_sound) )
		{
			AudioManager.Instance.PlayAt( _sound, transform.position, 1.0f );
		}

		if (_explosionRadius > 0)
			CreateExplosion(transform.position, _explosionRadius, _explosionForce);
		return true;
	}

	private void OnCollisionEnter(Collision collision)
	{
		base.OnCollide(collision);
		if (_explodeTag.Count == 0 || _explodeTag.Contains(collision.collider.tag))
		{
			TriggerSplode();
			_splodeForce = Mathf.Max(_splodeForce, collision.relativeVelocity.magnitude);
		}
	}
}
