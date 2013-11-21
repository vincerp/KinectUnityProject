using UnityEngine;
using System.Collections;

public class IntroAnimationSelector : MonoBehaviour {

	public enum IntroAnimType{
		JETPACK = 1,
		IRON_BOOTS = 2
	}

	public IntroAnimType animType;

	void Awake () {
		Animator a = GetComponent<Animator>();
		a.SetInteger("animType", (int)animType);
	}
}
