using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[System.Serializable]
public class AddStateInfo{
	public FatherStates newState;
}

[CustomEditor(typeof(RoteiroPai))]
public class FatherRoteiroEditor : Editor {

	private ReorderableList list;
	private RoteiroPai roteiro;

	float lineHeight;
	float lineHeightSpace;
	float elementHeightSpace;

	GUIStyle labelStyle = new GUIStyle ();
	GUIStyle currStateStyle = new GUIStyle ();

	private void OnEnable() {
		if(target == null){
			return;
		}

		roteiro = (RoteiroPai)target;

		lineHeight = EditorGUIUtility.singleLineHeight;
		lineHeightSpace = lineHeight + 5;
		elementHeightSpace = lineHeight + 10;

		labelStyle.fontStyle = currStateStyle.fontStyle = FontStyle.Bold;
		labelStyle.clipping = currStateStyle.clipping = TextClipping.Clip;
		currStateStyle.normal.textColor = new Color (0.7f, 0.1f, 0.3f);

		list = new ReorderableList(serializedObject, 
			serializedObject.FindProperty("roteiro"), 
			true, false, true, true);

		list.drawHeaderCallback = (Rect rect) =>
		{
			rect.y += 2;
			EditorGUI.LabelField(rect, new GUIContent("Roteiro do Pai na Fase"));
		};

		//list.drawElementBackgroundCallback

		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
		{
			var element = list.serializedProperty.GetArrayElementAtIndex(index);

			rect.y += 2;
			EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 80, lineHeight), element.FindPropertyRelative("name").stringValue, labelStyle);

			EditorGUIUtility.labelWidth = 40;
			EditorGUI.PropertyField(new Rect(rect.width - 40, rect.y, 80, lineHeight), element.FindPropertyRelative("show"));
			EditorGUIUtility.labelWidth = 0;

