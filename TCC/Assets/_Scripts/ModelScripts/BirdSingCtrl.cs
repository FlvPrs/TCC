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
	public StaccatoInteractionsCtrl singleNoteCollider;

	private HeightState oldState;

	private WalkingController playerCtrl;

	private int[] partiturasPossiveis;
	private string partituraAtual = "";
	private float delayBetweenNotes = 2f;
	private float cooldown = 0f;

	//TODO: Confirmar com Uiris
	private float singleNoteMinimumDuration = 0.5f;

	private AudioSource clarinet;
	public Material playerMat;
	private float currentAir = 5f;
	private float maxAir = 10f;
	private bool tocouPartitura;

	void Awake () {
		sustainCollider.gameObject.SetActive (false);
		singleNoteCollider.gameObject.SetActive (false);

		clarinet = GetComponent<AudioSource> ();
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
//		if(clarinet.isPlaying && cu	rrentAir > 0){
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
		oldState = playerCtrl.walkStates.CURR_HEIGHT_STATE;

		singleNoteCollider.currentHeight = oldState;

		//Quando o jogador tocar varias notas rapidamente, a seguinte linha deixa um
		//intervalo minimo de ~0.05s que o collider fica inativo entre cada nota
		singleNoteCollider.gameObject.SetActive (false);

		StopCoroutine ("StopSingleNote");

//		if(!clarinet.isPlaying)
//			clarinet.Play ();

		if(!tocouPartitura)
			UpdatePartituraAtual (oldState);
		
		StartCoroutine("StopSingleNote");
	}

	IEnumerator StopSingleNote(){
		yield return new WaitForSeconds (0.05f);
		singleNoteCollider.gameObject.SetActive (true);

		yield return new WaitForSeconds (singleNoteMinimumDuration - 0.05f);
		//clarinet.Stop ();
		playerCtrl.walkStates.TOCANDO_NOTAS = false;
		playerCtrl.canStartSing = true;
		singleNoteCollider.gameObject.SetActive (false);
	}

	public void SustainNote(){
		StopCoroutine ("StopSingleNote");

		oldState = playerCtrl.walkStates.CURR_HEIGHT_STATE;

		sustainCollider.currentHeight = oldState;
		sustainCollider.gameObject.SetActive (false);
		//StopCoroutine ("StopSustain");

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
	Empty,
	Amizade,
	Estorvo,
	Serenidade,
	Ninar,
	Crescimento,
	Encolhimento,
	Alegria
}