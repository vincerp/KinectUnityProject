using UnityEngine;
using System.Collections;

public class PlayMusicOnStart : MonoBehaviour {

	public string musicId;
	private bool playedAlready = false;

	IEnumerator Start(){
		yield return new WaitForEndOfFrame();
		if(!playedAlready){
			PlayIt();
		}
	}

	void OnEnable(){
		if(playedAlready) return;
		PlayIt();
	}

	void PlayIt(){
		SoundManager.instance.ChangeMusic(musicId);
		playedAlready = true;
	}
}
