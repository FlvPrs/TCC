using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentoCtrl : MonoBehaviour, IStaccatoInteractable {

	public GameObject orbHigh, orbMid, orbLow;
	public HeightState primeiraNota, segundaNota, terceiraNota;

	private bool tocouPrimeira, tocouSegunda, tocouTerceira, fimDaInteracao;

	public void Interact(HeightState currentHeight, int partitura){
		
		if(currentHeight == primeiraNota){
			ResetNotasTocadas ();
			orbLow.SetActive (true);
			tocouPrimeira = true;
		} 
		else if (currentHeight == segundaNota) {
			if (tocouPrimeira && !tocouSegunda) {
				orbMid.SetActive (true);
				tocouSegunda = true;
			} else {
				ResetNotasTocadas ();
			}
		} 
		else {
			if (tocouSegunda && !tocouTerceira) {
				orbHigh.SetActive (true);
				tocouTerceira = true;
				fimDaInteracao = true;

				gameObject.SetActive (false);
			} else {
				ResetNotasTocadas ();
			}
		}
	}

	void ResetNotasTocadas(){
		orbHigh.SetActive (false);
		tocouPrimeira = false;
		orbMid.SetActive (false);
		tocouSegunda = false;
		orbLow.SetActive (false);
		tocouTerceira = false;
	}
}
