using UnityEngine;
using System.Collections;

public class RobotModelController : MonoBehaviour {

	public float rotationSpeed = 40f;
	public float walkSpeed = 3f;
	
	private Player _player;
	private Transform _model;
	
	void Start () {
		_model = transform.Find("Model");
		_player = GetComponent<Player>();
		_model.animation["Jump"].speed = 2f;
		_model.animation["Walk"].speed = walkSpeed;
		PauseGame.onFreezeGame += OnFreezeGameHandler;
	}

	void OnDestroy(){
		PauseGame.onFreezeGame -= OnFreezeGameHandler;
	}

	void OnFreezeGameHandler(bool isFreezing){
		if(isFreezing){
			_model.animation.Stop();
			return;
		}
	}
	
	void Update () {
		if(PauseGame.isGamePaused) return;

		if(_player.isFacingRight){
			_model.localRotation = Quaternion.RotateTowards(_model.localRotation, Quaternion.Euler(0f, 90f, 0f), rotationSpeed);
		} else {
			_model.localRotation = Quaternion.RotateTowards(_model.localRotation, Quaternion.Euler(0f, 270f, 0f), rotationSpeed);
		}
		
		if(!_player.canJump){
			_model.animation.Play("Jump");
		} else {
			if(Mathf.Abs(Input.GetAxis(_player.input.horizontal)) > 0.01f){
				_model.animation.Play("Walk");
			} else {
				_model.animation.Play("Idle");
			}
		}
	}
}
