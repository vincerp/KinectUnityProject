//This script is a piece of shit and might need to be redone
using UnityEngine;
using System.Collections;

public class TutorialScript : MonoBehaviour {

	public TutorialScript other;
	public string inputAxis = "Fire1";
	
	private Platform thisPlatform;
	public Vector3 finalPosition;
	
	public Renderer buttonQuad;
	public Material aPressed;
	public Material aUnpressed;
	
	public GameObject turnOff;
	public GameObject turnOn;
	
	
	public bool hasDragged{
		get{
			return transform.position.y <= 18.5f;
		}
	}
	
	void Start(){
		thisPlatform = GetComponent<Platform>();
	}
	
	void Update(){
		if(other && Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.E)){
			Advance();
			return;
		}

		//buttonQuad.enabled = hasDragged;
		
		if(Input.GetButton(inputAxis)){
			buttonQuad.material = aPressed;
		} else {
			buttonQuad.material = aUnpressed;
		}
		Color nCol = buttonQuad.material.color;
		nCol = new Color(nCol.r, nCol.g, nCol.b, (hasDragged)?1f:0.3f);
		buttonQuad.material.color = nCol;
		
		if(!other) return;
		
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
