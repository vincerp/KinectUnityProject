using UnityEngine;
using System.Collections;

public class Scale : MonoBehaviour {
	public float time;									// The time it takes to scale up or down all the way
	public Vector3 destinationScale;					// The destination scale
	
	private Vector3 startScale;
		
	void Start() {
		startScale = transform.localScale;				// Let's remember the original scale, so we can scale back down
		GoThere ();										// Start the first tween
	}
	
	void GoThere() {
		// The syntax for a tween is a little hard to read... bear with me
		// We call the ScaleTo, with the object that has this script.
		// Then we give it a HashTable, which is just a list of parameters
		// 
		// It looks like this: iTween.Hash(property, value, property, value, ....)
		// 
		// You can find all of the possible parameters for a tween here: http://itween.pixelplacement.com/documentation.php		
		
		iTween.ScaleTo(gameObject, iTween.Hash(
			"x",destinationScale.x + startScale.x,		// We're scaling to whatever the base scale is, with the destination scale
			"y",destinationScale.y  + startScale.y,
			"time",time,
			"oncomplete","GoBack",
			"easetype","linear"));
	}
	
	void GoBack() {
		iTween.ScaleTo(gameObject, iTween.Hash(
			"scale",startScale,
			"time",time,
			"oncomplete", "GoThere",
			"easetype","linear"));
	}
}
