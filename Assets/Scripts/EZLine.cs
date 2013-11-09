using UnityEngine;
using System.Collections;

public class EZLine : MonoBehaviour {
	
	public Vector3 start;
	public Vector3 end;
	public Vector3 point;
	
	private float d1;
	private float d2;
	private float percent;
	private float eulero;
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
		
		d1 = Vector3.Distance(fromWhere, start);
		d2 = Vector3.Distance(fromWhere, end);
		float ls = lineSize;
		eulero = d1*Mathf.Sin(Mathf.Deg2Rad*(90f - Vector3.Angle(fromWhere-start, end-start)));
		percent = eulero/ls;
		return Vector3.Lerp(start, end, percent);
	}
	
	void Update(){
		lastTracked = GetPositionInLine(point);
	}
	
	[ExecuteInEditMode]
	public void OnGUI(){
		GUILayout.Box("percent:" + percent);
		GUILayout.Box("eulero:" + eulero);
		GUILayout.Box("d1:" + d1);
		GUILayout.Box("d2:" + d2);
		
		
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
