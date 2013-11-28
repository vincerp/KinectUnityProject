using UnityEngine;
using System.Collections;

public class ExplodeWalls : TriggerScript {

	public GameObject particles;
	public Vector3 position = new Vector3(0f, 23f, -2f);

	public override void Trigger (bool isOn)
	{
		if(isOn){
			GameObject go = Instantiate(particles) as GameObject;
			go.transform.position = position;
			if(!audio) gameObject.AddComponent<AudioSource>();
			SoundManager.instance.PlaySoundAt(audio, "ExplodeWalls");
			Destroy(go, 2.95f);
		}
	}
}
