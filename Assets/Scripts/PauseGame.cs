using UnityEngine;
using System.Collections;

public class PauseGame : MonoBehaviour {

	public static bool isGamePaused = false;
	
	void Update () {
		if(Input.GetButtonDown("Pause")){
			if(isGamePaused){
				SoundManager.instance.PlaySoundAt(audio, "Depause");
				isGamePaused = false;
				Time.timeScale = 1f;
				return;
			}
			SoundManager.instance.PlaySoundAt(audio, "Pause");
			isGamePaused = true;
			Time.timeScale = 0f;
		}
	}
}
