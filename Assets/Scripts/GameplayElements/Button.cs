using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Button : MonoBehaviour {
	
	public delegate void ButtonEvent();
	public event ButtonEvent ButtonChanged;
	
	public GameObject onlyToPlayer;
	public bool not = false;
	public bool activateOnlyOnce = true;
	private bool hasActivated = false;
	
	private int nOfPlayers = 0;
	
	private bool wasValue;
	public bool isActive{
		get{
			bool val = nOfPlayers>0;
			if(not) val = !val;
			return val;
		}
	}
	
	void Start () {
		collider.isTrigger = true;
		
	}
	
	void OnTriggerEnter(Collider col){
		if(hasActivated || col.tag != "Player" || (onlyToPlayer && col.gameObject != onlyToPlayer)) return;
		
		//play sound here
		nOfPlayers ++;
		if(nOfPlayers==1) ButtonChanged(); //this is such a shitty piece of code
		if(activateOnlyOnce) hasActivated = true;
	}
	
	void OnTriggerExit(Collider col){
		if(hasActivated || col.tag != "Player" || (onlyToPlayer && col.gameObject != onlyToPlayer)) return;
		
		//play sound here
		nOfPlayers --;
		if(nOfPlayers == 0) ButtonChanged(); //this is such a shitty piece of code
	}
}
