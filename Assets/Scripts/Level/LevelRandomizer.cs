﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO: Make this less shitty
public class LevelRandomizer : MonoBehaviour {

	public static LevelRandomizer instance;
	
	public PlayerProgression currentSkillLevel = new PlayerProgression();
	
	public List<LevelChunkSettings> availableChunks = new List<LevelChunkSettings>();
	
	private List<string> lastUsedChunkIds = new List<string>();
	
	public int skillTolerableAmount = 2;
	public int rememberHowManyChunks = 3;
	
	public void Start(){
		instance = this;
		
		Instantiate(SortNewChunk());
	}
	
	public LevelChunkSettings SortNewChunk(){
		
		List<LevelChunkSettings> _possibleChunks = new List<LevelChunkSettings>();
		PlayerProgression _prog;
		
		foreach(LevelChunkSettings chunk in availableChunks){
			_prog = chunk.progressionNeeded;
			if(PlayerProgression.CompareCompatibleSkills(currentSkillLevel, chunk.progressionNeeded, skillTolerableAmount))
			{
				//would be good if we could also ignore the ones that demand too little skill
				_possibleChunks.Add(chunk);
			}
			
		}
		if(_possibleChunks.Count == 0){
			Debug.LogError("There are no levels for this skill configuration!\n"+currentSkillLevel);
			return null;
		}
		
		int _randomNumber, _max = _possibleChunks.Count;
		string _chunkId;
		do{
			_randomNumber = Random.Range(0, _max);
			_chunkId = _possibleChunks[_randomNumber].chunkId;
		}while(lastUsedChunkIds.Contains(_chunkId));
		
		lastUsedChunkIds.Add(_chunkId);
		while(lastUsedChunkIds.Count > rememberHowManyChunks){
			lastUsedChunkIds.RemoveAt(0);
		}
		return _possibleChunks[_randomNumber];
	}
}

[System.Serializable]
public class PlayerProgression{
	public int tutorialSteps = 0;
	public int platformMovement = 0;
	public int platformRotating = 0;
	public int platformScaling = 0;
	
	public static bool CompareCompatibleSkills(PlayerProgression baseParams, PlayerProgression comparisson, int tolerableDifferenceAmount){
		if(baseParams.tutorialSteps >= comparisson.tutorialSteps &&
			baseParams.platformMovement >= comparisson.platformMovement &&
			baseParams.platformRotating >= comparisson.platformRotating &&
			baseParams.platformScaling >= comparisson.platformScaling) {
			
			if(baseParams.tutorialSteps - tolerableDifferenceAmount < comparisson.tutorialSteps &&
				baseParams.platformMovement - tolerableDifferenceAmount < comparisson.platformMovement &&
				baseParams.platformRotating - tolerableDifferenceAmount < comparisson.platformRotating &&
				baseParams.platformScaling - tolerableDifferenceAmount < comparisson.platformScaling) return true;
			Debug.Log("Chunk rejected because it was too easy.");
			return false;
		}
		Debug.Log("Chunk rejected because it was too hard.");
		return false;
	}
	
	public static PlayerProgression operator +(PlayerProgression a, PlayerProgression b){
		a.tutorialSteps += b.tutorialSteps;
		a.platformMovement += b.platformMovement;
		a.platformRotating += b.platformRotating;
		a.platformScaling += b.platformScaling;
		
		return a;
	}
	
	public override string ToString ()
	{
		return "[PlayerProgression]"+
			tutorialSteps+"|"+
				platformMovement+"|"+
				platformRotating+"|"+
				platformScaling;
	}
}