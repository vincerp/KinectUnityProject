using UnityEngine;
using System.Collections;

public class AbilitySettings : ScriptableObject {
	public string scriptName;
	public float duration;
	public float abilityDamage;
	public GameObject particles;
	public string pickupSound;
	public GameObject animation;

#if UNITY_EDITOR
	public string notes;
#endif
}
