using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkInput : MonoBehaviour {

	public bool UIRIS_INPUTS;

	public WalkingController player;

	float jumpPressTime;
	float glideTimeThreshold = 0.2f;

	bool holdingJump;

	bool holdingSing;

	[HideInInspector]
	public float singPressTime = 0f;

	public bool disableMovement = false;

	void Start() {
		jumpPressTime = 0f;
		holdingJump = false;
	}

	void Update () {
		if (!player.playerCanMove || player.eatenByCarnivora) {
			player.SetDirectionalInput (Vector3.zero);
			player.SetSanfonaStrength (0f);
			if(holdingJump){
				player.OnJumpInputUp ();
				holdingJump = false;
			}
			if(holdingSing){
				singPressTime = 0f;
				holdingSing = false;
				player.walkStates.SEGURANDO_NOTA = false;
			}

			if(player.playerCanCanOnlySing){
				if(Input.GetAxis("R_Trigger") != 0 || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.LeftAlt)){
					singPressTime += Time.deltaTime;
				} else if(Input.GetAxis("R_Trigger") == 0 || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.LeftAlt)) {
					singPressTime = 0f;
					holdingSing = false;
					player.walkStates.TOCANDO_NOTAS = false;
					player.OnSingInputUp ();
				}

				if(singPressTime > 0f && player.canStartSing) {
					player.walkStates.TOCANDO_NOTAS = true;
					player.canStartSing = false;
					player.OnSingInputDown ();
				} 
			}

			return;
		}
			
		if (!disableMovement) {
			#region Check Movement
			Vector3 directionalInput = Vector3.zero;

			//Set vertical movement

			directionalInput.z += Input.GetAxisRaw ("L_Joystick_Y");
		
			directionalInput.z += Input.GetAxisRaw ("Vertical");
			if (UIRIS_INPUTS){
				directionalInput.y = Input.GetAxisRaw ("VerticalUiris");

			}
			//Set horizontal movement
			directionalInput.x += Input.GetAxisRaw ("L_Joystick_X");
			directionalInput.x += Input.GetAxisRaw ("Horizontal");

			player.SetDirectionalInput (directionalInput);
			#endregion
		} else {
			player.SetDirectionalInput (Vector3.zero);
		}

		#region Check Sanfona
		//Set Sanfona
		float sanfonaStrength = Input.GetAxisRaw("R_Joystick_Y") + Input.GetAxisRaw("SanfonaAxis");
		sanfonaStrength = Mathf.Clamp(sanfonaStrength, -1f, 1f);
		player.SetSanfonaStrength(sanfonaStrength);
		#endregion

		//if (!disableMovement) {
			#region check Jump
			//Set Jump
			bool pressedJump = false;
			if (Input.GetKeyDown (KeyCode.JoystickButton0) || Input.GetKeyDown (KeyCode.Space)) {
				player.OnJumpInputDown ();
				pressedJump = true;
			}

			if (pressedJump && !holdingJump) {
				holdingJump = true;
				jumpPressTime = 0f;
			}
			if (!pressedJump && holdingJump) {
				if (jumpPressTime > glideTimeThreshold)
					player.OnJumpInputHold ();
				else
					jumpPressTime += Time.deltaTime;
			}

			if (Input.GetKeyUp (KeyCode.JoystickButton0) || Input.GetKeyUp (KeyCode.Space)) {
				player.OnJumpInputUp ();
				pressedJump = holdingJump = false;
				jumpPressTime = 0f;
			}
			#endregion
		//}


		#region Check Sing
		//Check Sing
		if(Input.GetAxis("R_Trigger") != 0 || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.LeftAlt)){
			singPressTime += Time.deltaTime;
		} else if(Input.GetAxis("R_Trigger") == 0 || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.LeftAlt)) {
			singPressTime = 0f;
			holdingSing = false;
			player.walkStates.TOCANDO_NOTAS = false;
			player.walkStates.SEGURANDO_NOTA = false;
			CancelInvoke("SegurandoNota");
			player.OnSingInputUp ();
		}

		if(singPressTime > 0f && player.canStartSing) {
			player.canStartSing = false;
			player.OnSingInputDown ();
			Invoke("SegurandoNota", player.singHoldTreshold);
		} 
//		else if (singPressTime > 0f && player.walkStates.TOCANDO_NOTAS) {
//			if(singPressTime >= player.singHoldTreshold && !holdingSing){
//				holdingSing = true;
//				player.OnSingInputHold ();
//			}
//		}

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

	void SegurandoNota (){
		holdingSing = true;
		player.OnSingInputHold ();
	}
}