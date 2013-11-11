using UnityEngine;
using System.Collections;

public class JetpackPowerUp : BaseAbility {
	
	public float jetpackStrength = 250f;
	
	Player _pl;
	Rigidbody _rb;
	AudioSource _as;
	
	void Start () {
		_pl = GetComponent<Player>();
		_rb = rigidbody;
		SetupParticles();
		particles.tag = "DangerousParticles";
		_as = gameObject.AddComponent<AudioSource>();
		_as.loop = true;
		_as.clip = SoundManager.instance.GetSound("Jetpack");
		transform.FindChild("Model").FindChild("JETPACK").gameObject.SetActive(true);
	}
	
	void OnDestroy(){
		transform.FindChild("Model").FindChild("JETPACK").gameObject.SetActive(false);
		if(_as.isPlaying)_as.Stop();
		Destroy(_as);
	}
	
	protected override void UpdateAbility ()
	{
		bool pressingButton = Input.GetButton(_pl.input.a);
		
		particles.enableEmission = pressingButton;
		
		if(!_pl.isGrounded && pressingButton){
			_rb.AddForce(Vector3.up * jetpackStrength);
			if(!_as.isPlaying) _as.Play();
		} else {
			_as.Stop();
		}
	}
}
