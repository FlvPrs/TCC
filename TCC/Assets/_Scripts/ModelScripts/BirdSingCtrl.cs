using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//LEGENDA
//1 - Abaixado
//2 - Normal
//3 - Esticado

[System.Serializable]
public struct NoteParts
{
	public AudioClip attack;
	public AudioClip loop;
	public AudioClip release;
}

[RequireComponent(typeof(WalkingController))]
public class BirdSingCtrl : MonoBehaviour {

	public Gradient hpColor;
	public SustainInteractionsCtrl sustainCollider;
	public StaccatoInteractionsCtrl singleNoteCollider;

	public NoteParts[] noteSounds; //0 Default, 1 High, 2 Low

	private HeightState oldState;

	private WalkingController playerCtrl;

	private int[] partiturasPossiveis;
	private string partituraAtual = "";
	private float delayBetweenNotes = 2f;
	private float cooldown = 0f;

	//TODO: Confirmar com Uiris
	private float singleNoteMinimumDuration = 0.29f;

	private AudioSource audioSourceAttack;
	private AudioSource audioSourceSustain;
	private AudioSource audioSourceRelease;
	public Material playerMat;
	private float currentAir = 5f;
	private float maxAir = 10f;
	private bool tocouPartitura;

	void Awake () {
		sustainCollider.gameObject.SetActive (false);
		singleNoteCollider.gameObject.SetActive (false);

		audioSourceAttack = GetComponent<AudioSource> ();
		audioSourceSustain = GetComponentsInChildren<AudioSource> ()[1];
		audioSourceSustain.loop = true;
		audioSourceRelease = GetComponentsInChildren<AudioSource> ()[2];

		playerCtrl = GetComponent<WalkingController> ();

		partiturasPossiveis = new int[]{ 1, 10, 20, 30 };
	}


	void Update () {
//		if (playerCtrl.walkStates.TOCANDO_NOTAS) {
//			if (oldState != playerCtrl.walkStates.CURR_HEIGHT_STATE) {
//				oldState = playerCtrl.walkStates.CURR_HEIGHT_STATE;
//				UpdatePartituraAtual (oldState);
//			}
//		}

		//TODO: Confirmar se mantem isso, entao Testar
//		if(clarinet.isPlaying && currentAir > 0){
//			currentAir -= Time.deltaTime;
//		} else if(!clarinet.isPlaying && currentAir < maxAir/2f) {
//			currentAir += Time.deltaTime;
//		} else if(clarinet.isPlaying) {
//			playerCtrl.walkStates.SEGURANDO_NOTA = false;
//			playerCtrl.walkStates.TOCANDO_NOTAS = false;
//			//clarinet.Stop ();
//			sustainCollider.gameObject.SetActive (false);
//		} else {
//			playerCtrl.canStartSing = true;
//		}

		if (cooldown > 0f) {
			cooldown -= Time.deltaTime;
		}else {
			cooldown = 0;
			CancelaPartitura ();
		}

		UpdateColor ();

		//print (partituraAtual);
	}

	public void SingNote(){
		StopCoroutine ("StopSingleNote");

		oldState = playerCtrl.walkStates.CURR_HEIGHT_STATE;

		singleNoteCollider.currentHeight = oldState;

		//Quando o jogador tocar varias notas rapidamente, a seguinte linha deixa um
		//intervalo minimo de ~0.05s que o collider fica inativo entre cada nota
		singleNoteCollider.gameObject.SetActive (false);

		//if(!clarinet.isPlaying)
		audioSourceAttack.clip = noteSounds [(int)oldState].attack;
		audioSourceAttack.Play ();

		if(!tocouPartitura)
			UpdatePartituraAtual (oldState);
		
		StartCoroutine("StopSingleNote");
	}

	IEnumerator StopSingleNote(){
		yield return new WaitForSeconds (0.05f);
		singleNoteCollider.gameObject.SetActive (true);

		yield return new WaitForSeconds (singleNoteMinimumDuration - 0.1f);
		//audioSourceStaccato.Stop ();
		audioSourceRelease.clip = noteSounds [(int)oldState].release;
		audioSourceRelease.Play ();

		playerCtrl.walkStates.TOCANDO_NOTAS = false;
		playerCtrl.canStartSing = true;
		singleNoteCollider.gameObject.SetActive (false);
	}

	public void SustainNote(){
		StopCoroutine ("StopSingleNote");
		StopCoroutine ("StopSustain");

		oldState = playerCtrl.walkStates.CURR_HEIGHT_STATE;

		sustainCollider.currentHeight = oldState;
		sustainCollider.gameObject.SetActive (false);

		audioSourceAttack.Stop ();
		audioSourceSustain.clip = noteSounds [(int)oldState].loop;
		audioSourceSustain.Play ();

		if(!tocouPartitura)
			UpdatePartituraAtual (oldState, true);

		StartCoroutine("StopSustain");
	}

	IEnumerator StopSustain(){
		yield return new WaitForSeconds (0.05f);
		singleNoteCollider.gameObject.SetActive (false);
		sustainCollider.gameObject.SetActive (true);

		while (playerCtrl.walkStates.SEGURANDO_NOTA) {
			yield return new WaitForSeconds (0.05f);
		}
		audioSourceSustain.Stop ();
		audioSourceRelease.clip = noteSounds [(int)oldState].release;
		audioSourceRelease.Play ();

		playerCtrl.walkStates.TOCANDO_NOTAS = false;
		playerCtrl.canStartSing = true;
		sustainCollider.gameObject.SetActive (false);
	}

	public void UpdateColor(){
		//playerMat.color = Color.black;
		playerMat.color = hpColor.Evaluate (currentAir / maxAir);
	}

	void UpdatePartituraAtual(HeightState state, bool holding = false){
		cooldown = delayBetweenNotes;

		if(holding){
			switch (state) {
			case HeightState.Low:
				partituraAtual = "1";
				break;
			case HeightState.Default:
				partituraAtual = "2";
				break;
			case HeightState.High:
				partituraAtual = "3";
				break;

			default:
				break;
			}

			partituraAtual += "0";

		} else {
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
		}

		if (partituraAtual.Length > 3)
			partituraAtual = partituraAtual.Remove (0, 1);

		int num = int.Parse(partituraAtual);
		for (int i = 0; i < partiturasPossiveis.Length; i++) {
			if(num == partiturasPossiveis[i]){
				//if (partiturasConhecidas [i] == true) {
					TocarPartitura (num);
				//}
				break;
			}
		}
		print (num);
	}

	void TocarPartitura(int partitura){
		tocouPartitura = true;
		print ("YAYYY " + partitura);
		singleNoteCollider.partitura = partitura;
		singleNoteCollider.gameObject.SetActive (true);
		Invoke ("CancelaPartitura", 0.5f);
	}

	void CancelaPartitura (){
		partituraAtual = "";
		tocouPartitura = false;
		singleNoteCollider.gameObject.SetActive (false);
	}
}

public enum PlayerSongs {
	Empty			= 0x00,
	Amizade			= 1<<0,
	Estorvo			= 1<<1,
	Serenidade		= 1<<2,
	Ninar			= 1<<3,
	Crescimento		= 1<<4,
	Encolhimento	= 1<<5,
	Alegria			= 1<<6
}