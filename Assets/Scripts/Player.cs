using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public int playerId = 1;
	public InputAxisNames input;
	
	public float speed;									// Horizontal movement speed
	public float jumpStrength;							// Jump power
	public float jumpStrengthMultiplier;				// Should be 1, but it can be changed by other functions
	public float gravity;								// Rigidbodies can have their own gravity, but this makes it all a bit more tweakable
	public float jumpSustainTime;
	public bool lockJumpSustain = true;
	
	public float raycastDistance = 0.40f;
	public float raycastRaysize = 0.55f;
	public float canJumpRaycastSize = 0.65f;
	private Vector3 raycastPoint;
	
	private float lockLeft;								// Don't allow left movement
	private float lockRight;							// ... or right movement
	
	public bool enableWallJump = true;
	public float slopeSlideLimit = 50f;
	
	public float maxHealth = 150f;
	public float health = 100f;
	private float timeSinceTakenDamage = 0f;
	public float invulnerabilityTime = 0.3f;
	
	public float stepTimeDistance = 0.3f;
	private float timeSinceLastStep = 0f;
	
	[HideInInspector]
	public bool isGrounded;
	[HideInInspector]
	public bool canJump;
	public bool isFacingRight;
	public LayerMask ignoreLayers;

	public float stairStepSize = 1f;

	public Material playerMaterial; 

	//Private stuff
	float colRad;
	Vector3 colCaps1, colCaps2;
	RaycastHit bottomLeft, bottomMiddle, bottomRight, left, right;
	bool isTouchingRight, isTouchingLeft;
	bool isJumping = false;
	bool releasedJumpingRightNow = false;
	float jumpAirTime = 0f;
	Vector3 jumpSpeed;
	
	Vector3 groundNormal;
	
	Transform _tr;
	Rigidbody _rb;
	
	IEnumerator Start () {
		Application.targetFrameRate = 30;
		
		_tr = transform;
		_rb = rigidbody;
		
		raycastPoint = new Vector3(raycastDistance, 0f, 0f);
		tag = "Player";

		PauseGame.onFreezeGame += OnFreezeGameHandler;

		float colSize = (collider as CapsuleCollider).height;
		colCaps1 = Vector3.up * stairStepSize;
		colCaps2 = Vector3.up * (colSize-stairStepSize*2f);
		colRad = (collider as CapsuleCollider).radius;

		yield return new WaitForEndOfFrame();
		SharkManager.instance.RegisterPlayer(_tr);
	}
	
	void Update() {
		if(PauseGame.isGamePaused) return;
		if(_rb.IsSleeping()) _rb.WakeUp();

		if(timeSinceTakenDamage>0f){
			timeSinceTakenDamage -= Time.deltaTime;
		}

		isGrounded = (
			//Ground raycast checks
			Physics.Raycast (_tr.position + -raycastPoint,
				Vector3.down, out bottomLeft,
				raycastRaysize * _tr.localScale.y, ignoreLayers) ||
			Physics.Raycast (_tr.position,
				Vector3.down, out bottomMiddle,
				raycastRaysize  * _tr.localScale.y, ignoreLayers) ||
			Physics.Raycast (_tr.position + raycastPoint,
				Vector3.down, out bottomRight,
				raycastRaysize  * _tr.localScale.y, ignoreLayers)
		);
		canJump = (
			//Ground raycast checks
			Physics.Raycast (_tr.position + -raycastPoint,
				Vector3.down, canJumpRaycastSize * _tr.localScale.y, ignoreLayers) ||
			Physics.Raycast (_tr.position,
				Vector3.down,canJumpRaycastSize  * _tr.localScale.y, ignoreLayers) ||
			Physics.Raycast (_tr.position + raycastPoint,
				Vector3.down, canJumpRaycastSize  * _tr.localScale.y, ignoreLayers)
		);

		Vector3 p = _tr.position;
		isTouchingRight = Physics.CapsuleCast(p + colCaps1, p + colCaps2, colRad, Vector3.right, 1.1f, ignoreLayers);
		//	_rb.SweepTest(Vector3.right, out right, 1.1f);
		//	Physics.Raycast (_tr.position, _tr.right, out right, 1.1f  * _tr.localScale.x, ignoreLayers);		
		isTouchingLeft = Physics.CapsuleCast(p + colCaps1, p + colCaps2, colRad, Vector3.left, 1.1f, ignoreLayers);
		//	_rb.SweepTest(Vector3.left, out left, 1.1f);
		//	Physics.Raycast (_tr.position, -_tr.right, out left, 1.1f  * _tr.localScale.x, ignoreLayers);

		//Starts jumping when we press the button
		if(!isJumping && (Input.GetButtonDown(input.a) || Input.GetButtonDown(input.b))){
			StartJump();
		}
		
		//Ends jumping when we release the button 
		releasedJumpingRightNow = Input.GetButtonUp(input.a) || Input.GetButtonUp(input.b);
		if(releasedJumpingRightNow) {
			if(!lockJumpSustain)jumpAirTime = 0f;
			isJumping = false;
		}
		if(jumpAirTime > 0f) jumpAirTime -= Time.deltaTime;
		if(lockLeft > 0) lockLeft -= Time.deltaTime;
		if(lockRight > 0) lockRight -= Time.deltaTime;
		//Always updating movement
		UpdateMovement();
	}
	
	void StartJump(){
		
		if (canJump || timeSinceTakenDamage > 0f) {	
			jumpSpeed = new Vector3(0, jumpStrength * jumpStrengthMultiplier,0);
			jumpAirTime = jumpSustainTime;
			isJumping = true;
			SoundManager.instance.PlaySoundAt(audio, "Jump");
		}
		else if(enableWallJump && isTouchingLeft)
		{
			jumpSpeed = new Vector3(jumpStrength* jumpStrengthMultiplier*0.3f, jumpStrength * jumpStrengthMultiplier*0.7f,0);
			lockLeft = 0.1f;
			jumpAirTime = jumpSustainTime;
			isJumping = true;
			SoundManager.instance.PlaySoundAt(audio, "Jump");
		}
		else if(enableWallJump && isTouchingRight)
		{
			jumpSpeed = new Vector3(-jumpStrength* jumpStrengthMultiplier*0.3f, jumpStrength* jumpStrengthMultiplier *0.7f,0);
			lockRight = 0.1f;
			jumpAirTime = jumpSustainTime;
			isJumping = true;
			SoundManager.instance.PlaySoundAt(audio, "Jump");
		}
	}
	
	/// <summary>
	/// Updates the movement. Might need to be rewritten when players are able to die.
	/// </summary>
	void UpdateMovement () {
		bool isMoving = false;
		if(jumpAirTime > 0f)
		{
			_rb.velocity = jumpSpeed;
			
		}
		if(!lockJumpSustain && releasedJumpingRightNow && (_rb.velocity.y > 0f || jumpAirTime > 0f))
		{
			_rb.velocity = (new Vector3(_rb.velocity.x, _rb.velocity.y/2f, 0f));
		}
		
		//moving left
		if(Input.GetAxis(input.horizontal) < -0.1f && lockLeft <= 0f && !isTouchingLeft)
		{
			isMoving = true;
			_rb.velocity = (new Vector3(-1f * speed * Time.deltaTime, _rb.velocity.y, 0f));
			isFacingRight = false;
		}
		//moving right
		if(Input.GetAxis(input.horizontal) > 0.1f  && lockRight <= 0f && !isTouchingRight)
		{
			isMoving = true;
			_rb.velocity=(new Vector3(speed * Time.deltaTime, _rb.velocity.y, 0f));
			isFacingRight = true;
		}
		
		
		////Antislope and Gravity
		if(bottomLeft.collider && Vector3.Angle(bottomLeft.normal, Vector3.up) < slopeSlideLimit){
			//check angle and stop slope sliding
			_rb.velocity = new Vector3(_rb.velocity.x, Mathf.Clamp(_rb.velocity.y, 0f, Mathf.Infinity), 0f);
		} else if(bottomRight.collider && Vector3.Angle(bottomRight.normal, Vector3.up) < slopeSlideLimit){
			//check angle and stop slope sliding
			_rb.velocity = new Vector3(_rb.velocity.x, Mathf.Clamp(_rb.velocity.y, 0f, Mathf.Infinity), 0f);
		} else if (bottomMiddle.collider){
			//if something collides here, we stop movement
			_rb.velocity = new Vector3(_rb.velocity.x, Mathf.Clamp(_rb.velocity.y, 0f, Mathf.Infinity), 0f);
		} else {
			//in any other case, gravity!
			_rb.AddForce(new Vector3(0f, gravity, 0f));
		}
		
		//steps audio
		if(isMoving && canJump){
			timeSinceLastStep -= Time.deltaTime;
			if(timeSinceLastStep <= 0f) {
				timeSinceLastStep = stepTimeDistance;
				SoundManager.instance.PlaySoundAt(audio, "Step");
			}
		}
		if(!canJump){
			timeSinceLastStep = 0f;
		}
	}

	public void OnFreezeGameHandler(bool isFreezing){
		if(isFreezing){
			_rb.Sleep();
		}
	}

	private void OnDestroy(){
		PauseGame.onFreezeGame -= OnFreezeGameHandler;
	}
	
	#region Health-related
	public void ApplyDamage(float amount){
		if(timeSinceTakenDamage > 0f) return;
		health += -amount;
		SoundManager.instance.PlaySoundAt(audio, "Damage");
		timeSinceTakenDamage = invulnerabilityTime;

		playerMaterial.SetInt("_AnimEnabled", 1);
		StartCoroutine(StopDamageAnimation(1f));

		if(health <= 0f){
			health = 0f;
			Debug.Log(name + " is dead!");
			if(GameOverScreen.instance)GameOverScreen.instance.PlayerDied(playerId);
		}
	}

	private IEnumerator StopDamageAnimation(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		playerMaterial.SetInt("_AnimEnabled", 0);
	}
	
	public void Heal(float amount){
		health += amount;
		if(health > maxHealth) health = maxHealth;
	}
	#endregion
	
	public Rect healthBarRect = new Rect(0f, 0f, 30f, 5f);
	public GUISkin healthBarSkin;
	
	void OnGUI(){
		Vector3 pos = Camera.main.WorldToScreenPoint(_tr.position);
		
		Rect copy = healthBarRect;
		copy.x = pos.x-(healthBarRect.width/2f);
		copy.y += Screen.height-pos.y;
		
		GUI.skin = healthBarSkin;
		
		GUI.color = new Color(0.8f, 0f, 0f, 0.7f);
		GUI.Box(copy, "");
		copy.width *= (health/maxHealth);
		if(copy.width<=0f)copy.width = 0f;
		GUI.color = new Color(0f, 0.8f, 0f, 0.9f);
		GUI.Box(copy, "");
	}
	
#if UNITY_EDITOR
	/*
	 * Here we define editor-specific functions that are useful only for debuging.
	 * Keep debug stuff in here to make sure that nothing escapes into our release builds. 
	 */
	
	void OnDrawGizmos(){
		if(!Application.isPlaying) return;
		Vector3 p = _tr.position;
		Debug.DrawRay (p + -raycastPoint, Vector3.down*1f, Color.green,0.1f);
		Debug.DrawRay (p, 	Vector3.down*1f, Color.red,0.1f);
		Debug.DrawRay (p + raycastPoint, 	Vector3.down*1f, Color.blue,0.1f);

		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(p + colCaps1, colRad);
		Gizmos.DrawWireSphere(p + colCaps2, colRad);
	}
#endif

}

[System.Serializable]
public class InputAxisNames{
	public string horizontal = "Horizontal";
	public string vertical = "Vertical";
	public string a = "Jump", b = "Fire1", x = "Fire2", y = "Fire3";
}