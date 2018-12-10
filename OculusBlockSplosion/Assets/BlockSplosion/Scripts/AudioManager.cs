// uncomment for audio debug spam
//#define AUDIO_MANAGER_DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*=============================================
SoundGroup
Holds a group of sounds clips which can be referenced by name.
=============================================*/
[System.Serializable]
public class SoundGroup
{
	[SerializeField]
	public string	_soundName;		// name of this sound group
	
	[SerializeField]
	public float	_volumeScale = 1.0f;	// additional scale for volume
	
	[SerializeField]
	public float	_minDistance = 0.0f;	// if the sound is this distance or less from the listener, play full volume
	
	[SerializeField]
	public float	_maxDistance = 500.0f;	// maximum distance at which the sound is audible
	
	[SerializeField]
	public float	_throttleTime = 0.1f;	// sound will not play twice within this threshold
	
	[SerializeField]
	public float	_minPitch = 0.95f;	// minimum random pitch
	
	[SerializeField]
	public float	_maxPitch = 1.05f;	// sound will not play twice within this threshold
	
	[SerializeField]
	public List< AudioClip >	_audioClips = new List< AudioClip >();
	
	private float	NextAllowedPlayTime = -1.0f;		// the sound group won't play unless Time.time >= this
	
	public void	Play( AudioSource source, float volume )
	{
		if ( source == null )
		{
			Debug.LogWarning( "SoundGroup.Play( <null audio source>, " + volume + " );" );
		}
		if ( Time.time < NextAllowedPlayTime )
		{
			return;
		}
		
		int clipIndex = Mathf.FloorToInt( Random.Range( 0.0f, (float)( _audioClips.Count - 1 ) ) );
		// TODO: handle distance
		// AudioListener al = GetCurrentAudioListener();

		source.pitch = Random.Range( _minPitch, _maxPitch );
		source.PlayOneShot( _audioClips[clipIndex], volume * _volumeScale );
#if AUDIO_MANAGER_DEBUG
		Debug.Log( Time.time + ":SoundGroup.Play( AudioClip:'" + _audioClips[clipIndex].name + "' );" );
#endif
		
		NextAllowedPlayTime = Time.time + _throttleTime;
	}
	
	public void	PlayAt( Vector3 worldPosition, float volume )
	{
		if ( Time.time < NextAllowedPlayTime )
		{
			return;
		}
		
		int clipIndex = Mathf.FloorToInt( Random.Range( 0.0f, (float)( _audioClips.Count - 1 ) ) );
		// TODO: handle distance
		// AudioListener al = GetCurrentAudioListener();
		// looks like this can't be done with PlayClipAtPoint
		AudioOneShot.PlayClip( _audioClips[clipIndex], worldPosition, volume * _volumeScale, 
				Random.Range( _minPitch, _maxPitch ) );

#if AUDIO_MANAGER_DEBUG
		Debug.Log( Time.time + "SoundGroup.PlayAt( '" + _audioClips[clipIndex].name + "', " 
				+ worldPosition.x + ", " + worldPosition.y + ", " + worldPosition.z + " );" );
#endif
		
		NextAllowedPlayTime = Time.time + _throttleTime;
	}	
};

/*=============================================
SoundInfo
Holds info about a sound waiting to be played on a delay.
=============================================*/
public class SoundInfo
{
	public enum ePlayType
	{
		PLAY,
		PLAYAT
	};
	public SoundInfo( ePlayType playType, AudioSource source, string soundGroupName, 
				Vector3 worldPosition, float volume, float playTime ) 
	{
		PlayType		= playType;	
		Source 			= source;
		SoundGroupName 	= soundGroupName;
		WorldPosition	= worldPosition;		
		Volume			= volume;
		PlayTime		= playTime;
	}

	public 	ePlayType	PlayType		= ePlayType.PLAY;		// call to use
	public 	AudioSource	Source;			// source that's playing the clip
	public	string		SoundGroupName;	// name of the sound group to play
	public	Vector3		WorldPosition	= new Vector3( 0.0f, 0.0f, 0.0f );	// position to play at if no source	
	public	float		Volume			= 1.0f;			// volume to play the sound group at
	public 	float 		PlayTime		= 0.0f;		// time when this group should play
};

