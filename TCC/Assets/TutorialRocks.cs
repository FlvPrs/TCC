using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialRocks : MonoBehaviour, ISongListener {
	public Text textoTutorial;
	public GameObject BlackBoxTutorial;
	public bool tutorialAtivo;
	public string texto;
	private float timerPressT;
	private bool startTimer;

	float listenCooldown = 0f;

	bool playerInRange = false;

	// Use this for initialization
	void Start () {
		DesativaTutorial ();
	}

	// Update is called once per frame
	void Update () {
		if (listenCooldown > 0f)
			listenCooldown -= Time.deltaTime;
		else
			listenCooldown = 0f;
	}

	public void DetectSong (PlayerSongs song, bool isSingingSomething, bool isFather = false, HeightState height = HeightState.Default){
		if (!playerInRange)
			return;
		
		if(isSingingSomething && listenCooldown == 0f){
			if (isFather)
				return;

			listenCooldown = 1f;

			tutorialAtivo = !tutorialAtivo;
			if (tutorialAtivo) {
				AtivaTutorial ();
			} else {
				DesativaTutorial ();
			}
		}
	}

	public void AtivaTutorial(){
		//tutorialAtivo = true;
		BlackBoxTutorial.SetActive (true);
		textoTutorial.text = texto;
	}

	public void DesativaTutorial(){
		tutorialAtivo = false;
		BlackBoxTutorial.SetActive (false);
		textoTutorial.text = "";
	}

	void OnTriggerStay(Collider colisor){
		if (colisor.CompareTag("Player")) {
			playerInRange = true;
		}
	}
	void OnTriggerExit(Collider colisor){
		if (colisor.CompareTag("Player")) {
			playerInRange = false;
			HideTxt ();
		}
	}

	void HideTxt (){
		DesativaTutorial ();
		listenCooldown = 0f;
	}
}
