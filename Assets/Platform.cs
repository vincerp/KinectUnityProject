using UnityEngine;
using System.Collections.Generic;
using System;

public class Platform : MonoBehaviour {
	
	public PlatformType pt;
	
	public enum PlatformType 
	{
		
		PT_RAIL,
		PT_PINNED,
		PT_CHAINED,
		PT_RAILPINNED,
		PT_EVERYTHING
		
	}
}