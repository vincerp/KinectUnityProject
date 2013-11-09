using UnityEngine;
using System.Collections;

public class EZLine {
	
	public Vector3 start;
	public Vector3 end;
	
	public float lineSize{
		get{
			return Vector3.Distance(start, end);
		}
	}
	
	public Vector3 GetPositionInLine(Vector3 fromWhere){
		//TODO: this
		return Vector3.zero;
	}
}
