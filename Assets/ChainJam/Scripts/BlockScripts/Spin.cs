using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {
	public Vector3 speed = Vector3.up;

	void FixedUpdate () {
		transform.Rotate(speed);
	}
}
