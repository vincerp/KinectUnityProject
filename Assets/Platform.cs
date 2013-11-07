using UnityEngine;
using System.Collections.Generic;
using System;

public class Platform : MonoBehaviour {
	
	public PlatformType pt;
	public int chainLenght = 5;
	public int railLenght;
	public Pin pin;
	
	
	public Vector3 offset;
	
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