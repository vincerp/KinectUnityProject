using UnityEngine;
using System.Collections;

public class EZLine : MonoBehaviour {
	
	public Vector3 startOffset;
	public Vector3 endOffset;
	public Vector3 start{
		get{
			return _tr.position + startOffset;
		}
	}
	public Vector3 end{
		get{
			return _tr.position + endOffset;
		}
	}
	
	public float lineSize{
		get{
			return Vector3.Distance(start, end);
		}
	}
	
	private Transform __tr;
	protected Transform _tr{
		get{
			if(__tr == null) __tr = transform;
			return __tr;
		}
	}
	
	public Vector3 GetPositionInLine(Vector3 fromWhere){
		if(Vector3.Angle(fromWhere-start, end-start) > 90f) return start;
		if(Vector3.Angle(fromWhere-end, start-end) > 90f) return end;
		
		float d1 = Vector3.Distance(fromWhere, start);
		float ls = lineSize;
		float eulero = d1*Mathf.Sin(Mathf.Deg2Rad*(90f - Vector3.Angle(fromWhere-start, end-start)));
		float percent = eulero/ls;
		return Vector3.Lerp(start, end, percent);
	}
	
	public virtual void OnDrawGizmos(){
		Gizmos.color = Color.white;
		Gizmos.DrawLine(start, end);
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(start, 0.5f);
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(end, 0.5f);
		Gizmos.color = Color.yellow;
	}
}
