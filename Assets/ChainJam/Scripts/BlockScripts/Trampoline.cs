using UnityEngine;
using System.Collections.Generic;

public class Trampoline : MonoBehaviour {
	
	public float multiplier = 2;									// The jump speed of the player will be multiplied by this

	void OnCollisionEnter (Collision collision) {
		
		// When the player lands on the trampoline, set the multiplier 
		if(collision.rigidbody.tag == "Player")
		{
			collision.rigidbody.GetComponent<Player>().jumpStrengthMultiplier = multiplier;
		}
    }
	
	void OnCollisionExit (Collision collision) {
		// The moment they leave, take it off
		if(collision.rigidbody.tag == "Player")
		{
			collision.rigidbody.GetComponent<Player>().jumpStrengthMultiplier = 1;
		}
    }
}
