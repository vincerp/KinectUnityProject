using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO: Make this less shitty
public class LevelRandomizer : MonoBehaviour {

	public static LevelRandomizer instance;
	
	public PlayerProgression currentSkillLevel = new PlayerProgression();
	
	public List<LevelChunkSettings> availableChunks = new List<LevelChunkSettings>();
	public List<LevelChunkSettings> initialChunks = new List<LevelChunkSettings>();
	
	private List<string> lastUsedChunkIds = new List<string>();
	
	public int skillTolerableAmount = 2;
	public int rememberHowManyChunks = 3;
	
	public int chunksAtATime = 3;
	public float verticalSpeed = 1f;
	public ScrollType _scrollType = ScrollType.Static;
	public ScrollType scrollType{
		get{
			return _scrollType;
		}
		set{
			_scrollType = value;
			if(_scrollType == ScrollType.ScrollToNextPiece) waitingForChunks++;
		}
	}
	private int waitingForChunks = 0;
	public List<LevelChunkSettings> chunksLoaded;
	
	public float metersScrolled = 0f;
	
	private void Start(){
		instance = this;
		LevelChunkSettings _lcs;
		float _addedHeight = 0f;
		int nOfChunks = 0;
		
//		while(nOfChunks<chunksAtATime){
//			_lcs = Instantiate(SortNewChunk()) as LevelChunkSettings;
//			chunksLoaded.Add(_lcs);
//			_lcs.transform.position = Vector3.up * _addedHeight;
//			_lcs.transform.parent = transform;
//			_addedHeight += _lcs.height;
//			nOfChunks++;
//		}
		while(nOfChunks<chunksAtATime){
			_lcs = Instantiate(initialChunks[nOfChunks]) as LevelChunkSettings;
			chunksLoaded.Add(_lcs);
			_lcs.transform.position = Vector3.up * _addedHeight;
			_lcs.transform.parent = transform;
			_addedHeight += _lcs.height;
			nOfChunks++;
		}
	}
	
	private void FixedUpdate(){
		Transform _ctr;
		
		if(scrollType == ScrollType.Static) return;
		for(int i = chunksLoaded.Count-1; i >= 0; i--){
			_ctr = chunksLoaded[i].transform;
			_ctr.Translate(Vector3.down*verticalSpeed*Time.fixedDeltaTime);
			if(_ctr.position.y <= -chunksLoaded[i].height){
				CreateNewChunk();
			}
		}
		metersScrolled += verticalSpeed*Time.fixedDeltaTime;
	}
	
	private void CreateNewChunk(){
		LevelChunkSettings last, newOb = Instantiate(SortNewChunk()) as LevelChunkSettings;
		Vector3 pos;
		
		//currentSkillLevel += chunksLoaded[0].skillAwarded;
		Destroy(chunksLoaded[0].gameObject);
		chunksLoaded.RemoveAt(0);
		last = chunksLoaded[chunksLoaded.Count-1];
		pos = last.transform.position + (Vector3.up * last.height);
		newOb.transform.position = pos;
		last.transform.parent = transform;
		chunksLoaded.Add(newOb);
		
		if(scrollType == ScrollType.ScrollToNextPiece && waitingForChunks > 0){
			waitingForChunks--;
			//Here we make it scroll only one piece for the tutorial part
			if(waitingForChunks == 0)scrollType = ScrollType.Static;
		}
	}
	
	public LevelChunkSettings SortNewChunk(){
		
		int _randomNumber, _max = availableChunks.Count;
		string _chunkId;
		//sort an ID
		do{
			_randomNumber = Random.Range(0, _max);
			_chunkId = availableChunks[_randomNumber].chunkId;
		}while(lastUsedChunkIds.Contains(_chunkId));
		//let's use it!
		lastUsedChunkIds.Add(_chunkId);
		
		//don't remember too many!
		while(lastUsedChunkIds.Count > rememberHowManyChunks){
			lastUsedChunkIds.RemoveAt(0);
		}
		return availableChunks[_randomNumber];
		
		/*List<LevelChunkSettings> _possibleChunks = new List<LevelChunkSettings>();
		//PlayerProgression _prog;
		
		foreach(LevelChunkSettings chunk in availableChunks){
			//_prog = chunk.progressionNeeded;
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
		return _possibleChunks[_randomNumber];*/
	}
}

public enum ScrollType{
	Static,
	ScrollToNextPiece,
	InfiniteScrolling
}

[System.Serializable]
public class PlayerProgression{
	//This is no longer used. At least for now.
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