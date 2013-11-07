using UnityEngine;
using System.Collections;

public class DangerousParticleCollider : MonoBehaviour {

	public float particleDamage;
	private ParticleSystem.CollisionEvent[] collisionEvents = new ParticleSystem.CollisionEvent[16];
	
	void Start(){
		tag = "DangerousParticles";
	}
	
	void OnParticleCollision (GameObject other) {
		if(other.tag != "Player") return;
		int safeLength = particleSystem.safeCollisionEventSize;
        if (collisionEvents.Length < safeLength)
            collisionEvents = new ParticleSystem.CollisionEvent[safeLength];
        
        int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
        int i = 0;
        while (i < numCollisionEvents) {
            if (other.rigidbody && other.transform != transform.parent) {
				other.GetComponent<Player>().ApplyDamage(particleDamage);
            }
            i++;
        }
	}
}
