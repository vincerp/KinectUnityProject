using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class BonesArea : MonoBehaviour 
{
	public SkeletonPart part;
	public KinectModelControllerV2 skeletonController;
	
	public float movingSpeed = 1f;
	public float rotationSpeed = 1f;
	public float scalingSpeed = 1f;
	
	
	public BonePair bones = new BonePair();
	
	public List<PlatformInfo> platforms = new List<PlatformInfo>();
	public PlatformInfo currentPlatform = null;
	private int nextPlatformIndex = 0;
	
	public float boneScaleFactor = 1f;
	public float boneAngleFactor = 1f;
	public float boneAngleAdjustment = 0f;
	
	public float curveFactor = 0.2f;
	public AnimationCurve boneDistanceFactor;
	public float minimumPlatformScale = 0.3f;
	public float maximumPlatformScale = 20f;
	
	#region Rotation variables
	public float angleSnapFactor = 5;
	public float preciseSnap = 45f;
	public float preciseSnapRange = 8;
	
	public float distanceDeadZone = 1f;
	private float rotateTowardsAngle = 0f;
	#endregion
	
	private Vector3 centralPoint;
	public float centralPointOffset = 0.6f;
	public float magnetMovementSpeed = 10f;
	public float distancePositionMultiplierX = 1f;
	public float distancePositionMultiplierY = 1f;
	
	public BonesInputHelper bonesInput;
//	public bool canSwitchWhileHolding = false;
//	private bool isReleasePlatforms = false;
	
	public bool displayDebugValues = false;
	public bool displayPlatformAngles = true;
	public bool displayInterfaceBox = true;
	public GUISkin clawSkin;
	public Color playerColor;
	public float clawSize = 40f;
	private float _angle;
	
	private Quaternion rotateFrom = Quaternion.identity;
	
	void Start()
	{
		if (skeletonController == null)
			return;
		// auto assignmentof bones game objects code
		switch( part )
		{
		case SkeletonPart.SP_HEAD:
			bones.bone1 = skeletonController.Head.transform;
			// Neck object
			bones.bone2 = skeletonController.Shoulder_Center.transform;
			break;
		case SkeletonPart.SP_HANDS:
			bones.bone1 = skeletonController.Hand_Left.transform;
			bones.bone2 = skeletonController.Hand_Right.transform;
			break;
		case SkeletonPart.SP_HIPS:
			// central hip, right and left hips are positioned in the same place
			// for some reason. The right bone is the child of the hip
			bones.bone1 = skeletonController.Hip_Left.transform.GetChild(0);
			bones.bone2 = skeletonController.Hip_Right.transform.GetChild(0);
			break;
		case SkeletonPart.SP_ELBOWS:
			bones.bone1 = skeletonController.Elbow_Left.transform;
			bones.bone2 = skeletonController.Elbow_Right.transform;
			
			centralPoint = Vector3.Lerp(skeletonController.Spine.transform.position,
				skeletonController.Shoulder_Center.transform.position,
				centralPointOffset);
			break;
		case SkeletonPart.SP_KNEES:
			bones.bone1 = skeletonController.Knee_Left.transform;
			bones.bone2 = skeletonController.Knee_Right.transform;
			break;
		}
	}
	
	void Update()
	{
		
			
		if( bonesInput.CheckRelease() && currentPlatform != null )
		{
//			if( currentPlatform == null )
//				return;
			
			
			currentPlatform.colorState = ColorState.CS_NOTACTIVE;
			currentPlatform.transform.renderer.material.color = Color.white;
			
			currentPlatform = null;
			
			// 2 cases: 
			// - if there is just one platform, then it will become green
			// - if there is more, then there will be reassignment to the same color
			
			if( platforms.Count > 0 )
			{
				platforms[nextPlatformIndex].colorState = ColorState.CS_NEXTTOSELECT;
				platforms[nextPlatformIndex].transform.renderer.material.color = Color.green;
			}
			return;
		}
		
		if( bonesInput.CheckGrab() )
		{
			if( platforms.Count == 0 )
					return;
			
			if( currentPlatform == null )
			{				
				currentPlatform = platforms[nextPlatformIndex];
			try
				{
				currentPlatform.colorState = ColorState.CS_ACTIVE;
				currentPlatform.transform.renderer.material.color = Color.red;	
				}
				catch(Exception c)
				{
					Debug.Log("error appeared");
				}
					
				
				
				if( platforms.Count > 1 )
				{
					nextPlatformIndex = nextPlatformIndex + 1 > platforms.Count - 1 ? 0 : nextPlatformIndex + 1;
					
					platforms[nextPlatformIndex].colorState = ColorState.CS_NEXTTOSELECT;
					platforms[nextPlatformIndex].transform.renderer.material.color = Color.green;
				}
			}
			else
			{
				currentPlatform.colorState = ColorState.CS_NOTACTIVE;
				currentPlatform.transform.renderer.material.color = Color.white;

				currentPlatform = platforms[nextPlatformIndex];
				
				currentPlatform.colorState = ColorState.CS_ACTIVE;
				currentPlatform.transform.renderer.material.color = Color.red;
				
				if( platforms.Count > 1 )
				{
					nextPlatformIndex = nextPlatformIndex + 1 > platforms.Count - 1 ? 0 : nextPlatformIndex + 1;
					
					platforms[nextPlatformIndex].colorState = ColorState.CS_NEXTTOSELECT;
					platforms[nextPlatformIndex].transform.renderer.material.color = Color.green;
				}	
			}

		}
		
		if( bonesInput.CheckSwitchNextPlatform() )
		{
			if( platforms.Count < 2 )
				return;
			
			platforms[nextPlatformIndex].colorState = ColorState.CS_NOTACTIVE;
			platforms[nextPlatformIndex].transform.renderer.material.color = Color.white;
			
			nextPlatformIndex = nextPlatformIndex + 1 > platforms.Count - 1 ? 0 : nextPlatformIndex + 1;
			
			// skip the platform if it is held
			if( currentPlatform != null && platforms[nextPlatformIndex] == currentPlatform )
				nextPlatformIndex = nextPlatformIndex + 1 > platforms.Count - 1 ? 0 : nextPlatformIndex + 1;
			
			platforms[nextPlatformIndex].colorState = ColorState.CS_NEXTTOSELECT;
			platforms[nextPlatformIndex].transform.renderer.material.color = Color.green;
		}

	}
	
	void FixedUpdate()
	{
		Vector3 moveTo = bones.CentralPoint + new Vector3((bones.CentralPoint.x - centralPoint.x)*distancePositionMultiplierX, (bones.CentralPoint.y - centralPoint.y)*distancePositionMultiplierY, 0f);
		
		////Magnet positioning
		transform.position = Vector3.MoveTowards(transform.position,
			moveTo,
			magnetMovementSpeed);
		
		////Magnet rotation
		if(bones.Distance > distanceDeadZone){
			rotateTowardsAngle = (bones.Angle+boneAngleAdjustment)*boneAngleFactor;
			//now we snap the value so it doesn't flicker
			if(Mathf.Abs(rotateTowardsAngle)%preciseSnap < preciseSnapRange || Mathf.Abs(rotateTowardsAngle)%preciseSnap > preciseSnap - preciseSnapRange) {
				//here we snap to precise degree angles
				rotateTowardsAngle = MathUtilities.SnapTo(rotateTowardsAngle, preciseSnap);
			} else {
				//here we snap to less precise angles
				rotateTowardsAngle = MathUtilities.SnapTo(rotateTowardsAngle, angleSnapFactor);
			}
		}
		
		if ( currentPlatform == null ) return;
		
//		if(!isReleasePlatforms) return;
		
		
		
		////Platform Positioning
		if ( currentPlatform == null ) return;
		
		// the platform has been deleted somewhere
		if ( currentPlatform.transform == null )
		{
			currentPlatform = null;
			return;
		}
//		if(!isReleasePlatforms) return;
		
		Platform pl = currentPlatform.transform.GetComponentInChildren<Platform>();
		
		if( pl == null )
		{
			currentPlatform = null;
			return;
		}
			
//		if (pl.pt == Platform.PlatformType.PT_RAIL)
//		{
//		float y = currentPlatform.transform.parent.position.y;
//
//		////Platform Positioning
//		currentPlatform.transform.parent.position = Vector3.MoveTowards( currentPlatform.transform.parent.position, new Vector3 (transform.position.x, y, transform.position.z), Time.deltaTime * movingSpeed);
//		}
		
		
		///Platform Positioning Constraints
		float y = currentPlatform.transform.parent.position.y;
		float x = currentPlatform.transform.parent.position.x;
		Vector3 startingPos = currentPlatform.transform.parent.position;
		
		
		
		switch(pl.pt)
		{
		case Platform.PlatformType.PT_EZLINE:
			Vector3 projectedPos70 = Vector3.MoveTowards( currentPlatform.transform.parent.position, new Vector3 (transform.position.x, transform.position.y, transform.position.z), Time.deltaTime * movingSpeed);
			currentPlatform.transform.parent.position = projectedPos70;
			break;
		case Platform.PlatformType.PT_ORAIL:
		case Platform.PlatformType.PT_ORAILPINNED:
			Vector3 projectedPos0 = Vector3.MoveTowards( currentPlatform.transform.parent.position, new Vector3 (transform.position.x, y, transform.position.z), Time.deltaTime * movingSpeed);
			float railPlatDist0 = Vector3.Distance(pl.rail.transform.position, projectedPos0);
			if(railPlatDist0 > pl.rail.transform.localScale.y/2)
				currentPlatform.transform.parent.position = startingPos;
			else
				currentPlatform.transform.parent.position = projectedPos0;
			break;
			
		case Platform.PlatformType.PT_VRAIL:
		case Platform.PlatformType.PT_VRAILPINNED:
			Vector3 projectedPos1 = Vector3.MoveTowards( currentPlatform.transform.parent.position, new Vector3 (x, transform.position.y, transform.position.z), Time.deltaTime * movingSpeed);
			float railPlatDist1 = Vector3.Distance(pl.rail.transform.position, projectedPos1);
			if(railPlatDist1 > pl.rail.transform.localScale.y)
				currentPlatform.transform.parent.position = startingPos;
			else
				currentPlatform.transform.parent.position = projectedPos1;
			break;
			
			
			
		case Platform.PlatformType.PT_PINNED:
//			currentPlatform.transform.parent = pl.pin.transform;
			currentPlatform.transform.parent.position = Vector3.MoveTowards( currentPlatform.transform.parent.position, new Vector3 (x, y, transform.position.z), Time.deltaTime * movingSpeed);
			break;
			
		case Platform.PlatformType.PT_CHAINED:
			Vector3 projectedPos2 = Vector3.MoveTowards( currentPlatform.transform.parent.position, new Vector3 (transform.position.x, transform.position.y, transform.position.z), Time.deltaTime * movingSpeed);
			float pinPlatDist = Vector3.Distance (pl.pin.transform.position, projectedPos2);
			if (pinPlatDist <= pl.chainLenght)
				currentPlatform.transform.parent.position = projectedPos2;
			else
				currentPlatform.transform.parent.position = startingPos;
			break;

//		case Platform.PlatformType.PT_EVERYTHING:
//			currentPlatform.transform.parent.position = Vector3.MoveTowards( currentPlatform.transform.parent.position, new Vector3 (transform.position.x, transform.position.y, transform.position.z), Time.deltaTime * movingSpeed);
//			
//			float pinPlatDist2 = Vector3.Distance (pl.pin.transform.position, currentPlatform.transform.parent.position);
//			if (pinPlatDist2 <= pl.chainLenght)
//				currentPlatform.transform.parent.position = Vector3.MoveTowards( currentPlatform.transform.parent.position, new Vector3 (transform.position.x, transform.position.y, transform.position.z), Time.deltaTime * movingSpeed);
//			else
//				if (
//				currentPlatform.transform.parent.position = startingPos;
//			break;
			
		}
		
					
		////Platform Rotation
		#region this was moved to be updated independently holding a platform or not
		//if the bones are too close, we will have rotation issues
		//so we will only update rotation if they are not too close
		/*if(bones.Distance > distanceDeadZone){
			rotateTowardsAngle = (bones.Angle+boneAngleAdjustment)*boneAngleFactor;
			//now we snap the value so it doesn't flicker
			if(Mathf.Abs(rotateTowardsAngle)%preciseSnap < preciseSnapRange || Mathf.Abs(rotateTowardsAngle)%preciseSnap > preciseSnap - preciseSnapRange) {
				//here we snap to precise degree angles
				rotateTowardsAngle = MathUtilities.SnapTo(rotateTowardsAngle, preciseSnap);
			} else {
				//here we snap to less precise angles
				rotateTowardsAngle = MathUtilities.SnapTo(rotateTowardsAngle, angleSnapFactor);
			}
		}*/
		#endregion
		//platform rotation happens here
		if (pl.pt != Platform.PlatformType.PT_VRAIL && pl.pt != Platform.PlatformType.PT_ORAIL && pl.pt != Platform.PlatformType.PT_EZLINE)
		{
			rotateFrom = currentPlatform.transform.parent.rotation;
			currentPlatform.transform.parent.rotation = Quaternion.RotateTowards(
				rotateFrom,
				Quaternion.AngleAxis(rotateTowardsAngle + currentPlatform.initialAngle, Vector3.forward),
				Time.deltaTime * rotationSpeed);
		}
		else if(pl.pt != Platform.PlatformType.PT_PINNED)
		{
			
			
		}
		////Platform Scaling
//		currentPlatform.transform.localScale = Vector3.MoveTowards(
//			currentPlatform.transform.localScale, 
//			new Vector3(Mathf.Clamp(currentPlatform.initialScale.x*GetBoneDistance(), minimumPlatformScale, maximumPlatformScale), currentPlatform.initialScale.y, currentPlatform.initialScale.z), 
//			Time.deltaTime * scalingSpeed);
		
	}
	
	float GetBoneDistance(){
		return boneDistanceFactor.Evaluate(bones.Distance)*curveFactor;
	}
	
	// bone area zone
	void OnTriggerEnter( Collider other )
	{
		if( currentPlatform != null && other.transform == currentPlatform.transform )
		{
			platforms.Add(currentPlatform);
			return;
		}
		
		PlatformInfo pinfo = new PlatformInfo();
		
		pinfo.initialAngle = other.transform.rotation.eulerAngles.z;
		pinfo.initialScale = other.transform.localScale;
		
		pinfo.transform = other.transform;
		
		switch( platforms.Count )
		{
		case 0:
			pinfo.colorState = ColorState.CS_NEXTTOSELECT;
			pinfo.transform.renderer.material.color = Color.green;
			
			nextPlatformIndex = 0;
			break;
		case 1:
			if( platforms.Contains( currentPlatform ) )
			{
				pinfo.colorState = ColorState.CS_NEXTTOSELECT;
				pinfo.transform.renderer.material.color = Color.green;
				
				nextPlatformIndex = 1;
			}
			break;
		}
		
		
		platforms.Add(pinfo);
	}
	
	void OnTriggerExit( Collider other )
	{
		// find the element in the list and remove it
		PlatformInfo outPlatform = null;
		
		IEnumerable<PlatformInfo> pls;
		
		
		try{
		pls = (from x in platforms 
							where x.transform.collider == other 
							select x);
		}
				catch(Exception c)
				{
					Debug.Log("error appeared");
				}
			
		outPlatform = pls.First() as PlatformInfo;

			
		int index = platforms.IndexOf( outPlatform );
		
		
		switch( platforms.Count )
		{
		case 0:
			Debug.LogError("List is empty");
			break;
				
		case 1:
			// if it is green
			if( outPlatform != currentPlatform )
			{
				outPlatform.colorState = ColorState.CS_NOTACTIVE;
				outPlatform.transform.renderer.material.color = Color.white;
			}
			// if it is red, do nothing special
			
			nextPlatformIndex = 0;
			
			break;
			
		case 2:
			// if it is green
			if( nextPlatformIndex == index )
			{
				// if there is no red
				if( currentPlatform == null )
				{	
					platforms[nextPlatformIndex].colorState = ColorState.CS_NOTACTIVE;
					platforms[nextPlatformIndex].transform.renderer.material.color = Color.white;
					
					nextPlatformIndex = nextPlatformIndex + 1 > platforms.Count - 1 ? 0 : nextPlatformIndex + 1;
					
					platforms[nextPlatformIndex].colorState = ColorState.CS_NEXTTOSELECT;
					platforms[nextPlatformIndex].transform.renderer.material.color = Color.green;
					
				}
				// if there is red
				else
				{
					// if red is inside magnet, meaning that there are red and green (which goes out)
					if( platforms.Contains(currentPlatform) )
					{
						platforms[nextPlatformIndex].colorState = ColorState.CS_NOTACTIVE;
						platforms[nextPlatformIndex].transform.renderer.material.color = Color.white;
						
						nextPlatformIndex = 0;
					}
					// if red is outside of magnet, meaning that there are white and green (which goes out)
					else
					{
						platforms[nextPlatformIndex].colorState = ColorState.CS_NOTACTIVE;
						platforms[nextPlatformIndex].transform.renderer.material.color = Color.white;
						
						nextPlatformIndex = nextPlatformIndex + 1 > platforms.Count - 1 ? 0 : nextPlatformIndex + 1;
						
						platforms[nextPlatformIndex].colorState = ColorState.CS_NEXTTOSELECT;
						platforms[nextPlatformIndex].transform.renderer.material.color = Color.green;
						
					}
				}
				
				nextPlatformIndex = nextPlatformIndex - 1 < 0 ? 0 : nextPlatformIndex - 1 ;	
			} 
			// white or red and list shifting to left
			else if( index < nextPlatformIndex )
			{
				nextPlatformIndex = nextPlatformIndex - 1 < 0 ? 0 : nextPlatformIndex - 1 ;	
			}
			
			break;
		
		// 3 and more	
		default:
			// if it is green
			if( nextPlatformIndex == index )
			{
				// if there is no red
				if( currentPlatform == null )
				{	
					platforms[nextPlatformIndex].colorState = ColorState.CS_NOTACTIVE;
					platforms[nextPlatformIndex].transform.renderer.material.color = Color.white;
					
					nextPlatformIndex = nextPlatformIndex + 1 > platforms.Count - 1 ? 0 : nextPlatformIndex + 1;
					
					platforms[nextPlatformIndex].colorState = ColorState.CS_NEXTTOSELECT;
					platforms[nextPlatformIndex].transform.renderer.material.color = Color.green;

				}
				// if there is red
				else
				{
					// if red is inside magnet, meaning that there are red, whites and green (which goes out)
					if( platforms.Contains(currentPlatform) )
					{
						platforms[nextPlatformIndex].colorState = ColorState.CS_NOTACTIVE;
						platforms[nextPlatformIndex].transform.renderer.material.color = Color.white;
						
						nextPlatformIndex = nextPlatformIndex + 1 > platforms.Count - 1 ? 0 : nextPlatformIndex + 1;
						
						// skip the platform if it is held
						if( platforms[nextPlatformIndex] == currentPlatform )
							nextPlatformIndex = nextPlatformIndex + 1 > platforms.Count - 1 ? 0 : nextPlatformIndex + 1;
						
						platforms[nextPlatformIndex].colorState = ColorState.CS_NEXTTOSELECT;
						platforms[nextPlatformIndex].transform.renderer.material.color = Color.green;
						
					}
					// if red is outside of magnet, meaning that there are whites and green (which goes out)
					else
					{
						platforms[nextPlatformIndex].colorState = ColorState.CS_NOTACTIVE;
						platforms[nextPlatformIndex].transform.renderer.material.color = Color.white;
						
						nextPlatformIndex = nextPlatformIndex + 1 > platforms.Count - 1 ? 0 : nextPlatformIndex + 1;
						
						platforms[nextPlatformIndex].colorState = ColorState.CS_NEXTTOSELECT;
						platforms[nextPlatformIndex].transform.renderer.material.color = Color.green;

					}
				}
				
				nextPlatformIndex = nextPlatformIndex - 1 < 0 ? 0 : nextPlatformIndex - 1 ;	
			}
			// white or red and list shifting to left
			else if( index < nextPlatformIndex )
			{
				nextPlatformIndex = nextPlatformIndex - 1 < 0 ? 0 : nextPlatformIndex - 1 ;	
			}
			
			break;
		}
		
		
		platforms.Remove(outPlatform);

	}
	
	void OnDrawGizmos(){
		Gizmos.DrawWireSphere(transform.position, (collider as SphereCollider).radius);
		if(!Application.isPlaying) return;
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(bones.CentralPoint, 0.3f);
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(centralPoint, 0.5f);
	}
	
	void OnGUI(){
		//DEBUG GUI: Shows all the info for the magnet
		if(displayDebugValues){
			GUILayout.Box("Info:\nAngle:"+(bones.Angle+boneAngleAdjustment)+
				"\nDistance:"+bones.Distance+
				"\nGetBoneDistance:"+GetBoneDistance()+
				"\nDistance from center:"+Vector3.Distance(centralPoint, bones.CentralPoint)+
				"\n1:"+bones.bone1.position.ToString()+
				"\n2:"+bones.bone2.position.ToString()+
				"\ntrue angle:"+transform.localRotation.z
				);
			
			GUILayout.Box("Angle:\n"+Vector3.Angle(Vector3.zero, Vector3.zero)+"\n"+Vector3.Angle(-Vector3.left, Vector3.right));
		}

		//DEBUG GUI: Shows angles for the platforms
		if(!displayPlatformAngles){
			Rect _pos = new Rect(0f, 0f, 120f, 30f);
			Vector3 _view;
			foreach (var plat in platforms){
				if( plat.transform != null )
				{
					_view = Camera.main.WorldToScreenPoint(plat.transform.position);
					
					_pos.x = _view.x-60f;
					_pos.y = Screen.height-_view.y-15f;
					GUI.color = new Color(0f,0f,0f,0.7f);
					GUI.Label(_pos, "zº:"+plat.transform.rotation.eulerAngles.z);
				}
			}
		}

		
		if(!displayInterfaceBox) return;
 		GUI.skin = clawSkin;
		GUI.color = playerColor;
		
		Vector3 _magPos = Camera.main.WorldToScreenPoint(transform.position);
		_magPos = new Vector3(_magPos.x, Screen.height - _magPos.y, 0f);
		Rect _boxyThing = new Rect(0f, 0f, clawSize, clawSize);
		float _distPercent = GetBoneDistance() / boneDistanceFactor.Evaluate(1000);
		float _minimumDistanceAmount = clawSize;
		float _distAmount = 400f;
		_angle = Mathf.MoveTowardsAngle(_angle, rotateTowardsAngle, rotationSpeed);
		_distAmount *= _distPercent;
		_distAmount += _minimumDistanceAmount;
		_distAmount = 80f; //this is a HACK!
		
		bool isHolding = currentPlatform != null;
		float halfClaw = clawSize/2;
		
		GUIUtility.RotateAroundPivot(-_angle, new Vector2(_magPos.x, _magPos.y));
		_boxyThing.y = _magPos.y-halfClaw;
		_boxyThing.x = _magPos.x-halfClaw;
		GUI.Box(_boxyThing, "", "Target");
		_boxyThing.x = _magPos.x - _distAmount - halfClaw;
		GUI.Toggle(_boxyThing, isHolding, "", "Claw");
		_boxyThing.x = _magPos.x + _distAmount + halfClaw;
		_boxyThing.width = -clawSize;
		GUI.Toggle(_boxyThing, isHolding, "", "Claw");

	}
}

