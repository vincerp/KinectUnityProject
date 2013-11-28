using UnityEngine;
using System.Collections;

public class ChangeScrollType : TriggerScript {
	
	public ScrollType toScrollType;
	public float wait = 0f;
#if UNITY_EDITOR
	public string message;
#endif
	public override void Trigger (bool isOn)
	{
		if(!isOn) return;
#if UNITY_EDITOR
		Debug.Log(message);
#endif
		StartCoroutine("Change");
	}

	public IEnumerator Change(){
		yield return new WaitForSeconds(wait);
		LevelRandomizer.instance.scrollType = toScrollType;
	}
}
