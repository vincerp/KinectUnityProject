using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {
	
	// This is a static function to get a free spawnpoint.
	public static Transform GetRandomSpawnpoint()
	{		
		// Get all spawnpoints
		GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
		
		// Make sure there are any, otherwise just give up.
		if(spawnPoints.Length <= 0) 
		{
			Debug.LogError("No spawnpoints found :(");
			return null;
		}
				
		// Now we are going to randomize the array of spawnpoints we have
		// With the	"Fischer Yates algorithm", don't ask me how it works, I found the code here
		// http://stackoverflow.com/questions/108819/best-way-to-randomize-a-string-array-with-net
		System.Random rng = new System.Random();
       	int n = spawnPoints.Length;

        while (n > 1) 
        {
            int k = rng.Next(n--);
            GameObject temp = spawnPoints[n];
            spawnPoints[n] = spawnPoints[k];
            spawnPoints[k] = temp;
        }		
		
		// Now that we have a randomized list of spawnpoints, let's just go through them and 
		// find one that is unoccupied
		for (int i = 0; i < spawnPoints.Length; i++) {
			
			// Physics.OverlapSphere is a static Unity function that returns all the colliders that are touching
			// a sphere. 
			Collider[] colliders = Physics.OverlapSphere(spawnPoints[i].transform.position,1);
			bool playerInside = false;
			
			// Now we are checking the tags of the colliders, to see if there's a player
			foreach (Collider collider in colliders) {
				if(collider.tag == "Player")
				{
					// If there's a player, just forget about this spawn...
					playerInside = true;
					break;
				}
			}
			
			// If no player was found
			if(!playerInside)
			{
				return spawnPoints[i].transform;
			}
		}
		
		// We should only get here if all spawns are occupied...
		Debug.LogWarning("No empty spawnpoints found");
		return spawnPoints[0].transform;
	}	
}
