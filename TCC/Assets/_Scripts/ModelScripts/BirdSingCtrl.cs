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
	public SustainInteractionsCtrl sustainCollider;
	public StaccatoInteractionsCtrl partituraCollider;

	private HeightState oldState;

	private WalkingController playerCtrl;

	private int[] partiturasPossiveis;
	private bool[] partiturasConhecidas;
	private string partituraAtual = "";
	private float delayBetweenNotes = 2f;
	private float cooldown = 0f;

	private AudioSource clarinet;
	public Material playerMat;
	private float currentAir = 5f;
	private float maxAir = 10f;
	private bool tocouPartitura;

	void Awake () {
		sustainCollider.gameObject.SetActive (false);
		partituraCollider.gameObject.SetActive (false);

		clarinet = GetComponent<AudioSource> ();
		playerCtrl = GetComponent<WalkingController> ();

		partiturasPossiveis = new int[]{ 121, 123, 131, 132, 212, 213, 231, 232, 312, 313, 321, 323 };
		partiturasConhecidas = new bool[partiturasPossiveis.Length]; //se não definir, bool permanece false por default.

		partiturasConhecidas [1] = true;
	}


	void Update () {
		if(playerCtrl.walkStates.TOCANDO_STACCATO){
			if(oldState != playerCtrl.walkStates.CURR_HEIGHT_STATE){
				oldState = playerCtrl.walkStates.CURR_HEIGHT_STATE;
				UpdatePartituraAtual (oldState);
			}
		}
		if(playerCtrl.walkStates.TOCANDO_SUSTAIN){
			if(oldState != playerCtrl.walkStates.CURR_HEIGHT_STATE){
				oldState = playerCtrl.walkStates.CURR_HEIGHT_STATE;
				sustainCollider.currentHeight = oldState;
			}
		}

		if(clarinet.isPlaying && currentAir > 0){
			currentAir -= Time.deltaTime;
		} else if(!clarinet.isPlaying && currentAir < maxAir/2f) {
			currentAir += Time.deltaTime;
		} else if(clarinet.isPlaying) {
			playerCtrl.walkStates.TOCANDO_SUSTAIN = false;
			clarinet.Stop ();
			sustainCollider.gameObject.SetActive (false);
		}

		if (cooldown > 0f) {
			cooldown -= Time.deltaTime;
		} else {
			partituraAtual = "";
			tocouPartitura = false;
			partituraCollider.gameObject.SetActive (false);
		}

		UpdateColor ();

		//print (partituraAtual);
	}

	public void StartClarinet_Staccato(){

		if(!clarinet.isPlaying)
			clarinet.Play ();

		oldState = playerCtrl.walkStates.CURR_HEIGHT_STATE;

		if(!tocouPartitura)
			UpdatePartituraAtual (oldState);
		
		StartCoroutine("StopStaccato");
	}

	IEnumerator StopStaccato(){
		yield return new WaitForSeconds (0.2f);
		clarinet.Stop ();
		playerCtrl.walkStates.TOCANDO_STACCATO = false;
	}

	public void StartClarinet_Sustain(bool start, float volume){
		if (start && currentAir >= maxAir/4f) { //ou seja, se ainda tiver 25% de ar disponível
			playerCtrl.walkStates.TOCANDO_SUSTAIN = true;
			clarinet.Play ();
			oldState = playerCtrl.walkStates.CURR_HEIGHT_STATE;
			sustainCollider.currentHeight = oldState;
			sustainCollider.gameObject.SetActive (true);
		} else {
			playerCtrl.walkStates.TOCANDO_SUSTAIN = false;
			clarinet.Stop ();
			sustainCollider.gameObject.SetActive (false);
		}

		UpdateSoundVolume (volume);
	}

	public void UpdateSoundVolume(float volume){
		clarinet.volume = volume;
	}

	public void UpdateColor(){
		//playerMat.color = Color.black;
		playerMat.color = hpColor.Evaluate (currentAir / maxAir);
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
		tocouPartitura = true;
		print ("YAYYY " + partitura);
		partituraCollider.partitura = partitura;
		partituraCollider.gameObject.SetActive (true);
	}
}

public enum HeightState {
	High,
	Default,
	Low
}