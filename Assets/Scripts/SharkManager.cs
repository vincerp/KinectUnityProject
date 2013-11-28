using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SharkManager : MonoBehaviour {

	public static SharkManager instance;

	public enum SharkState{
		SLEEPING,
		LOOK_AROUND,
		START_SWIM,
		SWIM,
		START_HIDE,
		HIDE,
		ATTACK
	}
	public SharkState currentState;

	public float sharkAwarenessZone = 12f;

	public GameObject lookAroundParticles;
	public Vector3 lookAroundPosition = new Vector3(0f, 1.5f, -2f);
	private Quaternion lookAroundRotation = Quaternion.Euler(0f, 180f, 0f);
	
	public float swimSpeed =1f;
	public Vector3 swimPosition = new Vector3(0f, 2.8f, 5f);
	public float hideSpeed =5f;
	public Vector3 hidePosition = new Vector3(0f, -2f, 5f);
	private Quaternion swimRotation = Quaternion.identity;
	
	public GameObject attackParticles;
	public Vector3 attackPositionOffset = new Vector3(0f, 1.5f, -2f);
	private Quaternion attackRotation = Quaternion.identity;

	private Transform tr;

	private List<Transform> players;

	public void Start(){
		instance = this;
		tr = transform;

		if(!audio)gameObject.AddComponent<AudioSource>();

		currentState = SharkState.SLEEPING;
		tr.position = hidePosition;
		players = new List<Transform>();
		animation.Stop();
		PauseGame.onFreezeGame += OnFreezeGameHandler;
	}
	
	private void Update(){
		if(currentState == SharkState.SLEEPING || currentState == SharkState.LOOK_AROUND || currentState == SharkState.ATTACK) return;
		float lower = 100f;
		foreach(Transform tr in players){
			if(tr.position.y < lower) lower = tr.position.y;
		}
		if((currentState == SharkState.HIDE || currentState == SharkState.START_HIDE) && lower <= sharkAwarenessZone){
			ChangeSharkState(SharkState.START_SWIM);
		} else if ((currentState == SharkState.SWIM || currentState == SharkState.START_SWIM) && lower > sharkAwarenessZone){
			ChangeSharkState(SharkState.HIDE);
		}
	} 

	public void RegisterPlayer(Transform playerTransform){
		if(!players.Contains(playerTransform))players.Add(playerTransform);
	}

	public void ChangeSharkState(SharkState to, GameObject reference = null){
		switch(to){
		case SharkState.LOOK_AROUND:
			StartCoroutine("LookAround");
			break;
		case SharkState.ATTACK:
			StartCoroutine("Attack", reference);
			break;
		case SharkState.SWIM:
		case SharkState.START_SWIM:
			StartCoroutine("StartSwim");
			break;
		case SharkState.HIDE:
		case SharkState.START_HIDE:
			StartCoroutine("StopSwim");
			break;
		}
	}

	private float savedTime;
	private bool wasPlaying;
	public void OnFreezeGameHandler(bool isFreezing){
		if(isFreezing){
			savedTime = animation[animation.clip.name].time;
			wasPlaying = animation.isPlaying;
			animation.Stop();
		} else {
			if(wasPlaying)animation.Play();
			animation[animation.clip.name].time = savedTime;
		}
	}

	private void OnDestroy(){
		PauseGame.onFreezeGame -= OnFreezeGameHandler;
		players = null;
	}

	private IEnumerator StartSwim(){
		if(currentState == SharkState.START_HIDE){
			StopCoroutine("StopSwim");
		}
		currentState = SharkState.START_SWIM;
		animation.Play("Swim Around");
		tr.rotation = swimRotation;

		while(tr.position != swimPosition){
			tr.position = Vector3.MoveTowards(tr.position, swimPosition, swimSpeed*Time.deltaTime);
			yield return null;
		}
		currentState = SharkState.SWIM;
	}

	private IEnumerator StopSwim(){
		if(currentState == SharkState.START_SWIM){
			StopCoroutine("StartSwim");
		}
		currentState = SharkState.START_HIDE;
		while(tr.position != hidePosition){
			tr.position = Vector3.MoveTowards(tr.position, hidePosition, hideSpeed*Time.deltaTime);
			yield return null;
		}
		animation.Stop();
		currentState = SharkState.HIDE;
	}

	private IEnumerator LookAround(){
		currentState = SharkState.LOOK_AROUND;
		tr.position = lookAroundPosition;
		tr.rotation = lookAroundRotation;
		GameObject part = Instantiate(lookAroundParticles) as GameObject;
		//TODO: play sound here!
		SoundManager.instance.PlaySoundAt(audio, "SharkIntro");
		animation.Play("Look Around");
		part.transform.position = lookAroundPosition;
		yield return new WaitForSeconds(8f);
		Destroy(part);
		currentState = SharkState.HIDE;
		tr.position = hidePosition;
	}
	
	private IEnumerator Attack(GameObject who){
		if(currentState == SharkState.SWIM || currentState == SharkState.START_SWIM) yield return StopSwim();

		Vector3 attackPos = new Vector3(who.transform.position.x, 0f, 0f) + attackPositionOffset;
		
		//TODO: play sound here!
		SoundManager.instance.PlaySoundAt(audio, "SharkAttack");
		tr.position = attackPos;
		tr.rotation = attackRotation;
		if(attackPos.x > 0f) tr.Rotate(0f, 180f, 0f);
		animation.Play("Attack");
		GameObject part = Instantiate(attackParticles) as GameObject;
		part.transform.position = attackPos;

		yield return new WaitForSeconds(0.5f);
		who.SetActive(false);
	}
}
