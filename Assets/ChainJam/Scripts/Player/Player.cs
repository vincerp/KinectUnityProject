using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public InputAxisNames input;
	
	public float speed;									// Horizontal movement speed
	public float jumpStrength;							// Jump power
	public float jumpStrengthMultiplier;				// Should be 1, but it can be changed by other functions
	public float gravity;								// Rigidbodies can have their own gravity, but this makes it all a bit more tweakable
	
	public Transform bottomLeftPoint;					
	public Transform bottomMiddlePoint;
	public Transform bottomRightPoint;					// This are points inside the player object, from which we cast a "ray" to find out if it's touching something
	
	private float lockLeft;								// Don't allow left movement
	private float lockRight;							// ... or right movement
	private Vector3  startScale;						// Remember the start scale, for the squishing animation
	private bool squished;								// Are we alive or what?
	
	private KeyCode keyLeft;
	private KeyCode keyRight;	
	private KeyCode keyUp;

	void Start () {
		startScale = transform.localScale;
	}
	
	void Update() {
		if(!squished)
		{
			// Jump and Squish check

			RaycastHit bottomLeft, bottomMiddle, bottomRight, left, right;
			Physics.Raycast (bottomLeftPoint.position, -bottomLeftPoint.up, out bottomLeft, 0.6f * transform.localScale.y);
			Physics.Raycast (bottomMiddlePoint.position, -bottomMiddlePoint.up , out bottomMiddle, 0.6f  * transform.localScale.y);
			Physics.Raycast (bottomRightPoint.position, -bottomRightPoint.up, out bottomRight, 0.6f  * transform.localScale.y);		
			Physics.Raycast (transform.position, transform.right, out right, 0.6f  * transform.localScale.x);		
			Physics.Raycast (transform.position, -transform.right, out left, 0.6f  * transform.localScale.x);		

			if(bottomLeft.collider && bottomLeft.collider.tag == "Player")
			{
				bottomLeft.collider.transform.GetComponent<Player>().Squish(this);
			}
			if(bottomMiddle.collider && bottomMiddle.collider.tag == "Player")
			{
				bottomMiddle.collider.transform.GetComponent<Player>().Squish(this);
			}
			if(bottomRight.collider && bottomRight.collider.tag == "Player")
			{
				bottomRight.collider.transform.GetComponent<Player>().Squish(this);
			}
			
			bool isJumpingRightNow = Input.GetButtonDown(input.a) || Input.GetButtonDown(input.b);
			if(isJumpingRightNow)
			{
				if (bottomLeft.collider || bottomMiddle.collider || bottomRight.collider) {	
					rigidbody.velocity = (new Vector3(0, jumpStrength * jumpStrengthMultiplier,0));
					//SoundManager.i.Play(SoundManager.i.Jump);
					Debug.Log("normal jump");
				}
				else if(left.collider)
				{
					rigidbody.velocity = (new Vector3(jumpStrength* jumpStrengthMultiplier*0.3f, jumpStrength * jumpStrengthMultiplier*0.7f,0));
					//SoundManager.i.Play(SoundManager.i.Jump);
					lockLeft = 0.1f;
					Debug.Log("side jump left" + jumpStrength + " " + jumpStrengthMultiplier );
				}
				else if(right.collider)
				{
					rigidbody.velocity = (new Vector3(-jumpStrength* jumpStrengthMultiplier*0.3f, jumpStrength* jumpStrengthMultiplier *0.7f,0));
					//SoundManager.i.Play(SoundManager.i.Jump);
					lockRight = 0.1f;
					Debug.Log("side jump right" + jumpStrength + " " + jumpStrengthMultiplier );
				}
			}
			bool releasedJumpingRightNow = Input.GetButtonUp(input.a) || Input.GetButtonUp(input.b);
			if(releasedJumpingRightNow && rigidbody.velocity.y > 0 )
			{
				rigidbody.velocity = (new Vector3(rigidbody.velocity.x, rigidbody.velocity.y / 2,0));
			}
		}
	}
		
	
	// Update is called once per frame
	void FixedUpdate () {
		if(!squished)
		{
			
			if(Input.GetAxis(input.horizontal) < -0.1 && lockLeft <= 0)
			{
				rigidbody.velocity = (new Vector3(-1 * speed * Time.deltaTime,rigidbody.velocity.y,0));
			}
			if(Input.GetAxis(input.horizontal) > 0.1  && lockRight <= 0)
			{
				rigidbody.velocity=(new Vector3(speed * Time.deltaTime,rigidbody.velocity.y,0));
			}
		}

		
		rigidbody.AddForce(new Vector3(0,gravity,0));
		
		if(lockLeft > 0) lockLeft -= Time.deltaTime;
		if(lockRight > 0) lockRight -= Time.deltaTime;
	}
	
	void OnGUI() {
		
		Debug.DrawRay (bottomLeftPoint.position, 	-bottomLeftPoint.up*1f, Color.green,0.1f);
		Debug.DrawRay (bottomMiddlePoint.position, 	-bottomMiddlePoint.up*1f, Color.red,0.1f);
		Debug.DrawRay (bottomRightPoint.position, 	-bottomRightPoint.up*1f, Color.blue,0.1f);
		
	}
	
	public void Squish(Player squishedBy=null) {
		if(!squished)
		{
			if(squishedBy)
			{
				//ChainJam.AddPoints(squishedBy.playerID,1	);
				//if(ChainJam.GetTotalPoints() >= 10) ChainJam.GameEnd();
			}

			SoundManager.i.Play(SoundManager.i.Squish);
			squished =true;
			iTween.ScaleTo(gameObject,iTween.Hash(
				"y",0.1f, 
				"x",1.3f,
				"time",0.2f,
				"easeType", "linear"));
			iTween.MoveTo(gameObject,iTween.Hash(
				"y",transform.position.y - 0.4f,
				"time",0.2f,
				"easeType", "linear"));
			StartCoroutine(Respawn());
		}
	}
	
	IEnumerator Respawn() {
		yield return new WaitForSeconds(2);
		
		iTween.ScaleTo(gameObject,iTween.Hash("scale",startScale,"time", 0.2f,"easeType", "linear"));
		squished = false;
		SoundManager.i.Play(SoundManager.i.Respawn);
		
		transform.position = SpawnPoint.GetRandomSpawnpoint().position;
	}
	

}

[System.Serializable]
public class InputAxisNames{
	public string horizontal = "Horizontal";
	public string vertical = "Vertical";
	public string a = "Jump", b = "Fire1", x = "Fire2", y = "Fire3";
}