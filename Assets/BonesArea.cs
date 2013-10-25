using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
	private PlatformInfo currentPlatform = null;
	
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
	public float magnetMovementSpeed = 10f;
	public float distanceXPositionMultiplier = 3f;
	public float distanceYPositionMultiplier = 3f;
	
	
	public KeyCode releasePlatformsKeycode = KeyCode.Joystick1Button5;
	public KeyCode switchPlatformKeycode = KeyCode.Joystick1Button4;
	public bool canSwitchWhileHolding = false;
	private bool isReleasePlatforms = false;
	
	public bool displayPlatformAngles = true;
	
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
				0.6f);
			break;
		case SkeletonPart.SP_KNEES:
			bones.bone1 = skeletonController.Knee_Left.transform;
			bones.bone2 = skeletonController.Knee_Right.transform;
			break;
		}
	}
	
	void Update()
	{
		if( currentPlatform == null )
			return;
		
		rotateFrom = currentPlatform.transform.parent.rotation;
		
		isReleasePlatforms = Input.GetKey(releasePlatformsKeycode);
		if(isReleasePlatforms)
		{
			if( currentPlatform.colorState != ColorState.CS_ACTIVE )
			{
				currentPlatform.colorState = ColorState.CS_ACTIVE;
				currentPlatform.transform.renderer.material.color = new Color(1f, 0f, 0f, 1f);
			}
		}
		else
		{
			if( currentPlatform.colorState != ColorState.CS_NOTACTIVE )
			{
				currentPlatform.colorState = ColorState.CS_NOTACTIVE;
				currentPlatform.transform.renderer.material.color = new Color(1f, 0f, 0.7f, 1f);
			}
		}
		
		
		
		if( Input.GetKeyDown( switchPlatformKeycode ) )
		{
			int index, nextIndex, secondnext;
			switch( platforms.Count )
			{
			case 0:
			case 1:		
				break;
			case 2:
				index = platforms.IndexOf( currentPlatform );
				nextIndex = index + 1 > platforms.Count - 1 ? 0 : index + 1;
				
				if( !isReleasePlatforms )
					return;
				
				platforms[index].colorState = ColorState.CS_NOTACTIVE;
				platforms[index].transform.renderer.material.color = new Color(1f, 0f, 0.7f, 1f);

				platforms[nextIndex].colorState = ColorState.CS_NEXTTOSELECT;
				platforms[nextIndex].transform.renderer.material.color = Color.green;
				break;
				
			default:
				index = platforms.IndexOf( currentPlatform );
				nextIndex = index + 1 > platforms.Count - 1 ? 0 : index + 1;
				secondnext = nextIndex + 1 > platforms.Count - 1 ? 0 : nextIndex + 1;
				
				if( !isReleasePlatforms )
				{
					platforms[index].colorState = ColorState.CS_NOTSELECTED;
					platforms[index].transform.renderer.material.color = Color.white;
					
					platforms[nextIndex].colorState = ColorState.CS_NOTACTIVE;
					platforms[nextIndex].transform.renderer.material.color = new Color(1f, 0f, 0.7f, 1f);
					
					platforms[secondnext].colorState = ColorState.CS_NEXTTOSELECT;
					platforms[secondnext].transform.renderer.material.color = Color.green;
					
					currentPlatform = platforms[nextIndex];
				}
				else
				{
					if( !canSwitchWhileHolding )
						return;
					
					platforms[nextIndex].colorState = ColorState.CS_NOTSELECTED;
					platforms[nextIndex].transform.renderer.material.color = Color.white;
					
					platforms[secondnext].colorState = ColorState.CS_NEXTTOSELECT;
					platforms[secondnext].transform.renderer.material.color = Color.green;
					
					//swapping
					PlatformInfo temp = platforms[nextIndex];
					platforms[nextIndex] = platforms[secondnext];
					platforms[secondnext] = temp;	
				}
				break;
			}
		}
	}
	
	void FixedUpdate()
	{
		////Magnet positioning
		Vector3 moveTo = (bones.CentralPoint - centralPoint);
		moveTo = new Vector3(moveTo.x*distanceXPositionMultiplier, moveTo.y*distanceYPositionMultiplier, 0f);
		moveTo += bones.CentralPoint;
		
		transform.position = Vector3.MoveTowards(transform.position,
			moveTo,
			magnetMovementSpeed);
		
		//Platform rotation getting
		//if the bones are too close, we will have rotation issues
		//so we will only update rotation if they are not too close
		if(bones.Distance > distanceDeadZone){
			rotateTowardsAngle = (bones.Angle+boneAngleAdjustment)*boneAngleFactor;
			//now we snap the value so it doesn't flicker
			if(rotateTowardsAngle%preciseSnap < preciseSnapRange || rotateTowardsAngle%preciseSnap > preciseSnap - preciseSnapRange) {
				//here we snap to precise degree angles
				rotateTowardsAngle = MathUtilities.SnapTo(rotateTowardsAngle, preciseSnap);
			} else {
				//here we snap to less precise angles
				rotateTowardsAngle = MathUtilities.SnapTo(rotateTowardsAngle, angleSnapFactor);
			}
		}
		
		if ( currentPlatform == null ) return;
		
		if(!isReleasePlatforms) return;
		
		
		
		////Platform Positioning
		currentPlatform.transform.parent.position = Vector3.MoveTowards( currentPlatform.transform.parent.position, transform.position, Time.deltaTime * movingSpeed);
		
		////Platform Rotation
		//platform rotation happens here
		currentPlatform.transform.parent.rotation = Quaternion.RotateTowards(
			rotateFrom,
			Quaternion.AngleAxis(rotateTowardsAngle + currentPlatform.initialAngle, Vector3.forward),
			Time.deltaTime * rotationSpeed);
		
		////Platform Scaling
		currentPlatform.transform.localScale = Vector3.MoveTowards(
			currentPlatform.transform.localScale, 
			new Vector3(Mathf.Clamp(currentPlatform.initialScale.x*GetBoneDistance(), minimumPlatformScale, maximumPlatformScale), currentPlatform.initialScale.y, currentPlatform.initialScale.z), 
			Time.deltaTime * scalingSpeed);
		
	}
	
	float GetBoneDistance(){
		return boneDistanceFactor.Evaluate(bones.Distance)*curveFactor;
	}
	
	// bone area zone
	void OnTriggerEnter( Collider other )
	{
		PlatformInfo pinfo = new PlatformInfo();
		
		pinfo.initialAngle = other.transform.rotation.eulerAngles.z;
		pinfo.initialScale = other.transform.localScale;
		
		pinfo.transform = other.transform;
		
		// if platforms is empty
		if( currentPlatform == null )
		{
			currentPlatform = pinfo;
			
			pinfo.colorState = ColorState.CS_NOTACTIVE;
			other.renderer.material.color = new Color(1f, 0f, 0.7f, 1f);
		}
		// if there is no next
		else if( platforms.Count == 1 )
		{
			pinfo.colorState = ColorState.CS_NEXTTOSELECT;
			other.renderer.renderer.material.color = Color.green;
		}
		
		platforms.Add(pinfo);
	}
	
	void OnTriggerExit( Collider other )
	{
		// find the element in the list and remove it
		PlatformInfo outPlatform = (from x in platforms 
							where x.transform.collider == other 
							select x).First() as PlatformInfo;
		
		int index, nextIndex, secondnext;
		index = platforms.IndexOf( currentPlatform );
		
		// 3 cases:
		// when there is only one platform
		// when there are 2 platforms (index == secondindex)
		// when there are more
		
		// maybe there is a way to optimize cases

		switch( platforms.Count )
		{
		case 0:
			break;
		case 1:
			platforms[index].colorState = ColorState.CS_NOTSELECTED;
			platforms[index].transform.renderer.material.color = Color.white;
			
			currentPlatform = null;
			break;
		case 2:
			nextIndex = index + 1 > platforms.Count - 1 ? 0 : index + 1;
			// if it is current
			if( outPlatform == platforms[index] )
			{		
				platforms[index].colorState = ColorState.CS_NOTSELECTED;
				platforms[index].transform.renderer.material.color = Color.white;
				
				platforms[nextIndex].colorState = ColorState.CS_NOTACTIVE;
				platforms[nextIndex].transform.renderer.material.color = new Color(1f, 0f, 0.7f, 1f);

				currentPlatform = platforms[nextIndex];
			}
			// if it is next one
			else if( outPlatform == platforms[nextIndex] )
			{
				platforms[nextIndex].colorState = ColorState.CS_NOTSELECTED;
				platforms[nextIndex].transform.renderer.material.color = Color.white;
			}
			break;
		// more than 2
		default:
			nextIndex = index + 1 > platforms.Count - 1 ? 0 : index + 1;
			secondnext = nextIndex + 1 > platforms.Count - 1 ? 0 : nextIndex + 1;
			
			// if it is current
			if( outPlatform == platforms[index] )
			{		
				platforms[index].colorState = ColorState.CS_NOTSELECTED;
				platforms[index].transform.renderer.material.color = Color.white;
				
				platforms[nextIndex].colorState = ColorState.CS_NOTACTIVE;
				platforms[nextIndex].transform.renderer.material.color = new Color(1f, 0f, 0.7f, 1f);
				
				platforms[secondnext].colorState = ColorState.CS_NEXTTOSELECT;
				platforms[secondnext].transform.renderer.material.color = Color.green;
			
				currentPlatform = platforms[nextIndex];
			}
			// if it is next one
			else if( outPlatform == platforms[nextIndex] )
			{
				platforms[index].colorState = ColorState.CS_NOTACTIVE;
				platforms[index].transform.renderer.material.color = new Color(1f, 0f, 0.7f, 1f);
				
				platforms[nextIndex].colorState = ColorState.CS_NOTSELECTED;
				platforms[nextIndex].transform.renderer.material.color = Color.white;
				
				platforms[secondnext].colorState = ColorState.CS_NEXTTOSELECT;
				platforms[secondnext].transform.renderer.material.color = Color.green;
			
				currentPlatform = platforms[index];
			}
			
			break;
		}
		
		platforms.Remove(outPlatform);
	}
	
	void OnDrawGizmos(){
		Gizmos.DrawWireSphere(transform.position, (collider as SphereCollider).radius);
		Gizmos.DrawWireSphere(centralPoint, (collider as SphereCollider).radius/2f);
	}
	
	public bool displayDebugValues = true;
	public bool displayInterfaceBox = true;
	
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
				_view = Camera.main.WorldToScreenPoint(plat.transform.position);
				
				_pos.x = _view.x-60f;
				_pos.y = Screen.height-_view.y-15f;
				GUI.color = new Color(0f,0f,0f,0.7f);
				GUI.Label(_pos, "zº:"+plat.transform.rotation.eulerAngles.z);
			}
		}
		
		if(!displayInterfaceBox) return;
		
		Vector3 _magPos = Camera.main.WorldToScreenPoint(transform.position);
		_magPos = new Vector3(_magPos.x, Screen.height - _magPos.y, 0f);
		Rect _boxyThing = new Rect(0f, 0f, 20f, 20f);
		float _distPercent = GetBoneDistance() / boneDistanceFactor.Evaluate(1000);
		float _minimumDistanceAmount = 20f;
		float _distAmount = 400f;
		float _angle = rotateTowardsAngle;
		_distAmount *= _distPercent;
		_distAmount += _minimumDistanceAmount;
		
		GUIUtility.RotateAroundPivot(-rotateTowardsAngle, new Vector2(_magPos.x, _magPos.y));
		_boxyThing.y = _magPos.y-10f;
		_boxyThing.x = _magPos.x - _distAmount - 10f;
		GUI.Box(_boxyThing, "");
		print(""+_distAmount);
		_boxyThing.x = _magPos.x + _distAmount - 10f;
		GUI.Box(_boxyThing, "");
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
#endregion