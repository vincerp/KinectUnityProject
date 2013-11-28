using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonDependantChildren : MonoBehaviour {
	
	public bool not = false; //inverts checking behaviour
	public bool activateOnlyOnce = true;
	private bool hasActivated = false;
	public bool triggerScriptsInThisObject = false;
	
	public List<Button> buttonsDependant = new List<Button>();
	
	public List<GameObject> children = new List<GameObject>();
	public List<ButtonTrail> trails = new List<ButtonTrail>();
	
	private IEnumerator Start(){
		yield return new WaitForEndOfFrame();
		ButtonTrail btr;
		foreach(Transform child in transform){
			btr = null;
			btr = child.GetComponent<ButtonTrail>();
			if(btr){
				//if it has trail, it is a trail
				trails.Add(btr);
				btr.isOn = not;
			} else {
				//if not, lets make it disapear
				children.Add(child.gameObject);
				child.gameObject.SetActive(not);
			}
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
		if(triggerScriptsInThisObject) SendMessage("Trigger", isActive);
		
		foreach(GameObject child in children){
			if(child.activeSelf == (not)?!isActive:isActive) continue;
			child.SetActive(
				(not)?!isActive:isActive
			);
			GameObject go = Instantiate( EZGrabber.instance.GetLinkedItem("PlatformDust") as GameObject ) as GameObject;
			go.transform.position = child.transform.position + Vector3.back*2f;
			go.transform.rotation = child.transform.rotation;
			Destroy(go, 2.95f);
		}
		foreach(ButtonTrail trail in trails){
			trail.isOn = (not)?!isActive:isActive;
		}
	}
}
