using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//LEGENDA
//0 - Abaixado
//1 - Normal
//2 - Esticado

[RequireComponent(typeof(WalkingController))]
public class BirdSingCtrl : MonoBehaviour {

	public Gradient hpColor;

	private HeightState oldState;

	private WalkingController playerCtrl;

	private int[] partiturasPossiveis;
	private bool[] partiturasConhecidas;
	private string partituraAtual = "";
	private float delayBetweenNotes = 2f;
	private float cooldown = 0f;

	private AudioSource clarinet;
	//public Material playerMat;
	private float currentAir = 5f;
	private float maxAir = 10f;

	void Awake () {
		clarinet = GetComponent<AudioSource> ();
		//playerMat = GetComponentInChildren<MeshRenderer> ().material;
		playerCtrl = GetComponent<WalkingController> ();

		partiturasPossiveis = new int[]{ 121, 123, 131, 132, 212, 213, 231, 232, 312, 313, 321, 323 };
		partiturasConhecidas = new bool[partiturasPossiveis.Length]; //se não definir, bool permanece false por default.

		partiturasConhecidas [1] = true;
	}


	void Update () {
		if(playerCtrl.walkStates.TOCANDO_NOTA){
			if(oldState != playerCtrl.walkStates.CURR_HEIGHT_STATE){
				oldState = playerCtrl.walkStates.CURR_HEIGHT_STATE;
				UpdatePartituraAtual (oldState);
			}
		}

		if(clarinet.isPlaying && currentAir > 0){
			currentAir -= Time.deltaTime;
		} else if(currentAir < maxAir/2f) {
			currentAir += Time.deltaTime;
		}

		if (cooldown > 0f) {
			cooldown -= Time.deltaTime;
		} else {
			partituraAtual = "";
		}

		UpdateColor ();

		//print (partituraAtual);
	}

	public void StartClarinet(bool start, float volume){
		if (start && currentAir >= maxAir/4f) { //ou seja, se ainda tiver 25% de ar disponível
			playerCtrl.walkStates.TOCANDO_NOTA = true;
			clarinet.Play ();
			oldState = playerCtrl.walkStates.CURR_HEIGHT_STATE;
			UpdatePartituraAtual (oldState);
		} else {
			playerCtrl.walkStates.TOCANDO_NOTA = false;
			clarinet.Stop ();
		}

		UpdateSoundVolume (volume);
	}

	public void UpdateSoundVolume(float volume){
		clarinet.volume = volume;
	}

	public void UpdateColor(){
		//playerMat.color = hpColor.Evaluate (currentAir / maxAir);
	}

	void UpdatePartituraAtual(HeightState state){
		cooldown = delayBetweenNotes;

		switch (state) {
		case HeightState.Low:
			partituraAtual += "1";
			break;
		case HeightState.Default:
			partituraAtual += "2";
			break;
		case HeightState.High:
			partituraAtual += "3";
			break;

		default:
			break;
		}

		if (partituraAtual.Length > 3)
			partituraAtual = partituraAtual.Remove (0, 1);

		int num = int.Parse(partituraAtual);
		for (int i = 0; i < partiturasPossiveis.Length; i++) {
			if(num == partiturasPossiveis[i]){
				if (partiturasConhecidas [i] == true) {
					TocarPartitura (num);
				}
				break;
			}
		}
	}

	void TocarPartitura(int partitura){
		print ("YAYYY " + partitura);
	}
}

public enum HeightState {
	High,
	Default,
	Low
}