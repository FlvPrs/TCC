using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_PlantaPlataforma : PlantaBehaviour {
	private bool playerPerto, acaoTerminada, idleAbertaOuFechada, stateIdle;
	private float distToPlayer;
	// Use this for initialization
	protected override void Awake(){
		base.Awake ();
//		MudancaEstado (1);
//		acaoTerminada = false;//algo vai fazer ele termianr a acao, final da animação ou tempo.
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();
		print (currentState);
//		distToPlayer = Vector3.Distance (player.position, plantaTransform.position);
//		if (distToPlayer <= 20f) {
//			playerPerto = true;
//		} else {
//			playerPerto = false;
//		}

	}
	public enum EstadosPlantaPlataforma{
		IdleAberta, //case0
		IdleFechada, //case1
		Morta, //case2
		Murcha,//case3
		Estica,//case4
		Encolhe,//case5
		Dancinha//case6
	}

//	void OnTriggerStay(Collider colisor){
//		if (colisor.name == "PlayerCollider") {
//			if (playerPerto) {
//				if (currentSong != PlayerSongs.Empty && currentSong != PlayerSongs.Ninar) {
//					MudancaEstado (0);
//				} else if (currentSong == PlayerSongs.Ninar) {
//					MudancaEstado (1);
//				}
//				if (stateIdle) {
//					if (currentSong == PlayerSongs.Crescimento) {
//						MudancaEstado (4);
//					} else if (currentSong == PlayerSongs.Encolhimento) {
//						MudancaEstado (5);
//					} else if (currentSong == PlayerSongs.Alegria) {
//						MudancaEstado (6);
//					}
//				}
//			}
//		}
//	}

	void RetornarEstadoIdle(){//chamar essa funcao quando terminar uma acao para voltar ao estado idle
		if (acaoTerminada) {//algo vai fazer ele termianr a acao, final da animação ou tempo.
			if (idleAbertaOuFechada) {//true == aberto , false == fechada
				MudancaEstado (0);
			} else {
				MudancaEstado (1);
			}
		}
	}

	private EstadosPlantaPlataforma estado;

	void MudancaEstado(int estadoAtual){
		switch (estadoAtual) {

		case 0:
			estado = EstadosPlantaPlataforma.IdleAberta;
			idleAbertaOuFechada = true;
			stateIdle = true;
			break;

		case 1:
			estado = EstadosPlantaPlataforma.IdleFechada;
			idleAbertaOuFechada = false;
			stateIdle = true;
			break;

		case 2:
			estado = EstadosPlantaPlataforma.Morta;
			stateIdle = false;
			break;

		case 3:
			estado = EstadosPlantaPlataforma.Murcha;
			stateIdle = false;
			break;

		case 4:
			estado = EstadosPlantaPlataforma.Estica;
			stateIdle = false;
			break;

		case 5:
			estado = EstadosPlantaPlataforma.Encolhe;
			stateIdle = false;
			break;

		case 6:
			estado = EstadosPlantaPlataforma.Dancinha;
			stateIdle = false;
			break;
		}
	}
}
