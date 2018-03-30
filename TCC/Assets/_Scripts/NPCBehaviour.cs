using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCBehaviour : MonoBehaviour, ISongListener {

	// (0) Amizade	-	-	(Seguir)	-	UPDATE
	// (1) Estorvo	-	-	(Irritar)	-	Start
	// (2) Serenidade	-	(Acalmar)	-	Start
	// (3) Ninar	-	-	(Dormir)	-	Start
	// (4) Crescimento	-	(Crescer)	-	UPDATE
	// (5) Encolhimento	-	(Encolher)	-	UPDATE
	// (6) Alegria	-	-	(Distrair)	-	Start

	protected NPC_CurrentState currentState;

	[BitMaskAttribute(typeof(PlayerSongs))]
	public PlayerSongs acceptedSongs;

	protected List<int> selectedSongs;
	protected PlayerSongs currentSong;

	protected NavMeshAgent nmAgent;

	void Start () {
		selectedSongs = ReturnSelectedElements ();
		nmAgent = GetComponent<NavMeshAgent> ();

		currentState = NPC_CurrentState.AcordadoPadrao;
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		
	}

	public void DetectSong (PlayerSongs song){
		currentSong = song;
	}


	protected virtual void Seguir (){
		if (!selectedSongs.Contains (0))
			return;

		//Faz Seguir
		//TODO: Se pa colocar aqui condições que se aplicam a todos, como "Só Seguir se NÃO estiver Dormindo".
		//TODO: No filho, adicionar condições especificas, como "Só Seguir se NÃO estiver Irritado".
	}
	protected virtual void PararDeSeguir (){

	}

	protected virtual void Irritar (){
		if (!selectedSongs.Contains (1))
			return;

		//Faz Irritar
	}
	protected virtual void Acalmar (){
		if (!selectedSongs.Contains (2))
			return;

		//Faz Acalmar
	}

	protected virtual void Dormir (){
		if (!selectedSongs.Contains (3))
			return;

		//Faz Dormir
	}
	protected virtual void Acordar (){

	}

	protected virtual void Crescer (){
		if (!selectedSongs.Contains (4))
			return;

		//Faz Crescer
	}
	protected virtual void Encolher (){
		if (!selectedSongs.Contains (5))
			return;

		//Faz Encolher
	}

	protected virtual void ChamarAtencao (){

	}
	protected virtual void Distrair (){
		if (!selectedSongs.Contains (6))
			return;

		//Faz Distrair
	}



	//=============================================================================
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

	//=============================================================================
	public enum NPC_CurrentState
	{
		AcordadoPadrao,
		Seguindo,
		Irritado,
		Calmo,
		Dormindo,
		Crescendo,
		Encolhendo,
		Atento,
		Distraido
	}
}
