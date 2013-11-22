using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(SoundFile))]
public class SoundLinkPropertyDrawer : PropertyDrawer {

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{

		EditorGUI.BeginProperty(position, label, property);
		Rect _label = position;
		_label.width /= 2f;
		EditorGUI.PropertyField(_label, property.FindPropertyRelative("id"), GUIContent.none);
		_label.x += _label.width;
		EditorGUI.PropertyField(_label, property.FindPropertyRelative("sound"), GUIContent.none);
		EditorGUI.EndProperty();
	}
}
