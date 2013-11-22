using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class AbilityPickup : MonoBehaviour {
	
	public AbilitySettings ability;
	public Player onlyForPlayer;
	
	void Start(){
		collider.isTrigger = true;
		gameObject.layer = 2; //so players don't think this trigger is actually ground
		Spin sp = gameObject.AddComponent<Spin>();
		sp.speed = new Vector3(0f, 3f, 0f);
	}
	
	void OnTriggerEnter (Collider col) {
		if(col.gameObject.tag != "Player") return;
		
		Player _player = col.GetComponent<Player>();
		
		if(onlyForPlayer && _player != onlyForPlayer){
			Debug.Log("This is not for you!");
			return;
		}
		
		BaseAbility.BestowAbility(ability, _player);
		Destroy(gameObject);
	}
}
