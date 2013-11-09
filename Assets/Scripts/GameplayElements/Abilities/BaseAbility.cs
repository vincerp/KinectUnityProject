using UnityEngine;
using System.Collections;

public abstract class BaseAbility : MonoBehaviour {

	public float abilityDuration = 5f;
	public float abilityTimer = 0f;
	public GameObject particlesPrefab;
	
	protected ParticleSystem particles;
	
	protected abstract void UpdateAbility();
	
	private void Update () {
		abilityTimer += Time.deltaTime;
		UpdateAbility();
		//Add also something here to warn that the ability is running out of power
		if(abilityTimer >= abilityDuration){
			Debug.Log("Player lost ability.");
			if(particles)Destroy(particles.gameObject);
			Destroy(this);
		}
	}
	
	protected void SetupParticles(){
		GameObject go = Instantiate(particlesPrefab) as GameObject;
		particles = go.particleSystem;
		go.transform.parent = transform;
		go.transform.localPosition = Vector3.zero;
	}
	
	public static void BestowAbility(AbilitySettings abilitySettings, Player toPlayer){
		BaseAbility _ability = toPlayer.GetComponent(abilitySettings.scriptName) as BaseAbility;
		SoundManager.instance.PlaySoundAt(toPlayer.audio, abilitySettings.pickupSound);
		if(_ability == null){
			Debug.Log("Adding ability to player.");
			_ability = toPlayer.gameObject.AddComponent(abilitySettings.scriptName) as BaseAbility;
			_ability.abilityDuration = abilitySettings.duration;
			_ability.particlesPrefab = abilitySettings.particles;
		} else {
			Debug.Log("Player had the ability already. Restoring duration.");
			_ability.abilityTimer = 0f;
		}
	}
}