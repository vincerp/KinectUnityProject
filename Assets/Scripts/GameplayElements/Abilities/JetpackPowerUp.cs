using UnityEngine;
using System.Collections;

public class JetpackPowerUp : BaseAbility {
	
	public float jetpackStrength = 250f;
	
	Player _pl;
	Rigidbody _rb;
	
	void Start () {
		_pl = GetComponent<Player>();
		_rb = rigidbody;
		SetupParticles();
		particles.tag = "DangerousParticles";
	}
	
	protected override void UpdateAbility ()
	{
		bool pressingButton = Input.GetButton(_pl.input.a);
		
		particles.enableEmission = pressingButton;
		
		if(!_pl.isGrounded && pressingButton){
			_rb.AddForce(Vector3.up * jetpackStrength);
		}
	}
}
