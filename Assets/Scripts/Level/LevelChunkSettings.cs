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
}