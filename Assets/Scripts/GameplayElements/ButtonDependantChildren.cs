using UnityEngine;
using System.Collections.Generic;

public class ButtonDependantChildren : MonoBehaviour {
	
	public bool not = false; //inverts checking behaviour
	public bool activateOnlyOnce = true;
	private bool hasActivated = false;
	
	public List<Button> buttonsDependant = new List<Button>();
	
	public List<GameObject> children = new List<GameObject>();
	
	private void Start(){
		foreach(Transform child in transform){
			children.Add(child.gameObject);
			child.gameObject.SetActive(not);
		}
		foreach(Button bt in buttonsDependant){
			bt.ButtonChanged += VerifyIfActive;
		}
	}
	
	void VerifyIfActive () {
		if(hasActivated) return;
		bool isActive = true;
		foreach(Button bt in buttonsDependant){
			if(!bt.isActive) isActive = false;
		}
		if(activateOnlyOnce && isActive) hasActivated = true;
		
		foreach(GameObject child in children){
			child.SetActive(
				(not)?!isActive:isActive
			);
		}
	}
}
