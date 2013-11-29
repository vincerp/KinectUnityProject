using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

	public float test = 1f;
	public float speed = 0.1f;
	public float currentHealth = 1f;
	public float targetHealth = 1f;

	public Renderer bar;

	private Transform tr;

	private void Start(){
		tr = transform;
	}

	public void UpdateHealthPercentage (float percent) {
		targetHealth = percent;
	}

	private void Update(){
		if(targetHealth == currentHealth) return;
		currentHealth = Mathf.MoveTowards(currentHealth, targetHealth, speed*Time.deltaTime);
		bar.material.SetFloat("_Cutoff", 1f-currentHealth);
	}

	[ContextMenu("UpdateShit")]
	public void Test(){
		UpdateHealthPercentage(test);
	}
}
