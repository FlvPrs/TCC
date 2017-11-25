using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentoCtrl : MonoBehaviour {

	//public GameObject orbHigh, orbMid, orbLow;
	public HeightState primeiraNota, segundaNota, terceiraNota;

	private bool tocouPrimeira, tocouSegunda, tocouTerceira, fimDaInteracao;
	public GameObject objToAppear;
	public Instrumento_InteractionCtrl instrumento_Baixo, instrumento_Medio, instrumento_Alto;
	private ParticleSystem primeira_Yay, primeira_Nay, segunda_Yay, segunda_Nay, terceira_Yay, terceira_Nay;

	void Awake(){
		primeira_Yay = instrumento_Baixo.transform.Find ("Particle_Acerto").GetComponent<ParticleSystem>();
		primeira_Nay = instrumento_Baixo.transform.Find ("Particle_Erro").GetComponent<ParticleSystem>();

		segunda_Yay = instrumento_Baixo.transform.Find ("Particle_Acerto").GetComponent<ParticleSystem>();
		segunda_Nay = instrumento_Baixo.transform.Find ("Particle_Erro").GetComponent<ParticleSystem>();

		terceira_Yay = instrumento_Baixo.transform.Find ("Particle_Acerto").GetComponent<ParticleSystem>();
		terceira_Nay = instrumento_Baixo.transform.Find ("Particle_Erro").GetComponent<ParticleSystem>();
	}

	void Update(){
		tocouPrimeira = instrumento_Baixo.interactionDone;
		tocouSegunda = instrumento_Medio.interactionDone;
		tocouTerceira = instrumento_Alto.interactionDone;

//		if(tocouPrimeira && !tocouSegunda && !tocouTerceira){
//			primeira_Yay.Play ();
//		} else if(tocouPrimeira && tocouSegunda && !tocouTerceira){
//			segunda_Yay.Play ();
//		} else if(tocouPrimeira && tocouSegunda && tocouTerceira){
//			objToAppear.SetActive (true);
//			enabled = false;
//		}

		if(tocouPrimeira && tocouSegunda && tocouTerceira){
			objToAppear.SetActive (true);
			enabled = false;
		}
	}


//	public void Interact(HeightState currentHeight, int partitura){
//		
//		if(currentHeight == primeiraNota){
//			//ResetNotasTocadas ();
//			primeira_Yay.Play ();
//			tocouPrimeira = true;
//		} 
//		else if (currentHeight == segundaNota) {
//			if (tocouPrimeira && !tocouSegunda) {
//				segunda_Yay.Play ();
//				tocouSegunda = true;
//			} else {
//				ResetNotasTocadas ();
//			}
//		} 
//		else {
//			if (tocouSegunda && !tocouTerceira) {
//				terceira_Yay.Play ();
//				tocouTerceira = true;
//				fimDaInteracao = true;
//			} else {
//				ResetNotasTocadas ();
//			}
//		}
//	}
//
//	void ResetNotasTocadas(){
//		instrumento_Baixo.interactionDone = false;
//		primeira_Nay.Play ();
//		instrumento_Medio.interactionDone = false;
//		segunda_Nay.Play ();
//		instrumento_Alto.interactionDone = false;
//		terceira_Nay.Play ();
//	}
}
