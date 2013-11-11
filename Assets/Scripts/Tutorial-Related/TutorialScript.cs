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
		buttonQuad.enabled = hasDragged;
		
		if(Input.GetButton(inputAxis)){
			buttonQuad.material = aPressed;
		} else {
			buttonQuad.material = aUnpressed;
		}
		
		if(!other) return;
		
		if(hasDragged && Input.GetButton(inputAxis) && other.hasDragged && Input.GetButton(other.inputAxis)){
			Debug.Log("YEEEEEEAH");
			turnOn.gameObject.SetActive(true);
			turnOff.gameObject.SetActive(false);
		}
	}
	
	void OnDrawGizmos(){
		Gizmos.DrawWireSphere(finalPosition, 0.5f);
	}
}
