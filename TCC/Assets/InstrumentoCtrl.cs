using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentoCtrl : MonoBehaviour {

	//public GameObject orbHigh, orbMid, orbLow;
	//public HeightState primeiraNota, segundaNota, terceiraNota;

	private bool tocouBaixo, tocouMedio, tocouAlto, fimDaInteracao;
	public GameObject objToAppear;
	public Instrumento_InteractionCtrl instrumento_Baixo, instrumento_Medio, instrumento_Alto;

	void Update(){
		tocouBaixo = instrumento_Baixo.interactionDone;
		tocouMedio = instrumento_Medio.interactionDone;
		tocouAlto = instrumento_Alto.interactionDone;

		if(tocouAlto && tocouBaixo && tocouMedio){
			objToAppear.SetActive (true);
		}
	}


//	public void Interact(HeightState currentHeight, int partitura){
//		
//		if(currentHeight == primeiraNota){
//			ResetNotasTocadas ();
//			orbLow.SetActive (true);
//			tocouPrimeira = true;
//		} 
//		else if (currentHeight == segundaNota) {
//			if (tocouPrimeira && !tocouSegunda) {
//				orbMid.SetActive (true);
//				tocouSegunda = true;
//			} else {
//				ResetNotasTocadas ();
//			}
//		} 
//		else {
//			if (tocouSegunda && !tocouTerceira) {
//				orbHigh.SetActive (true);
//				tocouTerceira = true;
//				fimDaInteracao = true;
//
//				objToAppear.SetActive (false);
//			} else {
//				ResetNotasTocadas ();
//			}
//		}
//	}

//	void ResetNotasTocadas(){
//		orbHigh.SetActive (false);
//		tocouPrimeira = false;
//		orbMid.SetActive (false);
//		tocouSegunda = false;
//		orbLow.SetActive (false);
//		tocouTerceira = false;
//	}
}
