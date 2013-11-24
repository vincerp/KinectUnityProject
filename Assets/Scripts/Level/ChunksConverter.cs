// This script is able to convert all the platforms inside
// chunks game objects (or any other objects)
// with game object that you attach to this script

// this will change the objects permanently

using UnityEngine;
using System.Collections.Generic;


[ExecuteInEditMode]
public class ChunksConverter : MonoBehaviour 
{
	public GameObject[] levelChunks;
	public GameObject newPlatformPrefab;

	void OnEnable()
	{
		print("Number of chunks: " + levelChunks.Length);
		foreach( GameObject chunk in levelChunks )
		{
			Transform serv = chunk.transform.FindChild("Platforms");
			PlatformChildServices services = serv.GetComponent<PlatformChildServices>();

			List<Platform> listOfPlatforms = new List<Platform>();
			foreach( Transform child in services.transform )
			{
				MonoBehaviour[] monobehs = child.GetComponents<MonoBehaviour>();
				for( int i = 0; i < monobehs.Length; i++ )
				{
					Platform pl = monobehs[i] as Platform;
					if( pl != null )
						listOfPlatforms.Add( pl );
				}
			}

			print ("List of platforms: " + listOfPlatforms.Count);

			foreach( Platform platform in listOfPlatforms )
			{
//				DestroyImmediate( platform.renderer );
//				DestroyImmediate( platform.gameObject.GetComponent<MeshFilter>() );
//
//				GameObject newPlatorm = Instantiate( newPlatformPrefab ) as GameObject;
//				newPlatorm.transform.parent = platform.transform.parent;
			}
		}
	}
}
