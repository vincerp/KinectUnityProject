using UnityEngine;
using System.Collections;

public class Lava : MonoBehaviour {

	public float GrowByBlood;
	public Vector3 postition;
	public string lalala;
	public AnimationCurve curve;
	
	void OnCollisionEnter (Collision hit) {
		
		// If the player touches the lava, squish 'em!
		if(hit.rigidbody.tag == "Player")
		{
			hit.rigidbody.GetComponent<Player>().Squish();
			transform.localScale = transform.localScale * GrowByBlood;
		}
    }
}