/*=============================================
AudioManager
I bet this manages audio.
=============================================*/
public class AudioManager : MonoBehaviour
{
	[SerializeField]
	public List< SoundGroup >	_soundGroups = new List< SoundGroup >();	// sound groups set up by the designer
	
	public List< SoundInfo >	SoundInfos = new List< SoundInfo >();	// list of sound groups waiting to play

	// Static singleton property
    public static AudioManager Instance { get; private set; }
	
	/*==========================
	Play()
	Use this method when you want to play a sound that is attached to an object,
	i.e. you need the sound to move with the object.
	NOTE: if you don't here your sound playing, make sure the object that's
	playing the sound is not being removed on the same frame you play the sound.
	If that is the case, you need to use PlayAt, instead.
	==========================*/
	public void Play( AudioSource source, string groupName, float volume = 1.0f, float delay = 0.0f ) 
	{
		int index = FindSoundGroupIndex( groupName );
		if ( index < 0 )
		{
			Debug.LogWarning( "No sound group named " + groupName );
			return;
		}
#if AUDIO_MANAGER_DEBUG
		Debug.Log( Time.time + "AudioManager.Play( '" + ( source == null ? "<null>" : source.name ) + ", '" + 
				groupName + "' );" );
#endif		
		if ( delay > 0.0f ) 
		{
			SoundInfos.Add( new SoundInfo( SoundInfo.ePlayType.PLAY, source, groupName, Vector3.zero, volume, Time.time + delay ) );
			return;
		}
		else
		{
			_soundGroups[index].Play( source, volume );
		}
	}
	
	/*==========================
	PlayAt()
	Use this method when you want to play a sound that is not attached to an AND
	the object is not going to persist until the sound is finished playing.  This
	method is more expensive than Play() because it must create a temporary game
	object and audio source in order to play the sound since Unity doesn't support
	directly playing a sound without an AudioSource, and an AudioSource is a component
	that must be attached to a game object.
	==========================*/	
	public void PlayAt( string groupName, Vector3 worldPosition, float volume = 1.0f, float delay = 0.0f )
	{
		int index = FindSoundGroupIndex( groupName );
		if ( index < 0 )
		{
			Debug.LogWarning( "No sound group named " + groupName );
			return;
		}
#if AUDIO_MANAGER_DEBUG
		Debug.Log( Time.time + "AudioManager.PlayAt( '" + groupName +  "' );" );
#endif		
		if ( delay > 0.0f ) 
		{
			SoundInfos.Add( new SoundInfo( SoundInfo.ePlayType.PLAYAT, null, groupName, worldPosition, volume, Time.time + delay ) );
			return;
		}
		else
		{
			_soundGroups[index].PlayAt( worldPosition, volume );
		}	
	}

	public int FindSoundGroupIndex( string groupName )
	{
		// find the group based on the group name
		for ( int i = 0; i < _soundGroups.Count; ++i )
		{
			if ( _soundGroups[i]._soundName == groupName )
			{
				return i;
			}
		}
		return -1;
	}

	void Awake()
    {
        if ( Instance != null && Instance != this )
        {
			// if there's another instance, destroy it
            Destroy( gameObject );
        }
      
        Instance = this;	// store singleton instance
        DontDestroyOnLoad( gameObject );	// don't destroy on scene changes
    }
	
	public void Update()
	{
		for ( int i = SoundInfos.Count - 1; i >= 0; --i ) 
		{
			SoundInfo soundInfo = SoundInfos[i];
			if ( soundInfo.PlayTime <= Time.time ) 
			{
				int groupIndex = FindSoundGroupIndex( soundInfo.SoundGroupName );
				if ( groupIndex >= 0 )
				{
					if ( soundInfo.PlayType == SoundInfo.ePlayType.PLAYAT || soundInfo.Source == null )
					{
						_soundGroups[groupIndex].PlayAt( soundInfo.WorldPosition, soundInfo.Volume );					
					}
					else
					{
						_soundGroups[groupIndex].Play( soundInfo.Source, soundInfo.Volume );
					}
					// might be faster to copy the last item into this slot, then remove the last item.
					_soundGroups.RemoveAt( groupIndex );
				}
			}
		}
	}
};