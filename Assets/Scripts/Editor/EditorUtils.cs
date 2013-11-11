using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EditorUtils {
	/*		Cheat Sheet:
	 * 		_	no modifier
	 * 		%	ctrl
	 * 		&	alt
	 * 		#	shift
	 */
	
	public static bool showDebugLog = false;
	
	[MenuItem("Other/Toggle custom shortcut debug log")]
	public static void ToggleShowDebug(){
		showDebugLog = !showDebugLog;
		Debug.LogWarning("Console will " + ((showDebugLog)?"now ":"no longer ") + "display debug messages.");
	}

	[MenuItem("Assets/Create/New Ability")]
	public static void CreateAbility(){
		AbilitySettings _new = ScriptableObject.CreateInstance<AbilitySettings>();
		AssetDatabase.CreateAsset(_new, "Assets/Abilities/New Ability.asset");
		AssetDatabase.SaveAssets();
		_new.scriptName = "New Ability";
		_new.duration = 5f;
		Selection.activeObject = _new;
	}
	
	#region GameObject Additions
	[MenuItem("GameObject/Create Parent %1")]
	public static void CreateParent(){
		Transform _selected;
		List<Transform> _sels;
		GameObject _go;
		
		if(Selection.gameObjects.Count() == 1){
			_selected = Selection.activeGameObject.transform;
			_go = new GameObject("Parent of " + _selected.name);
			_go.transform.position = _selected.position;
			if(_selected.parent) _go.transform.parent = _selected.parent;
			_selected.parent = _go.transform;
			Selection.activeGameObject = _go;
			if(showDebugLog)Debug.Log("Parent created for " + _selected.name);
		} else if (Selection.gameObjects.Count() > 1){
			_sels = new List<Transform>();
			foreach(GameObject gobj in Selection.gameObjects){
				_sels.Add(gobj.transform);
			}
			_go = new GameObject("New Parent");
			_go.transform.parent = _sels[0].parent;
			foreach(Transform tr in _sels) tr.parent = _go.transform;
			if(showDebugLog)Debug.Log("Parent created for selection group");
		} else {
			Debug.LogError("Nothing selected!");
			return;
		}
		Selection.activeGameObject = _go;
		
	}
	
	[MenuItem("GameObject/Mirror X Position &s")]
	public static void MirrorXPosition(){
		Transform _selected = Selection.activeGameObject.transform;
		Vector3 _lp = _selected.localPosition;
		Vector3 _lr = _selected.localRotation.eulerAngles;
		_selected.localPosition = new Vector3(-_lp.x, _lp.y, _lp.z);
		_selected.localRotation = Quaternion.Euler(_lr.x, _lr.y, -_lr.z);
		
		if(showDebugLog)Debug.Log("Mirrored position for " + _selected.name);
	}
	
	[MenuItem("GameObject/Reset Position %'")]
	public static void ResetPosition(){
		Transform _selected = Selection.activeGameObject.transform;
		_selected.localPosition = Vector3.zero;
		
		if(showDebugLog)Debug.Log("Position reseted for " + _selected.name);
	}
	
	[MenuItem("GameObject/Reset Transform %#'")]
	public static void ResetTransform(){
		Transform _selected = Selection.activeGameObject.transform;
		_selected.localPosition = Vector3.zero;
		_selected.localRotation = Quaternion.identity;
		_selected.localScale = Vector3.one;
		
		if(showDebugLog)Debug.Log("Transform reseted for " + _selected.name);
	}
	#endregion
	
	#region Selection Menu
	[MenuItem("Selection/Select Parent &c")]
	public static void SelectParent(){
		Transform _selected = Selection.activeGameObject.transform;
		Selection.activeGameObject = _selected.parent.gameObject;
	}
	
	[MenuItem("Selection/Select Child #%c")]
	public static void SelectChild(){
		Transform _selected = Selection.activeGameObject.transform;
		Selection.activeGameObject = _selected.GetChild(0).gameObject;
	}
	
	[MenuItem("Selection/Select Next Sibling #v")]
	public static void SelectSibling(){
		Transform _selected = Selection.activeGameObject.transform;
		int max = _selected.parent.childCount;
		int next = GetChildIndex(_selected)+1;
		if(next >= max) next = 0;
		Selection.activeGameObject = _selected.parent.GetChild(next).gameObject;
	}
	
	private static int GetChildIndex(Transform tr){
		Transform parent = tr.parent;
		for(int i = 0; i<parent.childCount; i++){
			if(parent.GetChild(i)==tr) return i;
		}
		return -1;
	}
	#endregion
}