			if(element.FindPropertyRelative("show").boolValue){
				int i = 1;

				EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("name"));
				i++;
				EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("state"));
				i++;

				if(element.FindPropertyRelative("state").enumValueIndex == 5 || element.FindPropertyRelative("state").enumValueIndex == 7 || element.FindPropertyRelative("state").enumValueIndex == 8){
					EditorGUI.indentLevel++;
					EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("destination"));
					i++;
					EditorGUI.indentLevel--;
				}
				else if(element.FindPropertyRelative("state").enumValueIndex == 2){
					EditorGUI.indentLevel++;
					EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("areaCenter"));
					i++;
					EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("areaRadius"));
					i++;
					EditorGUI.indentLevel--;
				}

				// ---------------------------- OPTIONAL STATE VARIABLES ------------------------- \\
				if(element.FindPropertyRelative("state").enumValueIndex == 3 || element.FindPropertyRelative("state").enumValueIndex == 8){
					EditorGUI.indentLevel++;
					element.FindPropertyRelative("showAdvanced").boolValue = EditorGUI.Foldout(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("showAdvanced").boolValue, new GUIContent("Advanced"), new GUIStyle(EditorStyles.foldout));
					i++;
					if(element.FindPropertyRelative("showAdvanced").boolValue){
						EditorGUI.indentLevel++;
						EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("secondsFlying"));
						i++;
						EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("allowSlowFalling"));
						i++;
						EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("jumpHeight"));
						i++;
						EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("timeToJumpApex"));
						i++;
						EditorGUI.indentLevel--;
					}
					EditorGUI.indentLevel--;
				}
				else if(element.FindPropertyRelative("state").enumValueIndex == 4){
					EditorGUI.indentLevel++;
					element.FindPropertyRelative("showAdvanced").boolValue = EditorGUI.Foldout(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("showAdvanced").boolValue, new GUIContent("Advanced"));
					i++;
					if(element.FindPropertyRelative("showAdvanced").boolValue){
						EditorGUI.indentLevel++;
						EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("jumpHeight"));
						i++;
						EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("timeToJumpApex"));
						i++;
						EditorGUI.indentLevel--;
					}
					EditorGUI.indentLevel--;
				}
				// ------------------------------------------------------------------------------- \\


				EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("songType"));
				i++;

				// ---------------------------- OPTIONAL SING VARIABLES ------------------------- \\
				if(element.FindPropertyRelative("songType").enumValueIndex == 1){ //Partitura
					EditorGUI.indentLevel++;
					EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("songIndex"));
					i++;
					EditorGUI.indentLevel--;
				}
				else if (element.FindPropertyRelative("songType").enumValueIndex == 2) { //Musica Simples
					EditorGUI.indentLevel++;
					EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("simpleSong"));
					i++;
					EditorGUI.indentLevel--;
				}
				else if (element.FindPropertyRelative("songType").enumValueIndex == 3) { //Musica com Sustain
					EditorGUI.indentLevel++;
					EditorGUIUtility.labelWidth = 100;
					EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width - 100, lineHeight), element.FindPropertyRelative("sustainSong"));
					EditorGUIUtility.labelWidth = 70;
					EditorGUI.PropertyField(new Rect(rect.width - 70, rect.y + (lineHeightSpace * i), 100, lineHeight), element.FindPropertyRelative("duration"));
					EditorGUIUtility.labelWidth = 0;
					i++;
					EditorGUI.indentLevel--;
				}
				// ------------------------------------------------------------------------------ \\

				EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("startingHeight"));
				i++;


				EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("stateChanger"));
				i++;
				EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeightSpace * i), rect.width, lineHeight), element.FindPropertyRelative("triggerDetail"));
				//i++;

				serializedObject.ApplyModifiedProperties();
			}
		};


		list.elementHeightCallback = (int index) =>
		{
			float height = 0;

			var element = list.serializedProperty.GetArrayElementAtIndex(index);

			float i = 6;

			if(element.FindPropertyRelative("show").boolValue){
				switch (element.FindPropertyRelative("state").enumValueIndex) {
				case 2: //RandomWalk
					i = 8;
					break;
				case 3: //Gliding
					i = (element.FindPropertyRelative("showAdvanced").boolValue)? 10 : 7;
					break;
				case 4: //Jumping
					i = (element.FindPropertyRelative("showAdvanced").boolValue)? 9 : 7;
					break;
				case 5: //SimpleWalk
					i = 7;
					break;
				case 7: //GuidingPlayer
					i = 7;
					break;
				case 8: //Flying
					i = (element.FindPropertyRelative("showAdvanced").boolValue)? 11 : 8;
					break;
				default:
				break;
				}

				if (element.FindPropertyRelative("songType").enumValueIndex != 0) {
					i++;
				}
			}
			else {
				i = 1.5f;
			}
				
			height = elementHeightSpace * i;

			serializedObject.ApplyModifiedProperties();
			return height;
		};

		list.onAddDropdownCallback = (Rect rect, ReorderableList rList) =>
		{
			GenericMenu dropdownMenu = new GenericMenu();

			dropdownMenu.AddItem(new GUIContent("Add State"), false, AddState, new AddStateInfo { newState = FatherStates.Inactive });

			dropdownMenu.AddItem(new GUIContent("Idle/Inactive"), false, AddState, new AddStateInfo { newState = FatherStates.Inactive });
			dropdownMenu.AddItem(new GUIContent("Idle/LookingAtPlayer"), false, AddState, new AddStateInfo { newState = FatherStates.LookingAtPlayer });
			dropdownMenu.AddItem(new GUIContent("Idle/RandomWalk"), false, AddState, new AddStateInfo { newState = FatherStates.RandomWalk });
			dropdownMenu.AddItem(new GUIContent("Idle/Gliding"), false, AddState, new AddStateInfo { newState = FatherStates.Gliding });
			dropdownMenu.AddItem(new GUIContent("Idle/Jumping"), false, AddState, new AddStateInfo { newState = FatherStates.Jumping });

			dropdownMenu.AddItem(new GUIContent("Walk/SimpleWalk"), false, AddState, new AddStateInfo { newState = FatherStates.SimpleWalk });
			dropdownMenu.AddItem(new GUIContent("Walk/FollowPlayer"), false, AddState, new AddStateInfo { newState = FatherStates.FollowingPlayer });
			dropdownMenu.AddItem(new GUIContent("Walk/GuidePlayer"), false, AddState, new AddStateInfo { newState = FatherStates.GuidingPlayer });
			dropdownMenu.AddItem(new GUIContent("Walk/Flying"), false, AddState, new AddStateInfo { newState = FatherStates.Flying });

			dropdownMenu.ShowAsContext ();
		};

	}

	public void AddState (object obj){
		AddStateInfo stateInfo = (AddStateInfo)obj;

		RoteiroPai.Roteiro newAction = new RoteiroPai.Roteiro ();
		newAction.state = stateInfo.newState;
		newAction.name = "Estado " + roteiro.roteiro.Count.ToString ();
		//newAction.show = true;

		roteiro.roteiro.Add (newAction);
	}

	public override void OnInspectorGUI() {
		//TODO:Comment later
		//DrawDefaultInspector ();

		serializedObject.Update();

		EditorGUILayout.PropertyField (serializedObject.FindProperty("fatherFSM"), new GUIContent ("Father State Controller"));

		EditorGUILayout.Separator ();

		int currentStateIndex = serializedObject.FindProperty("currentState").intValue;
		string currentStateName = " (" + currentStateIndex + ") - " + list.serializedProperty.GetArrayElementAtIndex (currentStateIndex).FindPropertyRelative ("name").stringValue;
		EditorGUIUtility.labelWidth = 90;
		EditorGUILayout.TextField(new GUIContent("Current State: "), currentStateName, currStateStyle);
		EditorGUIUtility.labelWidth = 0;

		EditorGUILayout.Separator ();

		list.DoLayoutList();

		serializedObject.ApplyModifiedProperties();
	}

}
