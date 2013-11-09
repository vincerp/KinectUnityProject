using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Rail : EZLine {

	public Platform platform;
	
	private LineRenderer lr;
	
	void Start(){
		lr = gameObject.AddComponent<LineRenderer>();
	}
	
	void LateUpdate(){
		platform.transform.position = GetPositionInLine(platform.transform.position);
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
	}
}
