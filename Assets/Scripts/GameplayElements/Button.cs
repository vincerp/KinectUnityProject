using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Button : MonoBehaviour {
	
	public delegate void ButtonEvent();
	public event ButtonEvent ButtonChanged;
	
	public enum OnlyToPlayer{
		BOTH,
		PLAYER1,
		PLAYER2
	}
	public OnlyToPlayer onlyToPlayer = OnlyToPlayer.BOTH; 
	
	private GameObject _onlyToPlayer;
	public bool not = false;
	public bool activateOnlyOnce = true;
	private bool hasActivated = false;
	public bool triggerScriptsInThisObject = false;
	public bool blinkWhileInnactive = true;
	
	private int nOfPlayers = 0;
	private int ignoreRaycastLayer = 2;
	
	private bool wasValue;
	public bool isActive{
		get{
			bool val = nOfPlayers>0;
			if(not) val = !val;
			return val;
		}
	}
	
	private Material _mat;
	private Color _originalColor;
	public Color blinkColor = Color.white;
	
	void Start () {
		collider.isTrigger = true;
		gameObject.layer = ignoreRaycastLayer;
		if(audio == null) gameObject.AddComponent<AudioSource>();
		_mat = transform.FindChild("BUTTON").renderer.material;
		_originalColor = _mat.color;
		
		if(onlyToPlayer != OnlyToPlayer.BOTH){
			string find = (onlyToPlayer == OnlyToPlayer.PLAYER1)?"Player":"Player2";
			_onlyToPlayer = GameObject.Find(find);
		}
		animation.Play("off");
	}
	
	void Update(){
		if(!blinkWhileInnactive || hasActivated) return;
		Color _blinkColor = Color.Lerp(_originalColor, blinkColor, Mathf.Abs(Mathf.Sin(Time.time)));
		_mat.color = _blinkColor;
	}
	
	void OnTriggerEnter(Collider col){
		if(hasActivated || col.tag != "Player") return;
//		(col.gameObject.GetComponent<Player>().playerId == (int)onlyToPlayer)
		
		if(_onlyToPlayer && col.gameObject != _onlyToPlayer)
		{
			SoundManager.instance.PlaySoundAt(audio, "WrongButton");
			return;
		}

		//play sound here
		nOfPlayers ++;
		if(nOfPlayers==1) { //this is such a shitty piece of code
			if(ButtonChanged != null)ButtonChanged();
			if(activateOnlyOnce) hasActivated = true;
			if(triggerScriptsInThisObject) SendMessage("Trigger", isActive);
			animation.Play("on");
			SoundManager.instance.PlaySoundAt(audio, "On");
		}
	}
	
	void OnTriggerExit(Collider col){
		if(hasActivated || col.tag != "Player" || (_onlyToPlayer && col.gameObject != _onlyToPlayer)) return;
		
		//play sound here
		nOfPlayers --;
		if(nOfPlayers == 0) { //this is such a shitty piece of code
			if(ButtonChanged != null)ButtonChanged();
			if(triggerScriptsInThisObject) SendMessage("Trigger", isActive);
			animation.Play("off");
			SoundManager.instance.PlaySoundAt(audio, "Off");
		}
	}
}
