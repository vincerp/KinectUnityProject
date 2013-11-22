using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PlatformMomentum : MonoBehaviour {
	
	
	public Transform attachedTo; //right now this does nothing other than check if we are attached to or not
	public float velocityMultiplier = 1;
	
	public List<Vector3> recordedPlayerPos;
	public int frameTrackingAmount = 5;
	//private System.Nullable<Vector3> lastPlayerPosition;
	private Transform _transform;
	private Quaternion _rotation;
	
	void Awake () {
		_transform = transform;
		_rotation = _transform.rotation;
		recordedPlayerPos = new List<Vector3>();
	}
	
	void FixedUpdate(){
		//here we update the player position to understand his movement to apply momentum
		if(attachedTo == null){
			//if it is not attached, we do not need to keep track of position
			if(recordedPlayerPos != null) recordedPlayerPos.Clear();
			return;
		}
		recordedPlayerPos.Add(attachedTo.position/*_transform.position*/);
		while(recordedPlayerPos.Count > frameTrackingAmount) recordedPlayerPos.RemoveAt(0);

		if(recordedPlayerPos.Count<2)return;
		Vector3 ma, moveAmount = recordedPlayerPos[recordedPlayerPos.Count-1]-recordedPlayerPos[recordedPlayerPos.Count-2];
		ma = new Vector3(moveAmount.x, 0f, 0f);
		if(moveAmount.y > 0f) rigidbody.AddForce(GetMomentumVelocityVector() * velocityMultiplier);

		_transform.Translate(ma);
		//rigidbody.AddForce(GetMomentumVelocityVector() * velocityMultiplier);
		_transform.rotation = _rotation;
	}
	
	void OnCollisionEnter (Collision col) {
		if(col.gameObject.layer != LayerValues.PLATFORM_LAYER_1 && col.gameObject.layer != LayerValues.PLATFORM_LAYER_2) return;
		
		attachedTo = col.transform;
		print("Attached to " + attachedTo.name);
	}
	
	void OnCollisionExit (Collision col) {
		if(col.gameObject.layer != LayerValues.PLATFORM_LAYER_1 && col.gameObject.layer != LayerValues.PLATFORM_LAYER_2) return;
		
		if(attachedTo){
			print("Dettached from " + attachedTo.name);
			attachedTo = null;
		}
	}
	
	Vector3 GetMomentumVelocityVector(){
		
		Vector3 result = Vector3.zero;
		
		if( recordedPlayerPos.Count > 0 )
		{
			//right now this is a pretty basic
			//we should consider all the points from our records to get a more accurate momentum
			Vector3 initialPositionConsidered = recordedPlayerPos[0];
			Vector3 lastPositionConsidered = recordedPlayerPos[recordedPlayerPos.Count-1];
			
			Vector3 direction = lastPositionConsidered - initialPositionConsidered;
			
			//taking away all movement that pulls the player below because o momentum
			//might not be good if the player is somehow attached to the bottom of a platform
			if(direction.y < 0) direction = new Vector3(direction.x, 0f, 0f);
			//otherwise, cancel z movement anyway
			else direction = new Vector3(direction.x, direction.y, 0f);
			
			result = direction;
			
			print("Vector force is " + direction.ToString());
		}
		
		return result;
	}

	/// <summary>
	/// Not being used.
	/// </summary>
	/// <returns>The momentum velocity.</returns>
	float GetMomentumVelocity(){
		
		float result = 0f;
		
		if( recordedPlayerPos.Count > 0 ){
			
			Vector3 initialPositionConsidered = recordedPlayerPos[0];
			Vector3 lastPositionConsidered = recordedPlayerPos[recordedPlayerPos.Count-1];
			result = (initialPositionConsidered - lastPositionConsidered).magnitude;
		}
		
		return result;
	}
}
