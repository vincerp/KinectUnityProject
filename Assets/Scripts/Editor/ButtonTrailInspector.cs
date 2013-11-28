using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(ButtonTrail))]
public class ButtonTrailInspector : Editor {

	public override void OnInspectorGUI ()
	{
		ButtonTrail _target = target as ButtonTrail;

		DrawDefaultInspector();

		if(GUILayout.Button("Update Trail")){
			_target.UpdateQuads();
		}
		if(GUILayout.Button("Update Material")){
			_target.UpdateMaterials();
		}
		if(GUILayout.Button("Add Trail Point")){
			_target.AddTrailPosition();
		}

		if(GUI.changed){
			_target.UpdateQuads();
			_target.color = _target.displayColor;
		}
	}
}
