using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentoCtrl : MonoBehaviour {

	//public GameObject orbHigh, orbMid, orbLow;
	//public HeightState primeiraNota, segundaNota, terceiraNota;

	private bool tocouPrimeira, tocouSegunda, tocouTerceira, fimDaInteracao;
	public GameObject objToAppear;
	public Instrumento_InteractionCtrl instrumento_PrimeiraNota, instrumento_SegundaNota, instrumento_TerceiraNota;
	private ParticleSystem primeira_Yay, primeira_Nay, segunda_Yay, segunda_Nay, terceira_Yay, terceira_Nay;

	void Awake(){
		primeira_Yay = instrumento_PrimeiraNota.particle_Yay;
		primeira_Nay = instrumento_PrimeiraNota.particle_Nay;

		segunda_Yay = instrumento_SegundaNota.particle_Yay;
		segunda_Nay = instrumento_SegundaNota.particle_Nay;

		terceira_Yay = instrumento_TerceiraNota.particle_Yay;
		terceira_Nay = instrumento_TerceiraNota.particle_Nay;
	}

	public void UpdateInstrumento(bool correctInteraction){
		if(!correctInteraction){
			ResetNotasTocadas ();
			//StopCoroutine (HideParticles ());
			//StartCoroutine (HideParticles ());
			return;
		}

		if(instrumento_PrimeiraNota.interactionDone && !tocouPrimeira){
			if(!tocouSegunda && !tocouTerceira){
				tocouPrimeira = true;
				primeira_Yay.Play ();
				var yay = primeira_Yay.emission;
				yay.enabled = true;
			} else {
				ResetNotasTocadas ();
			}
		}
		 else if(instrumento_SegundaNota.interactionDone && !tocouSegunda){
			if(tocouPrimeira && !tocouTerceira){
				tocouSegunda = true;
				segunda_Yay.Play ();
				var yay = segunda_Yay.emission;
				yay.enabled = true;
			} else {
				ResetNotasTocadas ();
			}
		}
		else if(instrumento_TerceiraNota.interactionDone && !tocouTerceira){
			if(tocouPrimeira && tocouSegunda){
				terceira_Yay.Play ();
				var yay = terceira_Yay.emission;
				yay.enabled = true;
				tocouTerceira = true;
				objToAppear.SetActive (true);
				enabled = false;
			} else {
				ResetNotasTocadas ();
			}
		}
		else {
			ResetNotasTocadas ();
		}

		//StopCoroutine (HideParticles ());
		//StartCoroutine (HideParticles ());
	}

	void ResetNotasTocadas(){
		
		tocouPrimeira = false;
		instrumento_PrimeiraNota.interactionDone = false;
//		primeira_Yay.Stop ();
//		primeira_Yay.emission.enabled = false;
		primeira_Nay.Play ();
		var nay = primeira_Nay.emission;
		nay.enabled = true;

		tocouSegunda = false;
		instrumento_SegundaNota.interactionDone = false;
//		segunda_Yay.Stop ();
//		segunda_Yay.emission.enabled = false;
		segunda_Nay.Play ();
		nay = segunda_Nay.emission;
		nay.enabled = true;

		tocouTerceira = false;
		instrumento_TerceiraNota.interactionDone = false;
//		terceira_Yay.Stop ();
//		terceira_Yay.emission.enabled = false;
		terceira_Nay.Play ();
		nay = terceira_Nay.emission;
		nay.enabled = true;

	}

//	IEnumerator HideParticles(){
//		yield return new WaitForSeconds (2.2f);
//		primeira_Yay.SetActive (false);
//		segunda_Yay.SetActive (false);
//		terceira_Yay.SetActive (false);
//
//		primeira_Nay.SetActive (false);
//		segunda_Nay.SetActive (false);
//		terceira_Nay.SetActive (false);
//	}
}

