// This script is able to convert all the platforms inside
// chunks game objects (or any other objects)
// with game object that you attach to this script

// this will change the objects permanently

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


//[ExecuteInEditMode]
public class ChunksConverter : MonoBehaviour 
{
	public GameObject[] levelChunks;
	public GameObject newPlatformPrefab;

	void OnEnable()
	{
		print("Number of chunks: " + levelChunks.Length);
		foreach( GameObject chunk in levelChunks )
		{
			GameObject runTimeChunk = Instantiate(chunk) as GameObject;
			Platform[] platforms = runTimeChunk.GetComponentsInChildren<Platform>();

			print ("List of platforms: " + platforms.Length);

			foreach( Platform platform in platforms )
			{

				// Well, for some reasons, some nigga folks sized the platform cube up using Y coordinate and then rotated it 270 degrees to make it horizontal
				// Why those nigga folks would do that - I dunno. But here, rotating it back.
//				 And also 180 deg around X, coz the new platform from Alessio nigga is rotated that way.
				platform.transform.localRotation *= Quaternion.AngleAxis(-270f, platform.transform.forward)/* * Quaternion.AngleAxis(180f, platform.transform.right)*/;

				DestroyImmediate( platform.gameObject.renderer );
				DestroyImmediate( platform.gameObject.GetComponent<MeshFilter>() );

				GameObject newPlatorm = Instantiate( newPlatformPrefab, platform.transform.position, platform.transform.rotation  ) as GameObject;

				Transform platformBase = newPlatorm.transform.FindChild("BASE PLATFORM");
				Transform pin = newPlatorm.transform.FindChild("PIN");
				Transform sideParts = newPlatorm.transform.FindChild("SIDE PARTS");

				foreach( Transform child in newPlatorm.transform )
					child.gameObject.SetActive(false);

				platformBase.gameObject.SetActive(true);

				sideParts.localScale = platformBase.localScale = new Vector3(platform.transform.localScale.y * 0.13f, platform.transform.localScale.x, platform.transform.localScale.z); 
				(platform.collider as BoxCollider).size = new Vector3(platform.transform.localScale.y, platform.transform.localScale.x, platform.transform.localScale.z);
				platform.transform.localScale = Vector3.one;

				newPlatorm.transform.parent = platform.transform;
				newPlatorm.transform.localPosition = Vector3.zero;

				// platform dependency code

				if( platform.pt == Platform.PlatformType.PT_VRAILPINNED || 
				   	platform.pt == Platform.PlatformType.PT_ORAILPINNED || 
				   	platform.pt == Platform.PlatformType.PT_PINNED ||
					platform.pt == Platform.PlatformType.PT_EVERYTHING)
						pin.gameObject.SetActive(true);


//				if( platform.pt == Platform.PlatformType.PT_VRAILPINNED || 
//				   	platform.pt == Platform.PlatformType.PT_ORAILPINNED || 
//				   	platform.pt == Platform.PlatformType.PT_VRAIL ||
//				   	platform.pt == Platform.PlatformType.PT_ORAIL ||
//				   	platform.pt == Platform.PlatformType.PT_EZLINE ||
//				   	platform.pt == Platform.PlatformType.PT_EVERYTHING )
						sideParts.gameObject.SetActive(true);
				
			}

			PrefabUtility.CreatePrefab( "Assets/Prefabs/Level Chunks/converted/" + chunk.name + ".prefab", runTimeChunk);
			Debug.Log("Level chunk " + chunk.name + " has been converted");
			Destroy(runTimeChunk);
		}
	}
}
