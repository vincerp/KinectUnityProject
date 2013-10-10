using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {
	public float speed = 1;

	void FixedUpdate () {
		// This script is pretty complicated, try to wrap your head around this baby...
		transform.Rotate(0,0, speed);
	}
}
