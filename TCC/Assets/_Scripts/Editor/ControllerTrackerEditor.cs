using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ControllerTracker))]
public class ControllerTrackerEditor : Editor {



	public override void OnInspectorGUI(){

		ControllerTracker ct = target as ControllerTracker;

		EditorGUILayout.LabelField ("Axes", EditorStyles.boldLabel);
		//EditorGUILayout.HelpBox("Can't find a way to make it work through here.", MessageType.Info);

		if(ct.axis.Length == 0){
			EditorGUILayout.HelpBox("No axes defined in InputManager.", MessageType.Info);
		} else {
			//SerializedProperty prop = serializedObject.FindProperty ("JoystickAxisKeys");
			for (int i = 0; i < ct.axis.Length; i++) {
				//EditorGUILayout.PropertyField (prop.GetArrayElementAtIndex (i), new GUIContent("Axis " + i));
				ct.axis[i] = (JoystickAxes)EditorGUILayout.EnumPopup ("Axis " + i, ct.axis[i]);
			}
		}

		EditorGUILayout.LabelField ("Buttons", EditorStyles.boldLabel);

		if(ct.buttonKeys.Length == 0){
			EditorGUILayout.HelpBox("No buttons defined in InputManager.", MessageType.Info);
		} else {
			for (int i = 0; i < ct.buttonKeys.Length; i++) {
				ct.buttonKeys [i] = (KeyCode)EditorGUILayout.EnumPopup ("Button " + i, ct.buttonKeys [i]);
			}
		}

		serializedObject.ApplyModifiedProperties ();
		serializedObject.Update ();

	}
}
