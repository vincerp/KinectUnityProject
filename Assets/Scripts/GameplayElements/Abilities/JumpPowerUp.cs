using UnityEngine;
using System.Collections;

public class JumpPowerUp : BaseAbility {
	
	public float raycastDistance = 1.5f;
	Transform _tr;
	
	void Start(){
		_tr = transform;
		_tr.FindChild("Model").FindChild("BOOTS").gameObject.SetActive(true);
	}
	
	override protected void OnDestroy(){
		base.OnDestroy();
		_tr.FindChild("Model").FindChild("BOOTS").gameObject.SetActive(false);
	}
	
	protected override void UpdateAbility ()
	{
		RaycastHit _hit;
		Player _player;
		if(Physics.Raycast(_tr.position, Vector3.down, out _hit, raycastDistance)){
			if(_hit.collider.tag != "Player") return;
			_player = _hit.collider.GetComponent<Player>();
			_player.ApplyDamage(abilityDamage);
		}
	}
	
}
