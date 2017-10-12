using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Keeps track of whatever key inputs are there;
//Then it passes this info back to InputManager
public class ControllerTracker : DeviceTracker {

	public JoystickAxes[] axis;
	public KeyCode[] buttonKeys;
	private string[] axisName = new string[0];
	private int axisCount;

	void Reset(){ //built-in function
		im = GetComponent<InputManager>();
		axis = new JoystickAxes[im.axisCount];
		axisName = new string[axis.Length];
		buttonKeys = new KeyCode[im.buttonCount];
	}

	public override void Refresh(){
		im = GetComponent<InputManager> ();

		//Create 2 temp arrays for buttons and axes
		KeyCode[] newButtons = new KeyCode[im.buttonCount];
		JoystickAxes[] newAxes = new JoystickAxes[im.axisCount];

		if(buttonKeys != null){
			for (int i = 0; i < Mathf.Min(newButtons.Length, buttonKeys.Length); i++) {
				newButtons [i] = buttonKeys [i];
			}
		}
		buttonKeys = newButtons;


		if(axis != null){
			for (int i = 0; i < Mathf.Min(newAxes.Length, axis.Length); i++) {
				newAxes [i] = axis [i];
			}
		}
		axis = newAxes;
	}

	void Start(){
		axisName = new string[axis.Length];
		for (int i = 0; i < axis.Length; i++) {
			switch (axis[i]) {
			case JoystickAxes.L_Horizontal_Joystick:
				axisName [i] = "L_Joystick_X";
				break;
			case JoystickAxes.L_Vertical_Joystick:
				axisName [i] = "L_Joystick_Y";
				break;
			case JoystickAxes.R_Horizontal_Joystick:
				axisName [i] = "R_Joystick_X";
				break;
			case JoystickAxes.R_Vertical_Joystick:
				axisName [i] = "R_Joystick_Y";
				break;
			case JoystickAxes.L_Trigger:
				axisName [i] = "L_Trigger";
				break;
			case JoystickAxes.R_Trigger:
				axisName [i] = "R_Trigger";
				break;
			default:
				Debug.LogWarning("Whoa");
				break;
			}
		}
	}

	void Update () {
		//check for inputs, if inputs detected, set newData to true
		//populate inputData to pass to the InputManager

		for (int i = 0; i < axis.Length; i++) {
			float val = 0f;
			if(Mathf.Abs(Input.GetAxis(axisName[i])) >= 0.1f){
				val += Input.GetAxis (axisName [i]);
				//print (val + " " + axisName [i]);
				newData = true;
			}
			data.axes [i] = val;
		}


		for (int i = 0; i < buttonKeys.Length; i++) {
			if(Input.GetKey(buttonKeys[i])){
				data.buttons [i] = true;
				newData = true;
			}
		}

		if(newData){
			im.PassInput (data);
			newData = false;
			data.Reset ();
		}
	}
}