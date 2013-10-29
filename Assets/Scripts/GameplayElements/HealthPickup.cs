using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class HealthPickup : MonoBehaviour {
	
	public float healAmount = 10f;
	public Player onlyForPlayer;
	
	void Start(){
		collider.isTrigger = true;
		gameObject.layer = 2; //so players don't think this trigger is actually ground
	}
	
	void OnTriggerEnter (Collider col) {
		if(col.gameObject.tag != "Player") return;
		
		Player _player = col.GetComponent<Player>();
		
		if(onlyForPlayer && _player != onlyForPlayer){
			Debug.Log("This is not for you!");
			return;
		}
		
		_player.Heal(healAmount);
		Destroy(gameObject);
	}
}
