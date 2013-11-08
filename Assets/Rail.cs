using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Rail : EZLine {

	public Platform platform;
	
	public LineRenderer lr;
	
	void Start(){
		lr = GetComponent<LineRenderer>();
		if(!lr)lr = gameObject.AddComponent<LineRenderer>();
		transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
		if(collider)Destroy(collider);
		lr.material = EZGrabber.instance.GetLinkedItem("RailMaterial") as Material;
	}
	
	void LateUpdate(){
		if( platform.transform == null )
			return;
		
		platform.transform.position = GetPositionInLine(platform.transform.position);
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
	}
}
