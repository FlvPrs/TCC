using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControllerInGame : MonoBehaviour {

	public WalkingController player;
	private bool onPause, onMenu;
	public GameObject menu1, menu2, menu3, menu4, menuPause, menuMorte;
	// Use this for initialization
	void Start () {
		onMenu = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.JoystickButton7)&& !onMenu) {
			onPause = true;
		}

		if (onMenu) {
			player.playerCanMove = false;
		} else {
			player.playerCanMove = false;
		}

		if (onPause) {
			player.playerCanMove = false;
		} else {
			player.playerCanMove = true;
		}
	}

	void StartGame(){
		
	}
}
