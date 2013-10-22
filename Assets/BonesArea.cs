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
	public float distancePositionMultiplier = 1f;
	
	
	public KeyCode releasePlatformsKeycode = KeyCode.Joystick1Button5;
	public KeyCode switchPlatformKeycode = KeyCode.Joystick1Button6;
	public bool isReleasePlatforms = false;
	
	public bool displayPlatformAngles = true;
	
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
				0.7f);
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
			if( platforms.Count > 1 )
			{
				int index = platforms.IndexOf( currentPlatform );
				int nextIndex = index + 1 > platforms.Count - 1 ? 0 : index + 1;
				int secondnext = nextIndex + 1 > platforms.Count - 1 ? 0 : nextIndex + 1;
				
				platforms[index].colorState = ColorState.CS_NOTSELECTED;
				platforms[index].transform.renderer.material.color = Color.white;
				
				platforms[nextIndex].colorState = ColorState.CS_NOTACTIVE;
				platforms[nextIndex].transform.renderer.material.color = new Color(1f, 0f, 0.7f, 1f);
				
				platforms[secondnext].colorState = ColorState.CS_NEXTTOSELECT;
				platforms[secondnext].transform.renderer.material.color = Color.green;
				
				currentPlatform = platforms[nextIndex];
			}
		}
	}
	
	void FixedUpdate()
	{
		////Magnet positioning
		transform.position = Vector3.MoveTowards(transform.position,
			bones.CentralPoint + (bones.CentralPoint - centralPoint)*distancePositionMultiplier,
			magnetMovementSpeed);
		
		if ( currentPlatform == null ) return;
		
		if(!isReleasePlatforms) return;
		
		
		
		////Platform Positioning
		currentPlatform.transform.parent.position = Vector3.MoveTowards( currentPlatform.transform.position, transform.position, Time.deltaTime * movingSpeed);
		
		////Platform Rotation
		//if the bones are too close, we will have rotation issues
		//so we will only update rotation if they are not too close
		if(bones.Distance > distanceDeadZone){
			rotateTowardsAngle = (currentPlatform.initialAngle + (bones.Angle+boneAngleAdjustment)*boneAngleFactor);
			//now we snap the value so it doesn't flicker
			if(rotateTowardsAngle%preciseSnap < preciseSnapRange || rotateTowardsAngle%preciseSnap > preciseSnap - preciseSnapRange) {
				//here we snap to precise degree angles
				rotateTowardsAngle = MathUtilities.SnapTo(rotateTowardsAngle, preciseSnap);
			} else {
				//here we snap to less precise angles
				rotateTowardsAngle = MathUtilities.SnapTo(rotateTowardsAngle, angleSnapFactor);
			}
		}
		//platform rotation happens here
		currentPlatform.transform.parent.rotation = Quaternion.RotateTowards(
			currentPlatform.transform.rotation,
			Quaternion.AngleAxis(rotateTowardsAngle, Vector3.forward),
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
	}
	
	void OnGUI(){
		GUILayout.Box("Info:\nAngle:"+(bones.Angle+boneAngleAdjustment)+
			"\nDistance:"+bones.Distance+
			"\nGetBoneDistance:"+GetBoneDistance()+
			"\n1:"+bones.bone1.position.ToString()+
			"\n2:"+bones.bone2.position.ToString()+
			"\ntrue angle:"+transform.localRotation.z
			);
		
		GUILayout.Box("Angle:\n"+Vector3.Angle(Vector3.zero, Vector3.zero)+"\n"+Vector3.Angle(-Vector3.left, Vector3.right));
		
		
		if(!displayPlatformAngles) return;
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