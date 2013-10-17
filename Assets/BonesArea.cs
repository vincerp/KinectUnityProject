using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
public class PlatformInfo
{
	public float initialAngle;
	public Vector3 initialScale;
	
	public Transform transform;
}

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
	
	public float boneScaleFactor = 1f;
	public float boneAngleFactor = 1f;
	public float boneAngleAdjustment = 0f;
	
	public float curveFactor = 0.2f;
	public AnimationCurve boneDistanceFactor;
	public float minimumDistance = 0.3f;
	
	public int angleSnapFactor = 5;
	
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
			break;
		case SkeletonPart.SP_KNEES:
			bones.bone1 = skeletonController.Knee_Left.transform;
			bones.bone2 = skeletonController.Knee_Right.transform;
			break;
		}
	}
	
	void Update()
	{
		transform.position = bones.CentralPoint;
		
		foreach ( PlatformInfo info in platforms )
		{
			info.transform.parent.position = Vector3.MoveTowards( info.transform.position, transform.position, Time.deltaTime * movingSpeed);
			info.transform.parent.rotation = Quaternion.RotateTowards( info.transform.rotation, Quaternion.AngleAxis(Mathf.Round((info.initialAngle + (bones.Angle+boneAngleAdjustment)*boneAngleFactor)/angleSnapFactor)*angleSnapFactor, Vector3.forward), Time.deltaTime * rotationSpeed);
			info.transform.localScale = Vector3.MoveTowards(info.transform.localScale, new Vector3(Mathf.Clamp(info.initialScale.x*GetBoneDistance(), minimumDistance, Mathf.Infinity), info.initialScale.y, info.initialScale.z), Time.deltaTime * scalingSpeed);
		}
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
		
		platforms.Add(pinfo);
	}
	
	void OnTriggerExit( Collider other )
	{
		// find the element in the list and remove it
		platforms.Remove((from x in platforms 
							where x.transform.collider == other 
							select x).First() as PlatformInfo);
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