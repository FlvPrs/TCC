using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Receives the info from the keyboardTracker;
//And decides what it means and which controller it should go to;
//i. e., acting as a middle man, passing that info to Controller
public class InputManager : MonoBehaviour {

	//SIMPLE SINGLETON PATTERN
	public static InputManager ins;

	void Awake(){
		ins = this;
	}

	[Range(0, 10)]
	public int axisCount;
	[Range(0, 20)]
	public int buttonCount;

	public Controller controller;

	public void PassInput(InputData data){
		controller.ReadInput (data);
	}

	public void RefreshTracker(){
		DeviceTracker[] dt = GetComponents<DeviceTracker> ();
		for (int i = 0; i < dt.Length; i++) {
			if(dt[i] != null){
				dt[i].Refresh ();
			}
		}

	}

}

public struct InputData{

	public float[] axes;
	public bool[] buttons;

	public InputData(int axisCount_data, int buttonCount_data){
		axes = new float[axisCount_data];
		buttons = new bool[buttonCount_data];
	}

	public void Reset(){
		for (int i = 0; i < axes.Length; i++) {
			axes [i] = 0f;
		}

		for (int i = 0; i < buttons.Length; i++) {
			buttons [i] = false;
		}
	}

}