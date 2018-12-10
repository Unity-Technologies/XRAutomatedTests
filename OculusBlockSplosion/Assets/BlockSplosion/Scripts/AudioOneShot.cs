using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*=============================================
AudioOneShot
This is a game object that can play a sound clip with pitch adjustment
=============================================*/
public class AudioOneShot : MonoBehaviour
{
	private float Length = 0.0f;
	
	public static void PlayClip( AudioClip clip, Vector3 worldPosition, float volume, float pitch )
	{	
		GameObject gameObject = new GameObject( "AudioOneShotObject" );
		AudioOneShot audioOneShot = gameObject.AddComponent<AudioOneShot>() as AudioOneShot;
		audioOneShot.PlayClipInternal( clip, worldPosition, volume, pitch );
	}
	
	private void PlayClipInternal( AudioClip clip, Vector3 worldPosition, float volume, float pitch )
	{
		gameObject.transform.position = worldPosition;	

		AudioSource audioSource = gameObject.AddComponent<AudioSource>(  ) as AudioSource;
		audioSource.clip = clip;
		audioSource.volume = volume;
		audioSource.pitch = pitch;
		audioSource.playOnAwake = false;
		audioSource.Play();
		
		Length = clip.length;
		
		StartCoroutine( "WaitForSoundToFinish" );
	}
	
	IEnumerator WaitForSoundToFinish()
	{
		yield return new WaitForSeconds( Length );
		GameObject.Destroy( gameObject );
	}
};