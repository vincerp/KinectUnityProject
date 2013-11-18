/**
 * This class finds homes for platforms that have no parents.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformChildServices : MonoBehaviour {
	
	/**
	 * This is the orphan finder
    	 */
	
	int index = 0;
	
	/**
	 * A parenting love story between transforms
	 */
	IEnumerator Start () {
		yield return new WaitForEndOfFrame();
		GameObject go;
		Transform _tr = transform;
		Transform _ptr, _ctr;
		Platform _plat;
		Dictionary<Transform, Transform> parents = new Dictionary<Transform, Transform>();
		
		foreach(Transform _child in _tr){
			if(_child.gameObject.layer != LayerMask.NameToLayer("PlatformP1") && _child.gameObject.layer != LayerMask.NameToLayer("PlatformP2")) continue;
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
			_plat = _ctr.GetComponent<Platform>();
			if(_plat){
				_ptr.position = _ctr.position + _plat.offset;
			} else {
				_ptr.position = _ctr.position;
			}
			//and adopt them
			_ptr.parent = _tr;
			_ctr.parent = _ptr;
			
			_ptr.gameObject.layer = _ctr.gameObject.layer;
			_ctr.name += " is now happy with his new parent";
			
			_ctr.name += index;
			index++;
		}
	}
}
