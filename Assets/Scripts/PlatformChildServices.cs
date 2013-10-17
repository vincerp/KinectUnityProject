﻿/**
 * This class finds homes for platforms that have no parents.
 */
using UnityEngine;
using System.Collections.Generic;

public class PlatformChildServices : MonoBehaviour {
	
	/**
	 * This is the orphan finder
	 */
	public int platformLayer = 9;
	
	/**
	 * A parenting love story between transforms
	 */
	void Start () {
		GameObject go;
		Transform _tr = transform;
		Transform _ptr, _ctr;
		Dictionary<Transform, Transform> parents = new Dictionary<Transform, Transform>();
		
		foreach(Transform _child in _tr){
			if(_child.gameObject.layer != platformLayer) continue;
			//first we find a parent for the orphan platforms
			go = new GameObject("Platform Parent");
			parents[_child] = go.transform;
			
			// add rigidbody, so iTween will call FixedUpdate instead of Update
			Rigidbody rigid = parents[_child].gameObject.AddComponent<Rigidbody>();
			rigid.isKinematic = true;
		}
		
		foreach(var family in parents){
			_ptr = family.Value;
			_ctr = family.Key;
			//parents go to the orphan child's place
			_ptr.position = _ctr.position;
			//and adopt them
			_ptr.parent = _tr;
			_ctr.parent = _ptr;
			
			_ptr.gameObject.layer = platformLayer;
			_ctr.name += " is now happy with his new parent";
		}
	}
}