using UnityEngine;
using System.Collections;

public class ActivateLava : TriggerScript {

	Transform lavaTransform;
	public float speed = 1f;

	public override void Trigger (bool isOn)
	{
		if(isOn){
			lavaTransform = GameObject.FindWithTag("Lava").transform;
			Debug.Log("Is this working?");
			StartCoroutine(AnimateLava());
		}
	}

	private IEnumerator AnimateLava(){
		while(lavaTransform.position != Vector3.zero){
			lavaTransform.position = Vector3.MoveTowards(lavaTransform.position, Vector3.zero, speed * Time.deltaTime);
			yield return null;
		}
	}
}
