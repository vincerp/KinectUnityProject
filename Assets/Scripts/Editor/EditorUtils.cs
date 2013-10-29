using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorUtils {

	[MenuItem("Assets/Create/New Ability")]
	public static void CreateAbility(){
		AbilitySettings _new = ScriptableObject.CreateInstance<AbilitySettings>();
		AssetDatabase.CreateAsset(_new, "Assets/Abilities/New Ability.asset");
		AssetDatabase.SaveAssets();
		_new.scriptName = "New Ability";
		_new.duration = 5f;
		Selection.activeObject = _new;
	}
}
