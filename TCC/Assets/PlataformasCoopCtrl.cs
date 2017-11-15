using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformasCoopCtrl : MonoBehaviour, ISustainInteractable, IFatherInteractable {

	public Platforms[] plataformas;

	public HeightState interactableState;

	private Vector3[] startingPos;
	private Vector3[] son_deltaY;
	private Vector3[] dad_deltaY;

	void Start(){
		startingPos = new Vector3[plataformas.Length];
		son_deltaY = new Vector3[plataformas.Length];
		dad_deltaY = new Vector3[plataformas.Length];

		for (int i = 0; i < startingPos.Length; i++) {
			startingPos [i] = plataformas [i].platform.localPosition;
			son_deltaY [i] = Vector3.up * ((plataformas [i].newYPos - startingPos [i].y) * (1 - plataformas [i].dadToSonRate));
			dad_deltaY [i] = Vector3.up * ((plataformas [i].newYPos - startingPos [i].y) * plataformas [i].dadToSonRate);
		}
	}


	public void Interact(HeightState currentHeight){
		if (currentHeight != interactableState) 
			return;

		for (int i = 0; i < plataformas.Length; i++) {
			
			Vector3 newY = plataformas [i].platform.localPosition;
			newY.y = startingPos [i].y + son_deltaY [i].y;

			plataformas [i].platform.localPosition = Vector3.Lerp (plataformas [i].platform.localPosition, newY, 0.02f);	

			startingPos[i] += son_deltaY [i] * 0.02f;
			son_deltaY [i] -= son_deltaY [i] * 0.02f;
		}
	}

	public bool FatherInteraction(){
		int chegou = 0;

		for (int i = 0; i < plataformas.Length; i++) {

			Vector3 newY = plataformas [i].platform.localPosition;
			newY.y = startingPos [i].y + dad_deltaY [i].y;

			if (plataformas [i].platform.localPosition.y >= newY.y - 0.1f) {
				chegou++;
				continue;
			} else {
				plataformas [i].platform.localPosition = Vector3.Lerp (plataformas [i].platform.localPosition, newY, 0.02f);

				startingPos[i] += dad_deltaY [i] * 0.02f;
				dad_deltaY [i] -= dad_deltaY [i] * 0.02f;
			}
		}

		if (chegou >= plataformas.Length)
			return true;
		else
			return false;
	}
}


[System.Serializable]
public class Platforms
{
	public Transform platform;
	public float newYPos;
	[Range(0f, 1f)]
	public float dadToSonRate; //0.5f significa que a plataforma sobe a mesma quantidade para ambos. Um numero menor faz a plataforma subir mais com o pai, e vice-versa.
}