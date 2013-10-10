using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
	
	public static SoundManager i;				// The Soundmanager class is basically a Singleton, it should only ever be created once
												//... but we need it like this so we can have them in the inspector
	public AudioClip Jump;
	public AudioClip Squish;
	public AudioClip Respawn;			// Just add more sounds here, and you'll be able to use them anywhere in the code by doing
												// SoundManager.i.Play(SoundManager.i.MySound);	
	public AudioClip applause;
	
	void Awake () {
		i =this;
	}
	
	public void Play(AudioClip clip) 
	{
		audio.PlayOneShot(clip);		
	}
	
}
