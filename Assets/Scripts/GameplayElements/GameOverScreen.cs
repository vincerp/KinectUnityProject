using UnityEngine;
using System.Collections;

public class GameOverScreen : MonoBehaviour {
	
	public static GameOverScreen instance;
	
	public GameObject p1, p2, pA;
	
	void Start () {
		instance = this;
		p1.SetActive(false);
		p2.SetActive(false);
		pA.SetActive(false);
	}
	
	public void PlayerDied (int player) {
		pA.SetActive(true);
		
		if(player == 1) p1.SetActive(true);
		if(player == 2) p2.SetActive(true);
		
		Time.timeScale = 0f;
		SoundManager.instance.sfxMute = true;
		StartCoroutine(WaitForA());
	}
	
	public IEnumerator WaitForA(){
		bool buttonWasPressed = false;
		//yield return new WaitForSeconds(0.2f);
		while(!buttonWasPressed){
			if(Input.GetButton("Jump") || Input.GetButton("p2A")){
				buttonWasPressed = true;
				pA.GetComponent<TextMesh>().text = "Loading...";
			}
			yield return null;
		}
		Application.LoadLevel(0);
		Time.timeScale = 1f;
		SoundManager.instance.sfxMute = false;
	}

	void Update(){
		TextMesh tm = pA.GetComponent<TextMesh>();
		if(!tm.text.Contains("Loading"))return;
		int nOfDots = (int)Time.realtimeSinceStartup%2;
		tm.text = "Loading";
		for(int i = 0; i <= nOfDots; i++){
			tm.text += ".";
		} 
	}
}
