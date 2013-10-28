using UnityEngine;
using System.Collections.Generic;

public class TestingShitScript : MonoBehaviour {
	
	public float boneAngleAdjustment = 0;
	public float boneAngleFactor = -1;
	public float preciseSnap = 30f;
	public float preciseSnapRange = 3f;
	public float angleSnapFactor = 1f;
	
	public float distance = 5f;
	public float distanceDeadZone = 1f;
	
	private float rotateTowardsAngle;
	
	public float startAt = -190f;
	public float endAt = 190f;
	public float increment = 0.5f;
	
	public List<ValuePairThing> testValues = new List<ValuePairThing>();
	
	void Start () {
		
		for(float i = startAt; i<endAt; i+=increment){
			testValues.Add(new ValuePairThing(i, GimmeSomething(i)));
		}
	}
	
	float GimmeSomething(float val){
		if(distance > distanceDeadZone){
			rotateTowardsAngle = (val+boneAngleAdjustment)*boneAngleFactor;
			//now we snap the value so it doesn't flicker
			if(Mathf.Abs(rotateTowardsAngle)%preciseSnap < preciseSnapRange || Mathf.Abs(rotateTowardsAngle)%preciseSnap > preciseSnap - preciseSnapRange) {
				//here we snap to precise degree angles
				rotateTowardsAngle = MathUtilities.SnapTo(rotateTowardsAngle, preciseSnap);
			} else {
				//here we snap to less precise angles
				rotateTowardsAngle = MathUtilities.SnapTo(rotateTowardsAngle, angleSnapFactor);
			}
		}
		return rotateTowardsAngle;
	}
}

[System.Serializable]
public class ValuePairThing{
	public string name;
	public float valueA, valueB;
	
	public ValuePairThing(){
		valueA = valueB = 0;
		name = "";
	}
	
	public ValuePairThing(float a, float b){
		valueA = a;
		valueB = b;
		name = "" + a + "° - " + b + "°";
	}
}