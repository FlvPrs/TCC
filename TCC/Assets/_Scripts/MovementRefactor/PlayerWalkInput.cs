using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkInput : MonoBehaviour {

	public bool UIRIS_INPUTS;

	public WalkingController player;

	float jumpPressTime;
	float floreioHoldTime;
	float staccatoHoldTime;

	bool holdingJump;

	bool holdingSing;

	[HideInInspector]
	public float singPressTime = 0f;

	void Start() {
		holdingJump = false;
	}

	void Update () {
		if (!player.playerCanMove)
			return;

		#region Check Movement
		Vector3 directionalInput = Vector3.zero;

		//Set vertical movement
		directionalInput.z += Input.GetAxisRaw("L_Joystick_Y");
		directionalInput.z += Input.GetAxisRaw("Vertical");
		if(UIRIS_INPUTS)
			directionalInput.x = Input.GetAxisRaw("VerticalUiris");

		//Set horizontal movement
		directionalInput.x += Input.GetAxisRaw("L_Joystick_X");
		directionalInput.x += Input.GetAxisRaw("Horizontal");

		player.SetDirectionalInput (directionalInput);
		#endregion

		#region Check Sanfona
		//Set Sanfona
		float sanfonaStrength = Input.GetAxisRaw("R_Joystick_Y") + Input.GetAxisRaw("SanfonaAxis");
		sanfonaStrength = Mathf.Clamp(sanfonaStrength, -1f, 1f);
		player.SetSanfonaStrength(sanfonaStrength);
		#endregion

		#region check Jump
		//Set Jump
		bool pressedJump = false;
		if(Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Space)){
			player.OnJumpInputDown ();
			pressedJump = true;
		}

		if(pressedJump && !holdingJump){
			holdingJump = true;
		}
		if(!pressedJump && holdingJump) {
			player.OnJumpInputHold ();
		}

		if(Input.GetKeyUp(KeyCode.JoystickButton0) || Input.GetKeyUp(KeyCode.Space)){
			player.OnJumpInputUp ();
			pressedJump = holdingJump = false;
		}
		#endregion

		#region Check Sing
		//Check Sing
		if(Input.GetAxis("R_Trigger") != 0 || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.LeftAlt)){
			singPressTime += Time.deltaTime;
		} else {
//			if (singPressTime > 0){ //Se eu estava segurando no frame anterior...
//				player.OnSingInputUp ();
//			}
			singPressTime = 0f;
			holdingSing = false;
			player.walkStates.SEGURANDO_NOTA = false;
		}

		if(singPressTime > 0f && player.canStartSing && singPressTime < player.singHoldTreshold) {
			player.canStartSing = false;
			player.OnSingInputDown ();
		} 
		else if (singPressTime > 0f && player.walkStates.TOCANDO_NOTAS) {
			if(singPressTime >= player.singHoldTreshold && !holdingSing){
				holdingSing = true;
				player.OnSingInputHold ();
			}
		}
		player.singHoldTime = singPressTime;
		#endregion

		#region Check Cheat
		if(Input.GetKeyDown(KeyCode.PageUp)){
			player.SetCheatState(true);
		}
		if(Input.GetKeyDown(KeyCode.PageDown)){
			player.SetCheatState(false);
		}
		#endregion
	}
}