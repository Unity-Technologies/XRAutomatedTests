using UnityEngine; 
using System.Collections; 

[RequireComponent( typeof( ParticleSystem ) )]
public class DestroyOnParticleSystemDeath : MonoBehaviour
{
	void OnEnable() { StartCoroutine( "WaitForParticleSystemDeath" ); }
	
	IEnumerator	WaitForParticleSystemDeath()
	{
		while( true )
		{
			yield return new WaitForSeconds( 0.5f );
			if ( !GetComponent<ParticleSystem>().IsAlive( true ) )
			{
				GameObject.Destroy( gameObject );
				break;
			}
		}
	}
};