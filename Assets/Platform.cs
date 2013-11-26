using UnityEngine;
using System.Collections.Generic;
using System;

using System.Linq;

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
	
	public Platform reference;
}

public class Platform : MonoBehaviour {
	
	public PlatformType pt;
	public int chainLenght = 5;
	public int railLenght;
	public Pin pin;
	public Rail rail;
	
	public Vector3 offset;

	public ColorState colorState;

	private MeshRenderer[] LEDRenderers;

	public void Start(){

		gameObject.layer = transform.position.x < 0 ? LayerMask.NameToLayer("PlatformP1")
													: LayerMask.NameToLayer("PlatformP2");

		LEDRenderers = 
			(from renderer in (transform.GetComponentsInChildren<MeshRenderer>() as MeshRenderer[]) 
			 where ((from material in renderer.materials 
			        where material.name.Contains( "LED" )
			        select material).Any<Material>() )		 
		select renderer).ToArray();

//		foreach( MeshRenderer rend in (transform.GetComponentsInChildren<MeshRenderer>() as MeshRenderer[]) )
//		{
//			for( int i = 0; i < rend.sharedMaterials.Length; i++ )
//			{
//				if( rend.sharedMaterials[i].name.Contains( "LED" ) )
//				{
//					if( LEDMaterial == null )
//					{
//						Material newMaterial = new Material(rend.sharedMaterials[i]);
//						newMaterial.name = "LED Custom Instance";
//						LEDMaterial = newMaterial;
//					}
//
//
//					rend.sharedMaterial = LEDMaterial;
//					print (LEDMaterial.name);
//					print (rend.sharedMaterial.name);
//				}
//			}
//		}


		rail = transform.GetComponentInChildren<Rail>();
		if(rail != null){
			rail.transform.parent = transform.parent;
			rail.platform = this;
			pt = PlatformType.PT_EZLINE;
			return;
		}
		
//		if(pt == PlatformType.PT_PINNED){
//			Transform _pin = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;
//			_pin.transform.position = transform.position + offset;
//			_pin.localRotation = Quaternion.Euler(90f, 0f, 0f);
//			_pin.renderer.material = EZGrabber.instance.GetLinkedItem("StaticMaterial") as Material;
//			_pin.parent = transform;
//		}
		
		if(pt == PlatformType.PT_ORAIL ||
			pt == PlatformType.PT_ORAILPINNED ||
			pt == PlatformType.PT_VRAIL ||
			pt == PlatformType.PT_VRAILPINNED ||
			pt == PlatformType.PT_EVERYTHING){
			rail = transform.GetChild(0).GetComponent<Rail>();
			rail.transform.parent = transform.parent;
		}
	
	}

	public void SetColorState( ColorState cs )
	{
		colorState = cs;

		foreach( MeshRenderer rend in LEDRenderers )
		{

			switch( cs )
			{
			case ColorState.CS_ACTIVE:
				rend.materials[1].color = Color.red;
				break;
			case ColorState.CS_NEXTTOSELECT:
				rend.materials[1].color = Color.green;
				break;
			case ColorState.CS_NOTACTIVE:
				rend.materials[1].color = Color.white;
				break;
			}

		}
	}
	
	public void OnDrawGizmos(){
		if(pt != PlatformType.PT_PINNED || Application.isPlaying) return;
		Vector3 absOffset = new Vector3(Mathf.Abs(offset.x), Mathf.Abs(offset.y), Mathf.Abs(offset.z));
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position+offset, 1f);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position+offset, ((transform.localScale/2) + absOffset).magnitude);
	}
	
	public enum PlatformType 
	{
		PT_VRAIL,
		PT_ORAIL,
		PT_PINNED,
		PT_CHAINED,
		PT_VRAILPINNED,
		PT_ORAILPINNED,
		PT_EVERYTHING,
		PT_EZLINE
	}
}
#endregion