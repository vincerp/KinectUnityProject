using UnityEngine;
using System.Collections;

public class LevelChunkSettings : MonoBehaviour {
	
	public string chunkId = "gimme some ID, bro!";
	/// <summary>
	/// The progression needed for this chunk to be able to be sorted.
	/// </summary>
	public PlayerProgression progressionNeeded;
	/// <summary>
	/// The skill awarded when this chunk is randomized.
	/// </summary>/
	public PlayerProgression skillAwarded;
	
	public float height = 5f;

#if UNITY_EDITOR
	public bool showGrid = true;

	void OnDrawGizmos(){
		float levelWidth = 70f;
		float levelDepth = 5f;
		float levelHeight = height;

		float blockDefault = 10f;

		Vector3 center;
		Vector3 size;

		Gizmos.color = new Color(0.8f, 0.8f, 0.1f, 0.7f);

		//Bottom
		center = new Vector3(0f, -blockDefault/2f, (levelDepth-1)/2f);
		size = new Vector3(levelWidth+blockDefault*2f, blockDefault, levelDepth);
		Gizmos.DrawCube(center, size);
		//Top
		center = new Vector3(0f, levelHeight+blockDefault/2f, (levelDepth-1)/2f);
		Gizmos.DrawCube(center, size);
		//right
		center = new Vector3((levelWidth+blockDefault)/2f, levelHeight/2f, (levelDepth-1)/2f);
		size = new Vector3(blockDefault, levelHeight, levelDepth);
		Gizmos.DrawCube(center, size);
		//left
		center = new Vector3(-(levelWidth+blockDefault)/2f, levelHeight/2f, (levelDepth-1)/2f);
		Gizmos.DrawCube(center, size);

		if(!showGrid) return;
		////Grid drawing
		Color dark = new Color(1f, 0f, 0f, 0.4f);
		Color light = new Color(1f, 0f, 0f, 0.1f);
		//vertical lines
		float max = levelWidth/2f;
		Vector3 start, end;
		//central line
		Gizmos.color = light;
		start = new Vector3(0f, 0f, 0.5f);
		end = new Vector3(0f, levelHeight, 0.5f);
		Gizmos.DrawLine(start, end);

		for(float w = 0.5f; w<max; w++){
			if(w%4==0.5f){
				Gizmos.color = dark;
			} else {
				Gizmos.color = light;
			}
			start = new Vector3(w, 0f, 0.5f);
			end = new Vector3(w, levelHeight, 0.5f);
			Gizmos.DrawLine(start, end);
			start = new Vector3(-w, 0f, 0.5f);
			end = new Vector3(-w, levelHeight, 0.5f);
			Gizmos.DrawLine(start, end);
		}
		//horizontal lines
		max = levelHeight;
		for(float h = 0.5f; h<max; h++){
			if(h%4==0.5f){
				Gizmos.color = dark;
			} else {
				Gizmos.color = light;
			}
			start = new Vector3(-levelWidth/2f, h, 0.5f);
			end = new Vector3(levelWidth/2f, h, 0.5f);
			Gizmos.DrawLine(start, end);
		}
	}
#endif
}