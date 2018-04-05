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
//	public SustainInteractionsCtrl sustainCollider;
	public StaccatoInteractionsCtrl singleNoteCollider;
	public PlayerSongInteractionsCtrl partituraCollider;

	public NoteParts[] noteSounds; //0 Default, 1 High, 2 Low

	private HeightState oldState;

	private WalkingController playerCtrl;

	private int[] partiturasPossiveis;
	private string partituraAtual = "";
	private float delayBetweenNotes = 1f;
	private float cooldown = 0f;

	//TODO: Confirmar com Uiris
	private float singleNoteMinimumDuration = 0.3f;

	private AudioSource audioSourceAttack;
	private AudioSource audioSourceSustain;
	private AudioSource audioSourceRelease;
	public Material playerMat;
	private float currentAir = 5f;
	private float maxAir = 10f;
	private bool tocouPartitura;

	private bool partituraIsSustain = false;

	void Awake () {
//		sustainCollider.gameObject.SetActive (false);
//		singleNoteCollider.gameObject.SetActive (false);

		partituraCollider.gameObject.SetActive (true);

		audioSourceAttack = GetComponent<AudioSource> ();
		audioSourceSustain = GetComponentsInChildren<AudioSource> ()[1];
		audioSourceSustain.loop = true;
		audioSourceRelease = GetComponentsInChildren<AudioSource> ()[2];

		playerCtrl = GetComponent<WalkingController> ();


		// Amizade	-	-	(Seguir)	-	UPDATE
		// Estorvo	-	-	(Irritar)	-	Start
		// Serenidade	-	(Acalmar)	-	Start
		// Ninar	-	-	(Dormir)	-	Start
		// Crescimento	-	(Crescer)	-	UPDATE
		// Encolhimento	-	(Encolher)	-	UPDATE
		// Alegria	-	-	(Distrair)	-	Start

		partiturasPossiveis = new int[]{ 
			4, 		//Encolhimento
			5, 		//Amizade
			6, 		//Crescimento
			1111,  	//Estorvo
			0000,	//Serenidade
			0000,	//Ninar
			0000	//Alegria
		};
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

		if (!tocouPartitura) {
			if (cooldown > 0f) {
				cooldown -= Time.deltaTime;
			} else {
				cooldown = 0;
				CancelaPartitura ();
			}
		}

		UpdateColor ();

		//print (partituraAtual);
	}

	public void SingNote(){
		StopCoroutine ("StopSingleNote");

		oldState = playerCtrl.walkStates.CURR_HEIGHT_STATE;

		partituraCollider.isSingingSomething = true;

		singleNoteCollider.currentHeight = oldState;

		//Quando o jogador tocar varias notas rapidamente, a seguinte linha deixa um
		//intervalo minimo de ~0.05s que o collider fica inativo entre cada nota
		singleNoteCollider.gameObject.SetActive (false);

		//if(!clarinet.isPlaying)
		audioSourceAttack.clip = noteSounds [(int)oldState].attack;
		audioSourceAttack.Play ();

		if (tocouPartitura)
			CancelaPartitura ();
		
		UpdatePartituraAtual (oldState);
		
		StartCoroutine("StopSingleNote");
	}

	IEnumerator StopSingleNote(){
		yield return new WaitForSeconds (0.05f);
		singleNoteCollider.gameObject.SetActive (true);

		yield return new WaitForSeconds (singleNoteMinimumDuration - 0.1f);
		audioSourceAttack.Stop ();
		audioSourceRelease.clip = noteSounds [(int)oldState].release;
		audioSourceRelease.Play ();

		playerCtrl.walkStates.TOCANDO_NOTAS = false;
		playerCtrl.canStartSing = true;
		partituraCollider.isSingingSomething = false;
	}

	public void SustainNote(){
		StopCoroutine ("StopSingleNote");
		StopCoroutine ("StopSustain");

		oldState = playerCtrl.walkStates.CURR_HEIGHT_STATE;

		partituraCollider.isSingingSomething = true;

//		sustainCollider.currentHeight = oldState;
//		sustainCollider.gameObject.SetActive (false);

		//audioSourceAttack.Stop ();
		audioSourceSustain.clip = noteSounds [(int)oldState].loop;
		audioSourceSustain.Play ();

		if (tocouPartitura)
			CancelaPartitura ();
		
		UpdatePartituraAtual (oldState, true);

		StartCoroutine("StopSustain");
	}

	IEnumerator StopSustain(){
//		yield return new WaitForSeconds (0.05f);
//		singleNoteCollider.gameObject.SetActive (false);
//		sustainCollider.gameObject.SetActive (true);

		while (playerCtrl.walkStates.SEGURANDO_NOTA) {
			yield return new WaitForSeconds (0.05f);
		}
		audioSourceSustain.Stop ();
		audioSourceRelease.clip = noteSounds [(int)oldState].release;
		audioSourceRelease.Play ();

		playerCtrl.walkStates.TOCANDO_NOTAS = false;
		playerCtrl.canStartSing = true;

		partituraCollider.isSingingSomething = false;

		if(tocouPartitura && partituraIsSustain)
			CancelaPartitura ();
	}

	public void UpdateColor(){
		//playerMat.color = Color.black;
		playerMat.color = hpColor.Evaluate (currentAir / maxAir);
	}

	void UpdatePartituraAtual(HeightState state, bool holding = false){
		cooldown = delayBetweenNotes;

		if(holding){
			//Se a nota anterior for staccato, desconsidere-a
			if(partituraAtual.Length > 0 && (string.Equals(partituraAtual[partituraAtual.Length - 1].ToString(), "1") || string.Equals(partituraAtual[partituraAtual.Length - 1].ToString(), "2") || string.Equals(partituraAtual[partituraAtual.Length - 1].ToString(), "3")))
				partituraAtual = partituraAtual.Remove (partituraAtual.Length - 1, 1);

//			//Se eu quiser desconsiderar todas as notas caso esta não seja a ultima nota da partitura...
//			if (partituraAtual.Length > 0 && partituraAtual.Length < 3)
//				partituraAtual = "";

			switch (state) {
			case HeightState.Low:
				partituraAtual += "4";
				break;
			case HeightState.Default:
				partituraAtual += "5";
				break;
			case HeightState.High:
				partituraAtual += "6";
				break;

			default:
				break;
			}
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

		if (partituraAtual.Length > 4)
			partituraAtual = partituraAtual.Remove (0, 1);

		int num = int.Parse(partituraAtual);
		for (int i = 0; i < partiturasPossiveis.Length; i++) {
			if(num == partiturasPossiveis[i]){
				TocarPartitura (num);
				break;
			}
		}
		print (num);
	}

	void TocarPartitura(int partitura){
		tocouPartitura = true;
		print ("YAYYY " + partitura);

		switch (partitura) {

		//Se a partitura for de sustain, espera parar de tocar para cancelar partitura.
		case 4:	//Encolhimento
			partituraCollider.currentSong = PlayerSongs.Encolhimento;
			partituraIsSustain = true;
			return;
		case 5:	//Amizade
			partituraCollider.currentSong = PlayerSongs.Amizade;
			partituraIsSustain = true;
			return;
		case 6:	//Crescimento
			partituraCollider.currentSong = PlayerSongs.Crescimento;
			partituraIsSustain = true;
			return;

		//Se a partitura for só de staccato, cancela a partitura depois de 0.5s.
		case 1111:	//Estorvo
			partituraCollider.currentSong = PlayerSongs.Estorvo;
			break;
		case 0000:	//Serenidade
			partituraCollider.currentSong = PlayerSongs.Serenidade;
			break;
		case 8888:	//Ninar
			partituraCollider.currentSong = PlayerSongs.Ninar;
			break;
		case 9999:	//Alegria
			partituraCollider.currentSong = PlayerSongs.Alegria;
			break;
		default:
			break;
		}

		Invoke ("CancelaPartitura", 0.5f);
	}

	void CancelaPartitura (){
		partituraAtual = "";
		tocouPartitura = false;
		partituraIsSustain = false;
		partituraCollider.currentSong = PlayerSongs.Empty;

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