[System.Serializable]
public class BonePair{
	public Transform bone1, bone2;
	public float centralPointOffset = 0.5f;
	
	public Vector3 CentralPoint{
		get{
			Vector3 shit = Vector3.Lerp(bone1.position, bone2.position, centralPointOffset);
			shit = new Vector3(shit.x, shit.y, 0f);
			return shit;
		}
	}
	
	public float Angle{
		get{
			Vector3 a = bone1.position, b = bone2.position;
			
			if (bone1.position.y > bone2.position.y)
			{
				return Vector3.Angle(b-a, Vector3.right);
			}
			else	
				return -Vector3.Angle(b-a, Vector3.right);
			
		}
	}
	
	public float Distance{
		get {
			return Vector3.Distance(bone1.position, bone2.position);
		}
	}
}

public enum ColorState
{
	CS_NOTSELECTED,
	CS_NEXTTOSELECT,
	CS_NOTACTIVE,
	CS_ACTIVE
}

#region Other Support Classes
[System.Serializable]
public class PlatformInfo
{
	public float initialAngle;
	public Vector3 initialScale;
	public ColorState colorState;
	
	public Transform transform;
}

public enum SkeletonPart
{
	SP_HEAD,
	SP_HANDS,
	SP_SHOULDERS,
	SP_HIPS,
	SP_ELBOWS,
	SP_KNEES
}

