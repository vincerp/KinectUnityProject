using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Rail : EZLine {

	public Platform platform;
	
	public LineRenderer lr;
	private Transform tr;

	public bool canUpdate = false;
	
	void Start(){
		tr = transform;
		lr = GetComponent<LineRenderer>();
		if(!lr)lr = gameObject.AddComponent<LineRenderer>();
		transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
		if(collider)Destroy(collider);
		lr.material = EZGrabber.instance.GetLinkedItem("RailMaterial") as Material;
	}
	
	public void LateUpdate(){
		if( platform == null && !canUpdate)
			return;
		if(!canUpdate){
			Transform p = tr;
			while(p = p.parent){
				if(p == platform.transform.parent) {
					//print("this shit actually works");
					return;
				}
			}
			canUpdate = true;
		}
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);

		platform.transform.parent.position = GetPositionInLine(platform.transform.parent.position);
	}
}
