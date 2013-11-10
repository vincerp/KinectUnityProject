using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EZGrabber : MonoBehaviour {

	public static EZGrabber instance;
	public List<EZLink> links;
	
	void Start () {
		instance = this;
	}
	
	public object GetLinkedItem(string id){
		return links.First(x => x.id == id).obj;
	}
}

[System.Serializable]
public class EZLink{
	public string id;
	public Object obj;
}