[System.Serializable]
public class BonesInputHelper{
	public enum InputActionType{
		PRESS,
		HOLD,
		RELEASE
	}
	
	public InputActionType grabAction = InputActionType.PRESS;
	public KeyCode grabKey = KeyCode.Joystick1Button5;
	public InputActionType releaseAction = InputActionType.RELEASE;
	public KeyCode releaseKey = KeyCode.Joystick1Button5;
	public InputActionType switchNextPlatformAction = InputActionType.PRESS;
	public KeyCode switchNextPlatformKey = KeyCode.Joystick1Button4;
	
	public bool CheckGrab(){
		switch(grabAction){
		case InputActionType.PRESS: return Input.GetKeyDown(grabKey);
		case InputActionType.HOLD : return Input.GetKey(grabKey);
		case InputActionType.RELEASE: return Input.GetKeyUp(grabKey);
		}
		return false;
	}
	public bool CheckRelease(){
		switch(releaseAction){
		case InputActionType.PRESS: return Input.GetKeyDown(grabKey);
		case InputActionType.HOLD : return Input.GetKey(grabKey);
		case InputActionType.RELEASE: return Input.GetKeyUp(grabKey);
		}
		return false;
	}
	public bool CheckSwitchNextPlatform(){
		switch(switchNextPlatformAction){
		case InputActionType.PRESS: return Input.GetKeyDown(switchNextPlatformKey);
		case InputActionType.HOLD : return Input.GetKey(switchNextPlatformKey);
		case InputActionType.RELEASE: return Input.GetKeyUp(switchNextPlatformKey);
		}
		return false;
	}
}
#endregion