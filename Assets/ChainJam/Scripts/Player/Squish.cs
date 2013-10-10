using UnityEngine;
using System.Collections;

public class Squish : MonoBehaviour {
	
	public Player player;
	
  	void OnTriggerEnter(Collider other)
	{
		// This script is attached to a small collider, fully inside the player object. 
		// If anything touches it, it means the physics can't take it anymore and we're being squished...
 		if(other.transform.GetComponent<Move>() != null || other.transform.GetComponent<Spin>() != null || other.transform.GetComponent<Scale>() != null)
		{
			// We know what to do
			// ...
			// Sorry
			// ...
			// We'll sing at your funeral
			// ...
			// Goodbye
			player.Squish();
			 
		}
	}
}
