using UnityEngine;
using System.Collections;

public class PauseGame : MonoBehaviour {

	#region Pause Booleans
	/**
	 * Tell us if the game is paused or not.
	 */
	public static bool isGamePaused = false;

	/**
	 * Tell us if the game is frozen or not.
	 */
	public static bool isGameFrozen{ get; private set; }
	#endregion

	#region OnFreezeGame Event
	public delegate void OnFreezeGameHandler(bool isFreezing);
	/**
	 * Called whenever the game is freezed or defreezed
	 */
	public static OnFreezeGameHandler onFreezeGame;
	#endregion

	/**
	 * The Monobehaviour part of the script.
	 * Leave this in some place in the game so the game is pausable or not.
	 * Might not be useful at all or be a pain, so we can take it away.
	 */
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

	#region Freeze-related functions
	/**
	 * Pauses the game without zeroing time scale.
	 */
	public static void FreezeGame(){
		Debug.LogWarning("Game being freezed");
		isGameFrozen = true;
		isGamePaused = true;
		if(onFreezeGame != null)onFreezeGame(true);
	}

	/**
	 * Defreezes the game.
	 */
	public static void DefreezeGame(){
		Debug.LogWarning("Game being defreezed");
		isGameFrozen = false;
		isGamePaused = false;
		if(onFreezeGame != null)onFreezeGame(false);
	}
	#endregion
}
