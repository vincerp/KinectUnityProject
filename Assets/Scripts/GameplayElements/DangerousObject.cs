﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class DangerousObject : MonoBehaviour {

	public float damageDealt;
	public bool knockbackWhenTouched = true;
	public float knockbackStrength = 50f;
	public float knockbackXDifference = 3f;

	private float stayDamageCounter = 0.1f;
	private float lastCausedDamage = 0f;
	
	private void OnCollisionStay (Collision col) {
		if(col.gameObject.tag != "Player") return;
		
		Player _pl = col.gameObject.GetComponent<Player>();
		if(!knockbackWhenTouched && Time.time < lastCausedDamage + stayDamageCounter) return;
		
		Debug.Log("Ouch!");
		ApplyDamage(_pl);
		
		if(!knockbackWhenTouched) return;
		Rigidbody _rb = col.rigidbody;
		Vector3 _force = -col.contacts[0].normal*knockbackStrength;
		_force = new Vector3(_force.x/knockbackXDifference, _force.y, 0f);
		_rb.AddForce(_force);
		lastCausedDamage = Time.time;
	}

	protected virtual void ApplyDamage(Player to){
		to.ApplyDamage(damageDealt);
	}
	
	private void OnCollisionExit(Collision col){
		if(col.gameObject.tag != "Player") return;
		lastCausedDamage = 0f;
	}
}
