using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public InputAxisNames input;
	
	public float speed;									// Horizontal movement speed
	public float jumpStrength;							// Jump power
	public float jumpStrengthMultiplier;				// Should be 1, but it can be changed by other functions
	public float gravity;								// Rigidbodies can have their own gravity, but this makes it all a bit more tweakable
	public float jumpSustainTime;
	
	public float raycastDistance = 0.40f;
	public float raycastRaysize = 0.55f;
	public float canJumpRaycastSize = 0.65f;
	private Vector3 raycastPoint;
	
	private float lockLeft;								// Don't allow left movement
	private float lockRight;							// ... or right movement
	private bool squished;								// Are we alive or what?
	
	public bool enableWallJump = true;
	public float slopeSlideLimit = 50f;
	
	public float maxHealth = 150f;
	public float health = 100f;
	
	public float stepTimeDistance = 0.3f;
	private float timeSinceLastStep = 0f;
	
	[HideInInspector]
	public bool isGrounded;
	public LayerMask ignoreLayers;
	
	//Private stuff
	RaycastHit bottomLeft, bottomMiddle, bottomRight, left, right;
	bool isTouchingRight, isTouchingLeft, canJump;
	bool isJumping = false;
	bool releasedJumpingRightNow = false;
	float jumpAirTime = 0f;
	Vector3 jumpSpeed;
	
	Vector3 groundNormal;
	
	Transform _tr;
	Rigidbody _rb;
	
	void Start () {
		_tr = transform;
		_rb = rigidbody;
		
		raycastPoint = new Vector3(raycastDistance, 0f, 0f);
		tag = "Player";
	}
	
	void Update() {
		if(_rb.IsSleeping()) _rb.WakeUp();
		if(!squished)
		{
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
			
			isTouchingRight = Physics.Raycast (_tr.position, _tr.right, out right, 0.6f  * _tr.localScale.x, ignoreLayers);		
			isTouchingLeft = Physics.Raycast (_tr.position, -_tr.right, out left, 0.6f  * _tr.localScale.x, ignoreLayers);		

			//Starts jumping when we press the button
			if(!isJumping && (Input.GetButtonDown(input.a) || Input.GetButtonDown(input.b))){
				StartJump();
			}
			
			//Ends jumping when we release the button 
			releasedJumpingRightNow = Input.GetButtonUp(input.a) || Input.GetButtonUp(input.b);
			if(releasedJumpingRightNow) {
				jumpAirTime = 0f;
				isJumping = false;
			}
			
		}
		if(jumpAirTime > 0f) jumpAirTime -= Time.deltaTime;
		if(lockLeft > 0) lockLeft -= Time.deltaTime;
		if(lockRight > 0) lockRight -= Time.deltaTime;
		//Always updating movement
		UpdateMovement();
	}
	
	void StartJump(){
		
		if (canJump) {	
			jumpSpeed = new Vector3(0, jumpStrength * jumpStrengthMultiplier,0);
			jumpAirTime = jumpSustainTime;
			isJumping = true;
			SoundManager.instance.PlaySoundAt(audio, "Jump");
		}
		else if(enableWallJump && left.collider)
		{
			jumpSpeed = new Vector3(jumpStrength* jumpStrengthMultiplier*0.3f, jumpStrength * jumpStrengthMultiplier*0.7f,0);
			lockLeft = 0.1f;
			jumpAirTime = jumpSustainTime;
			isJumping = true;
			SoundManager.instance.PlaySoundAt(audio, "Jump");
		}
		else if(enableWallJump && right.collider)
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
		if(releasedJumpingRightNow && (_rb.velocity.y > 0f || jumpAirTime > 0f))
		{
			_rb.velocity = (new Vector3(_rb.velocity.x, _rb.velocity.y/2f, 0f));
		}
		
		//moving left
		if(Input.GetAxis(input.horizontal) < -0.1f && lockLeft <= 0f && !isTouchingLeft)
		{
			isMoving = true;
			_rb.velocity = (new Vector3(-1f * speed * Time.deltaTime, _rb.velocity.y, 0f));
		}
		//moving right
		if(Input.GetAxis(input.horizontal) > 0.1f  && lockRight <= 0f && !isTouchingRight)
		{
			isMoving = true;
			_rb.velocity=(new Vector3(speed * Time.deltaTime, _rb.velocity.y, 0f));
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
	
	#region Health-related
	public void ApplyDamage(float amount){
		health += -amount;
		SoundManager.instance.PlaySoundAt(audio, "Damage");
		if(health < 0f){
			health = 0f;
			Debug.Log(name + " is dead!");
		}
	}
	
	public void Heal(float amount){
		health += amount;
		if(health > maxHealth) health = maxHealth;
	}
	#endregion
	
#if UNITY_EDITOR
	/*
	 * Here we define editor-specific functions that are useful only for debuging.
	 * Keep debug stuff in here to make sure that nothing escapes into our release builds. 
	 */
	
	void OnGUI() {
		GUILayout.Label(""+jumpAirTime);
		string gr = "not grounded";
		if(true){
			if(bottomLeft.collider) gr = ""+ Vector3.Angle(bottomLeft.normal, Vector3.up);
			if(bottomRight.collider) gr = ""+ Vector3.Angle(bottomRight.normal, Vector3.up);
		}
		GUILayout.Label(gr);
		
		Vector3 _pos = Camera.main.WorldToScreenPoint(_tr.position);
		Rect _rect = new Rect(_pos.x-40f, Screen.height-_pos.y-25f, 80f, 20f);
		GUI.Box(_rect, "<3:"+health);
		
	}
	
	void OnDrawGizmos(){
		if(!Application.isPlaying) return;
		Debug.DrawRay (_tr.position + -raycastPoint, Vector3.down*1f, Color.green,0.1f);
		Debug.DrawRay (_tr.position, 	Vector3.down*1f, Color.red,0.1f);
		Debug.DrawRay (_tr.position + raycastPoint, 	Vector3.down*1f, Color.blue,0.1f);
	}
#endif
	
	//not used for now
	public void Squish(Player squishedBy=null) {
		/*
		if(!squished)
		{
			if(squishedBy)
			{
				//ChainJam.AddPoints(squishedBy.playerID,1	);
				//if(ChainJam.GetTotalPoints() >= 10) ChainJam.GameEnd();
			}

			squished =true;
			//do the squish
			StartCoroutine(Respawn());
		}
		*/
	}
	
	//not used for now
	IEnumerator Respawn() {
		yield return new WaitForSeconds(2);
		
		//do the respawn
		squished = false;
	}
	

}

[System.Serializable]
public class InputAxisNames{
	public string horizontal = "Horizontal";
	public string vertical = "Vertical";
	public string a = "Jump", b = "Fire1", x = "Fire2", y = "Fire3";
}