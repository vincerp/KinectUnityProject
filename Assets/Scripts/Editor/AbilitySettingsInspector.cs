using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 0618
[CustomEditor(typeof(AbilitySettings))]
public class AbilitySettingsInspector : Editor {

	MonoScript[] possibleScripts;
	AbilitySettings lastTarget;
	bool showDefaultInspector = false;

	override public void OnInspectorGUI () {
		AbilitySettings _target = (AbilitySettings)target;

		string selected = _target.scriptName;
		if(possibleScripts == null){
			if(GUILayout.Button("Find possible scripts and crash Unity maybe")){
				possibleScripts = GetScriptAssetsOfType<BaseAbility>();
			}
		} else {
			bool selectedDoesntExists = possibleScripts.FirstOrDefault(x => x.name == selected) == null;
			if((_target.scriptName == "" || selectedDoesntExists) && possibleScripts != null){
				EditorGUILayout.HelpBox("No valid script assigned! Please select one script.", MessageType.Error);
			}
			if(_target.animation == null){
				EditorGUILayout.HelpBox("This Power Up is missing an introduction animation!", MessageType.Warning);
			}
			EditorGUILayout.HelpBox("Select a Power Up from one of the available scripts below:", MessageType.None);
			foreach(MonoScript sc in possibleScripts){
				if(GUILayout.Toggle(sc.name == selected, sc.name)) selected = sc.name;
			}
			_target.scriptName = selected;
		}

		GUIStyle boldMe = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).label;
		boldMe.fontStyle = FontStyle.Bold;

		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Power Up Values:", boldMe);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Animation");
		_target.animation = EditorGUILayout.ObjectField(_target.animation, typeof(GameObject), false) as GameObject;
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Particles");
		_target.particles = EditorGUILayout.ObjectField(_target.particles, typeof(GameObject), false) as GameObject;
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Pick-up Sound");
		_target.pickupSound = EditorGUILayout.TextField(_target.pickupSound);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Duration (seconds)");
		_target.duration = EditorGUILayout.FloatField(_target.duration);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Ability Damage");
		_target.abilityDamage = EditorGUILayout.FloatField(_target.abilityDamage);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();
		EditorGUILayout.LabelField("Notes:", boldMe);
		GUIStyle notesStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).textArea;
		notesStyle.wordWrap = true;
		_target.notes = EditorGUILayout.TextArea(_target.notes, notesStyle);
		
		EditorGUILayout.Separator();
		showDefaultInspector = GUILayout.Toggle(showDefaultInspector, "Show default inspector:");
		if(showDefaultInspector)DrawDefaultInspector();
		if(GUI.changed) EditorUtility.SetDirty(target);
	}

	public static MonoScript[] GetScriptAssetsOfType<T>()
	{

		MonoScript[] scripts = (MonoScript[])Object.FindObjectsOfTypeIncludingAssets( typeof( MonoScript ) );
		//List<MonoScript> scripts = new List<MonoScript>();
		//Object[] scripts = AssetDatabase.LoadAllAssetsAtPath("Assets/Scripts/GameplayElements/Abilities");
		//Debug.Log("%"+scripts.Length);

		List<MonoScript> result = new List<MonoScript>();
		
		foreach( MonoScript m in scripts )
		{
			if( m.GetClass() != null && m.GetClass().IsSubclassOf( typeof( T ) ) )
			{
				result.Add( m );
			}
		}
		return result.ToArray();
	}
}
