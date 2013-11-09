using UnityEngine;
using System.Collections.Generic;
using System;

public class Platform : MonoBehaviour {
	
	public PlatformType pt;
	public int chainLenght = 5;
	public int railLenght;
	public Pin pin;
	public Rail rail;
	
	public Vector3 offset;
	
	public void Start(){
		gameObject.layer = 9;
		if(pt == PlatformType.PT_ORAIL ||
			pt == PlatformType.PT_ORAILPINNED ||
			pt == PlatformType.PT_VRAIL ||
			pt == PlatformType.PT_VRAILPINNED ||
			pt == PlatformType.PT_EVERYTHING){
			rail = transform.GetChild(0).GetComponent<Rail>();
			rail.transform.parent = transform.parent;
		} 
	}
	
	public void OnDrawGizmos(){
		if(offset == Vector3.zero || Application.isPlaying) return;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position+offset, 1f);
	}
	
	public enum PlatformType 
	{
		
		PT_VRAIL,
		PT_ORAIL,
		PT_PINNED,
		PT_CHAINED,
		PT_VRAILPINNED,
		PT_ORAILPINNED,
		PT_EVERYTHING
		
	}
}