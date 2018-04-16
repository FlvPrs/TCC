using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlantaBehaviour : MonoBehaviour, ISongListener {

	protected Transform plantaTransform;
	protected Transform player;
	protected Transform father;

	[BitMaskAttribute(typeof(PlayerSongs))]
	public PlayerSongs acceptedSongs; //Definir pelo inspector com quais melodias o NPC poderá interagir.
	protected List<int> selectedSongs; //Armazena separadamente cada uma das melodias escolhidas em acceptedSongs. Ou seja, ela me permite saber quais melodias um NPC pode interagir.


	// (0) Amizade	-	-	(Seguir)	-	UPDATE
	// (1) Estorvo	-	-	(Irritar)	-	Start
	// (2) Serenidade	-	(Acalmar)	-	Start
	// (3) Ninar	-	-	(Dormir)	-	Start
	// (4) Crescimento	-	(Crescer)	-	UPDATE
	// (5) Encolhimento	-	(Encolher)	-	UPDATE
	// (6) Alegria	-	-	(Distrair)	-	Start

	[SerializeField]
	protected PlayerSongs currentSong;

	protected bool playerIsMakingNoise;

	[SerializeField]
	protected Planta_CurrentState currentState;

	protected Transform currentInteractionAgent; //Player ou Pai

	[HideInInspector]
	public float timer = 0f;

	public bool isMurcha;
	public bool isBroto;
	protected GameObject MDL_Broto, MDL_Crescida, MDL_Murcha;


	protected virtual void Awake () {
		selectedSongs = ReturnSelectedElements ();
		plantaTransform = GetComponent<Transform> ();
		player = GameObject.FindObjectOfType<WalkingController> ().transform;
		//father = GameObject.FindObjectOfType<FatherFSM> ().transform;

		MDL_Broto = plantaTransform.Find ("MDL_Broto").gameObject;
		MDL_Crescida = plantaTransform.Find ("MDL_Crescida").gameObject;
		MDL_Murcha = plantaTransform.Find ("MDL_Murcha").gameObject;

		if(isBroto){
			MDL_Broto.SetActive (true);
			MDL_Crescida.SetActive (false);
			MDL_Murcha.SetActive (false);
		} else if (!isMurcha) {
			MDL_Broto.SetActive (false);
			MDL_Crescida.SetActive (true);
			MDL_Murcha.SetActive (false);
		} else {
			MDL_Broto.SetActive (false);
			MDL_Crescida.SetActive (false);
			MDL_Murcha.SetActive (true);
		}

		currentSong = PlayerSongs.Empty;
		currentInteractionAgent = player;

		GetComponent<Rigidbody> ().isKinematic = true;
	}


	protected virtual void Update () {
		if (timer >= 1f) {
			timer = 1f;
			currentSong = PlayerSongs.Empty;
		} else {
			timer += Time.deltaTime;
		}

		if (!isBroto) { //Se a planta está crescida...
			if (!isMurcha) { //Se a planta não estiver Murcha...
				switch (currentSong) { //Listen to the songs
				case PlayerSongs.Amizade:
					if (selectedSongs.Contains (0))
						Seguir ();
					else
						DefaultState ();
					break;
				case PlayerSongs.Estorvo:
					if (selectedSongs.Contains (1))
						Irritar ();
					else
						DefaultState ();
					break;
				case PlayerSongs.Serenidade:
					if (selectedSongs.Contains (2))
						Acalmar ();
					else
						DefaultState ();
					break;
				case PlayerSongs.Ninar:
					if (selectedSongs.Contains (3))
						Dormir ();
					else
						DefaultState ();
					break;
				case PlayerSongs.Crescimento:
					if (selectedSongs.Contains (4))
						Crescer ();
					else
						DefaultState ();
					break;
				case PlayerSongs.Encolhimento:
					if (selectedSongs.Contains (5))
						Encolher ();
					else
						DefaultState ();
					break;
				case PlayerSongs.Alegria:
					if (selectedSongs.Contains (6))
						Distrair ();
					else
						DefaultState ();
					break;
				default: //PlayerSongs.Empty
					switch (currentState) {
					case Planta_CurrentState.Seguindo:
						Seguir ();
						break;
					case Planta_CurrentState.Dormindo:
						Dormir ();
						break;
					case Planta_CurrentState.Calmo:
						Acalmar ();
						break;
					case Planta_CurrentState.Irritado:
						Irritar ();
						break;
					case Planta_CurrentState.Distraido:
						Distrair ();
						break;
					default:
						DefaultState ();
						break;
					}
					break;
				}

				if(playerIsMakingNoise){
					ChamarAtencao ();
				}

			} else { //Se a planta estiver Murcha...
				MurchaState ();
			}
		} else { //Se a planta for um broto...
			if(currentSong == PlayerSongs.Crescimento){
				CrescerBroto ();
			}
		}
	}

	public void DetectSong (PlayerSongs song, bool isSingingSomething, bool isFather = false){
		timer = 0f;
		currentSong = song;
		playerIsMakingNoise = isSingingSomething;

		if (!isFather)
			currentInteractionAgent = player;
		else
			currentInteractionAgent = father;
	}

	//======================================================================================================================
	//=================------------------------- FUNÇÕES DE COMPORTAMENTO -------------------------=========================
	//======================================================================================================================

	protected virtual void CrescerBroto (){
		MDL_Broto.SetActive (false);
		MDL_Crescida.SetActive (true);
		MDL_Murcha.SetActive (false);
		isBroto = false;
		isMurcha = false;
	}

	//----------------------------------------------------------------------------------------------------------------------

	protected virtual void MurcharPlanta (){
		if(currentState != Planta_CurrentState.Murcho && !isBroto){
			currentState = Planta_CurrentState.Murcho;
			isMurcha = true;
		} else {
			return;
		}
	}

	//TODO: Esta função será chamada apenas pelo código da Fruta
	public virtual void RevigorarPlanta (){
		if(currentState == Planta_CurrentState.Murcho){
			currentState = Planta_CurrentState.DefaultState;
			isMurcha = false;
		} else {
			return;
		}
	}

	//----------------------------------------------------------------------------------------------------------------------

	protected virtual void DefaultState (){
		currentState = Planta_CurrentState.DefaultState;

	}

	protected virtual void MurchaState (){
		if (currentState != Planta_CurrentState.Murcho) {
			currentState = Planta_CurrentState.Murcho;
			MDL_Broto.SetActive (false);
			MDL_Crescida.SetActive (false);
			MDL_Murcha.SetActive (true);
		}
	}


	protected virtual void Seguir (){
		currentState = Planta_CurrentState.Seguindo;
	}
	protected virtual void PararDeSeguir (){
		if (currentState == Planta_CurrentState.DefaultState)
			return; //Esta função só deve roda uma vez.

		currentState = Planta_CurrentState.DefaultState;
	}

	protected virtual void Irritar (){
		if (currentState == Planta_CurrentState.Irritado)
			return; //Esta função só deve roda uma vez.

		currentState = Planta_CurrentState.Irritado;
		//Faz Irritar
	}
	protected virtual void Acalmar (){
		if (currentState == Planta_CurrentState.Calmo)
			return; //Esta função só deve roda uma vez.

		currentState = Planta_CurrentState.Calmo;
		//Faz Acalmar
	}

	protected virtual void Dormir (){
		if (currentState == Planta_CurrentState.Dormindo)
			return; //Esta função só deve roda uma vez.

		currentState = Planta_CurrentState.Dormindo;
		//Faz Dormir
	}
	protected virtual void Acordar (){
		if (currentState == Planta_CurrentState.DefaultState)
			return; //Esta função só deve roda uma vez.

		currentState = Planta_CurrentState.DefaultState;
	}

	protected virtual void Crescer (){
		currentState = Planta_CurrentState.Crescendo;
		//Faz Crescer
	}
	protected virtual void Encolher (){
		currentState = Planta_CurrentState.Encolhendo;
		//Faz Encolher
	}

	protected virtual void ChamarAtencao (){
		playerIsMakingNoise = false;
//		if (currentState == Planta_CurrentState.Atento)
//			return; //Esta função só deve roda uma vez.
//
//		currentState = Planta_CurrentState.Atento;
		if(currentState == Planta_CurrentState.Dormindo){
			Acordar ();
		}
	}
	protected virtual void Distrair (){
		if (currentState == Planta_CurrentState.Distraido)
			return; //Esta função só deve roda uma vez.

		currentState = Planta_CurrentState.Distraido;
		//Faz Distrair
	}

	//======================================================================================================================

	protected virtual void OnTriggerEnter (Collider col){
		
	}
	protected virtual void OnTriggerStay (Collider col){
		if(col.CompareTag("Veneno") && !isMurcha){
			MurcharPlanta ();
		}
	}
	protected virtual void OnTriggerExit (Collider col){

	}

	//======================================================================================================================
	//======================================================================================================================
	//======================================================================================================================

	//Esta função me retorna todos os indices selecionados do Enum acceptedSongs
	List<int> ReturnSelectedElements () {
		List<int> selectedElements = new List<int>();
		for (int i = 0; i < System.Enum.GetValues(typeof(PlayerSongs)).Length; i++)
		{
			int layer = 1 << i;
			if (((int) acceptedSongs & layer) != 0)
			{
				selectedElements.Add(i);
			}
		}

		return selectedElements;
	}

	//======================================================================================================================

	public enum Planta_CurrentState
	{
		DefaultState,
		Seguindo,
		Irritado,
		Calmo,
		Dormindo,
		Crescendo,
		Encolhendo,
//		Atento,
		Distraido,
		Murcho
	}
}
