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
	public bool triggerScriptsInThisObject = false;
	
	private int nOfPlayers = 0;
	private int ignoreRaycastLayer = 2;
	
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
		gameObject.layer = ignoreRaycastLayer;
	}
	
	void OnTriggerEnter(Collider col){
		if(hasActivated || col.tag != "Player" || (onlyToPlayer && col.gameObject != onlyToPlayer)) return;
		
		//play sound here
		nOfPlayers ++;
		if(nOfPlayers==1) { //this is such a shitty piece of code
			ButtonChanged();
			if(activateOnlyOnce) hasActivated = true;
			if(triggerScriptsInThisObject) SendMessage("Trigger", isActive);
			animation.Play("on");
		}
	}
	
	void OnTriggerExit(Collider col){
		if(hasActivated || col.tag != "Player" || (onlyToPlayer && col.gameObject != onlyToPlayer)) return;
		
		//play sound here
		nOfPlayers --;
		if(nOfPlayers == 0) { //this is such a shitty piece of code
			ButtonChanged();
			if(triggerScriptsInThisObject) SendMessage("Trigger", isActive);
			animation.Play("off");
		}
	}
}
