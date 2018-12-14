using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Block : MonoBehaviour
{
	[SerializeField]
	private Color _editorColor = Color.magenta;

	public Color EditorColor { get { return _editorColor; } }

	public const float ZForceScalar = 0.5f;

	public enum Type
	{
		Plain,
		Special,
	}

	private static int[] _blockCounts = new int[System.Enum.GetNames(typeof(Type)).Length];

	[SerializeField]
	private GameObject	DeathPrefab;

	[SerializeField]
	private bool		_useOrientation = true;

	[SerializeField]
	private float		_yOffset		= 0;

	[SerializeField]
	private Type		_blockType;

	[SerializeField]
	private string		_prefabName = "Not specified. :(";

	protected bool HitByExplosion { get; private set; }

	public string PrefabName { get { return _prefabName; } }
	
	public float  SortDistance { get; set; }

	[SerializeField]
	private bool		_diesOnExplosionHit = false;

	private bool		_sploded;

	public Type			BlockType
	{
		get {	return _blockType;	}
		set {	_blockType = value;	}
	}

	protected virtual bool OnSplode() { return true; }

	[SerializeField]
	private float _timeBeforeSplode	= 0.25f;
	
	[SerializeField]
	private string _hitSound;

	[SerializeField]
	private float _minForceForHitSound = 1;

	[SerializeField]
	private string _hitShotSound;

	//[SerializeField]
	//private AudioClip HitShotSound;

	private const float _hitVelocityVolumeScalar	= 0.25f;
	private const float _minVolume					= 0.1f;

	private float _nextSplode;

	private bool _sploding;
	
	public static void CreateExplosion(Vector3 pos, float radius, float force)
	{
		var colliders = Physics.OverlapSphere(pos, radius);
		foreach (var hit in colliders)
		{
			if (hit.GetComponent<Rigidbody>() != null)
			{
				hit.GetComponent<Rigidbody>().AddExplosionForce(force, pos, radius, 1, ForceMode.Impulse);
				Vector3 offset = Camera.main.transform.position - pos;
				offset.y = 0;
				hit.GetComponent<Rigidbody>().AddForce(force*ZForceScalar*offset.normalized, ForceMode.Impulse);
			}

			var block = hit.GetComponent<Block>();
			if (block != null)
				block.OnHit();
		}
	}
	
	public static int GetCount(Type type)
	{
		return _blockCounts[(int)type];
	}
	
	private void Awake()
	{
		++_blockCounts[(int)_blockType];
	}

	private void OnDestroy()
	{
		//print( Time.time + " : Object " + name + ".OnDestroy()" );
		--_blockCounts[(int)_blockType];
		GameObject bgm = GameObject.Find( "BlockGameManager" );
		if ( bgm != null ) 
		{
			bgm.GetComponent< BlockGameManager >().RemoveBlock( gameObject );
		}
	}
	
	public void TriggerSplode()
	{
		if (!_sploding)
		{
			_nextSplode	= Time.time + _timeBeforeSplode;
			_sploding	= true;
		}
	}

	private void OnHit()
	{
		HitByExplosion = true;
		if (_diesOnExplosionHit)
			TriggerSplode();
	}

	private void Update()
	{
		if (_sploding && Time.time > _nextSplode)
		{
			_sploding = false;
			Splode();
		}
	}

	protected void OnCollide(Collision collision)
	{
		if (collision.collider.CompareTag("Shot"))
		{
			if (!string.IsNullOrEmpty(_hitShotSound))
			{
				AudioManager.Instance.Play( GetComponent<AudioSource>(), _hitShotSound, 1.0f );
			}
		}
		else if (!string.IsNullOrEmpty(_hitSound))
		{
			float mag = collision.relativeVelocity.magnitude;
			if (mag > _minForceForHitSound)
			{
				AudioManager.Instance.Play( GetComponent<AudioSource>(), _hitSound, _minVolume + ( mag - _minForceForHitSound) * _hitVelocityVolumeScalar );
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		OnCollide(collision);
	}

	public void Splode()
	{
		if (!_sploded)
		{
			if (DeathPrefab != null) {
				//print( "Instantiating " + DeathPrefab.name + "..." );
				Instantiate( DeathPrefab, transform.position + Vector3.up*_yOffset, _useOrientation ? transform.rotation : DeathPrefab.transform.rotation) ;
				//print( "Done" );
			}

			_sploded = true;
			//This way this cannot be triggered again until OnSplode resolves.
			_sploded = OnSplode();

			if (_sploded)
				Destroy(gameObject);
		}
	}
	
	public Vector3 GetWorldspacePos() {
		if ( transform.parent == null ) {
			return transform.position;
		}
		Vector3 pos = new Vector3();
		pos = transform.parent.transform.position + ( transform.parent.rotation * transform.position );
		return pos;
	}
}