using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour {
	
	public static SoundManager instance;
	
	#region Music-Related
	private AudioSource _musicSource;
	private float _musicVolume = 1f;
	private bool _musicMute = false;
	public float musicVolume{
		get{return _musicVolume;}
		set{
			_musicVolume = value;
			_musicSource.volume = value;
		}
	}
	public bool musicMute{
		get{return _musicMute;}
		set{
			_musicMute = value;
			_musicSource.mute = value;
		}
	}
	#endregion
	
	#region SFX-Related
	private float _sfxVolume = 1f;
	private bool _sfxMute = false;
	public float sfxVolume{
		get{return _sfxVolume;}
		set{
			_sfxVolume = value;
			foreach(AudioSource a in registeredSources) a.volume = value;
		}
	}
	public bool sfxMute{
		get{return _sfxMute;}
		set{
			_sfxMute = value;
			foreach(AudioSource a in registeredSources) a.mute = value;
		}
	}
	#endregion
	
	public List<SoundFile> soundFiles = new List<SoundFile>();
	private List<AudioSource> registeredSources = new List<AudioSource>();
	
	public void Start(){
		if(instance != null && instance != this){
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
		
		if(!_musicSource) {
			_musicSource = gameObject.AddComponent<AudioSource>();
			_musicSource.loop = true;
		}
	}
	
	public void PlaySoundAt(AudioSource source, string soundId){
		if(!registeredSources.Contains(source)) registeredSources.Add(source);
		
		AudioClip _clip = GetSound(soundId);
		if(!_sfxMute)source.PlayOneShot(_clip, _sfxVolume);
		//Debug.Log("Playing sound " + soundId + " at " + source.name);
	}
	
	public AudioClip GetSound(string id){
		return soundFiles.First(x => x.id == id).sound;
	}
}

[System.Serializable]
public class SoundFile{
	public string id;
	public AudioClip sound;
}
