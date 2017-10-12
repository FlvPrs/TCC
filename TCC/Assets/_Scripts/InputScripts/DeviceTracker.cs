using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public abstract class DeviceTracker : MonoBehaviour {

	protected InputManager im;
	protected InputData data;
	protected bool newData;

	void Awake(){
		im = GetComponent<InputManager> ();
		data = new InputData (im.axisCount, im.buttonCount);
	}
		
	public abstract void Refresh();

}

[System.Serializable]
public struct AxisKeys{
	public KeyCode positive;
	public KeyCode negative;
}

[System.Serializable]
public struct JoystickAxisKeys{
	public JoystickAxes axis;
}

public enum JoystickAxes
{
	L_Horizontal_Joystick,
	L_Vertical_Joystick,
	R_Horizontal_Joystick,
	R_Vertical_Joystick,
	L_Trigger,
	R_Trigger
}