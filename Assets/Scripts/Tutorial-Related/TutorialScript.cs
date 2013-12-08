//This script is a piece of shit and might need to be redone
using UnityEngine;
using System.Collections;

public class TutorialScript : MonoBehaviour {

	public bool thisIsTheOne = false;

	public TutorialScript other;
	public string inputAxis = "Fire1";
	public BonesArea magnet;
	
	private Platform thisPlatform;
	public Vector3 finalPosition;
	
	public Renderer buttonQuad;
	public Material aPressed;
	public Material aUnpressed;
	public Material rbPressed;
	public Material rbUnpressed;

	public ButtonTrail trail;

	public GameObject turnOff;
	public GameObject turnOn;
	
	
	public bool hasDragged{
		get{
			return transform.position.y <= 18.5f;
		}
	}
	
	void Start(){
		thisPlatform = GetComponent<Platform>();
		trail.isOn = false;
		trail.color = Color.gray;
	}
	
	void Update(){
		if(thisIsTheOne && Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.E)){
			Advance();
			return;
		}

		//buttonQuad.enabled = hasDragged;
		if(hasDragged){
			if(trail.color != Color.white) trail.color = Color.white;
			if(Input.GetButton(inputAxis)){
				buttonQuad.material = aPressed;
				trail.isOn = true;
			} else {
				buttonQuad.material = aUnpressed;
				trail.isOn = false;
			}
		} else {
			if(trail.color != Color.gray) trail.color = Color.gray;
			if(trail.isOn)trail.isOn = false;
			if(Input.GetKey(magnet.bonesInput.grabKey)){
				buttonQuad.material = rbPressed;
			} else {
				buttonQuad.material = rbUnpressed;
			}
		}
		Color nCol = buttonQuad.material.color;
		if(hasDragged)nCol = new Color(nCol.r, nCol.g, nCol.b, (Input.GetButton(other.inputAxis))?1f:0.5f);
		buttonQuad.material.color = nCol;
		
		if(!thisIsTheOne) return;
		
		if(hasDragged && Input.GetButton(inputAxis) && other.hasDragged && Input.GetButton(other.inputAxis)){
			Advance();
		}
	}

	void Advance(){
		Debug.Log("YEEEEEEAH");
		
		turnOff.gameObject.SetActive(false);
		
		BonesArea[] magnets = Component.FindObjectsOfType(typeof(BonesArea)) as BonesArea[];
		foreach(BonesArea magnet in magnets)
		{
			magnet.platforms.Clear();
			magnet.currentPlatform = null;
		}
		
		turnOn.gameObject.SetActive(true);
	}
	
	void OnDrawGizmos(){
		Gizmos.DrawWireSphere(finalPosition, 0.5f);
	}
}
