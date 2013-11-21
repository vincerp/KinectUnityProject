using UnityEngine;
using System.Collections;

public abstract class BaseAbility : MonoBehaviour {

	public float abilityDuration = 5f;
	public float abilityTimer = 0f;
	public float abilityDamage = 0f;
	public GameObject particlesPrefab;
	
	protected ParticleSystem particles;
	protected GameObject introAnimation;
	
	protected abstract void UpdateAbility();
	
	private void Update () {
		if(PauseGame.isGamePaused) return;
		abilityTimer += Time.deltaTime;
		UpdateAbility();
		//Add also something here to warn that the ability is running out of power
		if(abilityTimer >= abilityDuration){
			Debug.Log("Player lost ability.");
			if(particles)Destroy(particles.gameObject);
			Destroy(this);
		}
	}

	virtual protected void OnDestroy(){
		PauseGame.onFreezeGame -= OnFreezeGameHandler;
	}
	
	protected void SetupParticles(){
		GameObject go = Instantiate(particlesPrefab) as GameObject;
		particles = go.particleSystem;
		go.transform.parent = transform;
		go.transform.localPosition = Vector3.zero;
	}

	virtual protected void OnFreezeGameHandler(bool isFreezing){
		if(isFreezing){
			if(particles)particles.Stop(true);
			return;
		}
		if(particles)particles.Play(true);
	}
	
	public static void BestowAbility(AbilitySettings abilitySettings, Player toPlayer){
		BaseAbility _ability = toPlayer.GetComponent(abilitySettings.scriptName) as BaseAbility;
		SoundManager.instance.PlaySoundAt(toPlayer.audio, abilitySettings.pickupSound);
		if(_ability == null){
			Debug.Log("Adding ability to player.");
			_ability = toPlayer.gameObject.AddComponent(abilitySettings.scriptName) as BaseAbility;
			_ability.abilityDuration = abilitySettings.duration;
			_ability.particlesPrefab = abilitySettings.particles;
			_ability.introAnimation = abilitySettings.animation;
			_ability.abilityDamage = abilitySettings.abilityDamage;
			PauseGame.onFreezeGame += _ability.OnFreezeGameHandler;
			_ability.StartCoroutine("PlayIntroAnim");
		} else {
			Debug.Log("Player had the ability already. Restoring duration.");
			_ability.abilityTimer = 0f;
		}
	}



	protected IEnumerator PlayIntroAnim(){
		yield return new WaitForEndOfFrame();
		GameObject go = Instantiate(introAnimation) as GameObject;
		PauseGame.FreezeGame();
		go.transform.position = new Vector3(0f, 0f, -6f);
		yield return new WaitForSeconds(3f);
		PauseGame.DefreezeGame();
		Destroy(go);
	}
}