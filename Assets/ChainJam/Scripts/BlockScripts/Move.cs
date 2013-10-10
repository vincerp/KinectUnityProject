
using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {
	public float time;									// The time it takes to reach the destination
	public Vector3 destination;							// The destination, relative to the object
	public float delay;									// If you don't want it to start right away...
	public float initialDelay;							// Or maybe you just want it to have a delay in the beginning.
	public bool yoyo;									// There and travel back or there and teleport back
	
	private Vector3 startPosition;
		
	void Start() {
		startPosition = transform.position;				// We need to remember where the player started
		GoThere (initialDelay);							// Start the first tween
	}
	
	void GoThere(float extraDelay=0) {		
		// If Yoyo is set, this is redundent, otherwise it puts the block back to it's original position.
		transform.position = startPosition;
		
		// The syntax for a tween is a little hard to read... bear with me
		// We call the MoveTo, with the object that has this script.
		// Then we give it a HashTable, which is just a list of parameters
		// 
		// It looks like this: iTween.Hash(property, value, property, value, ....)
		// 
		// You can find all of the possible parameters for a tween here: http://itween.pixelplacement.com/documentation.php
		
		iTween.MoveTo(gameObject, iTween.Hash(
			"position",startPosition+ destination,
			"time",time,
			"oncomplete",(yoyo?"GoBack":"GoThere"),  // in this line, if it's a yoyo, then go back, otherwise just repeat this function
			"oncompleteparams", 0,
			"easetype","linear",
			"delay",delay + extraDelay));				
									
	}
	
	void GoBack(float extraDelay=0) {
		// Tween back to the original position		
		iTween.MoveTo(gameObject, iTween.Hash(
			"position",startPosition,
			"time",time,
			"oncomplete", "GoThere",
			"oncompleteparams", 0,
			"easetype","linear",
			"delay",delay+extraDelay));
	}
	
	void OnDrawGizmosSelected() 
	{
		// This is some Unity Inspector Voodoo! 
		// It draws the line you see when you pick the destination.
		Gizmos.color = Color.magenta;
		if(startPosition != Vector3.zero)
		{
			Gizmos.DrawLine(startPosition,startPosition + destination);
		}
		else
		{
			Gizmos.DrawLine(transform.position,transform.position + destination);
		}
	}

}
