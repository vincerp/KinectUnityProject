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
		StartCoroutine(WaitForA());
	}
	
	public IEnumerator WaitForA(){
		while(!Input.GetButton("Jump") || !Input.GetButton("p2A")){
			yield return null;
		}
		Application.LoadLevel(0);
		Time.timeScale = 1f;
	}
}
