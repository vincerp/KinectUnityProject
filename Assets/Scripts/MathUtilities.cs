using UnityEngine;
using System.Collections;

public class MathUtilities{

	public static float SnapTo(float value, float increment){
		return Mathf.Round(value/increment)*increment;
	}
}
			