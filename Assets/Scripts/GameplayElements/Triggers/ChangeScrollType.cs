using UnityEngine;
using System.Collections;

public class ChangeScrollType : TriggerScript {
	
	public ScrollType toScrollType;
#if UNITY_EDITOR
	public string message;
#endif
	public override void Trigger (bool isOn)
	{
		if(!isOn) return;
#if UNITY_EDITOR
		Debug.Log(message);
#endif
		LevelRandomizer.instance.scrollType = toScrollType;
	}
}
