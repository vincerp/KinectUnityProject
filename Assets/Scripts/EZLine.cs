using UnityEngine;
using System.Collections;

public class EZLine : MonoBehaviour {
	
	public Vector3 start;
	public Vector3 end;
	public Vector3 point;
	
	private float percent;
	private Vector3 lastTracked;
	
	public float lineSize{
		get{
			return Vector3.Distance(start, end);
		}
	}
	
	public Vector3 GetPositionInLine(Vector3 fromWhere){
		//TODO: this
		if(Vector3.Angle(fromWhere-start, end-start) > 90f) return start;
		if(Vector3.Angle(fromWhere-end, start-end) > 90f) return end;
		
		float d1 = Vector3.Distance(fromWhere, start);
		float d2 = Vector3.Distance(fromWhere, end);
		float ls = lineSize;
		float eucl = d1*d1/ls;
		percent = eucl/ls;
		return Vector3.Lerp(start, end, eucl/ls);
	}
	
	void Update(){
		lastTracked = GetPositionInLine(point);
	}
	
	[ExecuteInEditMode]
	public void OnGUI(){
		GUILayout.Box("percent:" + percent);
		if(GUILayout.Button("track")){
			lastTracked = GetPositionInLine(point);
		}
	}
	
	public void OnDrawGizmos(){
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(point, 0.5f);
		Gizmos.DrawLine(start, point);
		Gizmos.DrawLine(end, point);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(start, end);
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(start, 0.5f);
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(end, 0.5f);
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(lastTracked, 0.5f);
	}
}
