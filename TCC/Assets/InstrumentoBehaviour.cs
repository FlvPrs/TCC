using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentoBehaviour : MonoBehaviour {

	public bool startPauta_Filho, startPauta_Pai;
	public float duration;

	public Transform[] raycastOrigins;
	public InstrumentoTrigger_Ctrl[] triggersNota_Filho;
	public InstrumentoTrigger_Ctrl[] triggersNota_Pai;
	public GameObject[] finishLights;
	public LayerMask raycastMask = -1;

	//[HideInInspector]
	public bool canStart;

	bool forward = false;
	float speed;
	Transform myT;
	Quaternion originalRotation;
	Transform pauta_Filho, pauta_Pai;

	float rotationAmount_Filho, rotationAmount_Pai;

	bool isFinished_Filho, isFinished_Pai;

	int oldAltura = -1;

	void Start () {
		speed = 360f / duration;

		myT = GetComponent<Transform> ();
		originalRotation = myT.rotation;

		pauta_Filho = myT.GetChild (0);
		pauta_Pai = myT.GetChild (1);

		finishLights [0].SetActive (false);
		finishLights [1].SetActive (false);
	}
		
	void Update () {
		int altura = 3; //3 é o índice da nota vazia.
		for (int i = 0; i < raycastOrigins.Length; i++) {
			bool hitNota = Physics.Raycast (raycastOrigins[i].position, Vector3.forward, 1f, raycastMask);
			Debug.DrawRay (raycastOrigins[i].position, Vector3.forward, Color.red);
			if(hitNota){
				altura = i;
				oldAltura = i;
				break;
			}
		}
		if (canStart) {
			if (isFinished_Filho) {
				startPauta_Filho = false;
				//pauta_Filho.Rotate (Vector3.up * speed * Time.deltaTime);
			}
			if (isFinished_Pai) {
				startPauta_Pai = false;
				//pauta_Pai.Rotate (Vector3.up * speed * Time.deltaTime);
			}

			if (isFinished_Filho && isFinished_Pai) {
				pauta_Filho.Rotate (Vector3.up * speed * Time.deltaTime);
				pauta_Pai.Rotate (Vector3.up * speed * Time.deltaTime);
				return;
			}
		}
		else {
			if(startPauta_Filho){ //Não importa se com ou sem o pai, pois com certeza é a pauta na qual o player está tocando.
				for (int i = 0; i < triggersNota_Filho.Length; i++) {
					if(i != altura){
						triggersNota_Filho [i].gameObject.SetActive (false);
					} else {
						triggersNota_Filho [i].gameObject.SetActive (true);
						if(triggersNota_Filho[i].interactedWith && triggersNota_Filho[i].interagiuCorretamente){
							forward = true;
							canStart = true;
						} else {
							triggersNota_Filho [i].interactedWith = false;
						}
					}
				}
			} else if (startPauta_Pai){ //Apenas se a pauta do filho não for ativada (o que significa que é o player que está tocando na pauta do pai).
				for (int i = 0; i < triggersNota_Pai.Length; i++) {
					if(i != altura){
						triggersNota_Pai [i].gameObject.SetActive (false);
					} else {
						triggersNota_Pai [i].gameObject.SetActive (true);
						if(triggersNota_Pai[i].interactedWith && triggersNota_Pai[i].interagiuCorretamente){
							forward = true;
							canStart = true;
						} else {
							triggersNota_Pai [i].interactedWith = false;
						}
					}
				}
			}
			return;
		}

		if(forward){
			if (startPauta_Filho) {
				pauta_Filho.Rotate (Vector3.up * speed * Time.deltaTime);
				rotationAmount_Filho += speed * Time.deltaTime;
			}
			if (startPauta_Pai) {
				pauta_Pai.Rotate (Vector3.up * speed * Time.deltaTime);
				rotationAmount_Pai += speed * Time.deltaTime;
			}

			if (rotationAmount_Filho >= 360f) {
				isFinished_Filho = true;
				finishLights [0].SetActive (true);
			}
			if (rotationAmount_Pai >= 360f) {
				isFinished_Pai = true;
				finishLights [1].SetActive (true);
			}
		} else {
			if (startPauta_Filho) {
				if (rotationAmount_Filho > 0) {
					pauta_Filho.Rotate (Vector3.up * -speed * 6f * Time.deltaTime);
					rotationAmount_Filho -= speed * 6f * Time.deltaTime;
				} else {
					pauta_Filho.rotation = originalRotation;
					rotationAmount_Filho = 0f;
					//canStart = false; //Apagar oq aparece primeiro pra garantir q o segundo rode.
				}
			}
			if (startPauta_Pai) {
				if (rotationAmount_Pai > 0) {
					pauta_Pai.Rotate (Vector3.up * -speed * 6f * Time.deltaTime);
					rotationAmount_Pai -= speed * 6f * Time.deltaTime;
				} else {
					pauta_Pai.rotation = originalRotation;
					rotationAmount_Pai = 0f;
					canStart = false;
				}
			}
		}

		if(startPauta_Filho){ //Não importa se com ou sem o pai, pois com certeza é a pauta na qual o player está tocando.
			for (int i = 0; i < triggersNota_Filho.Length; i++) {
				if(i != altura){
					triggersNota_Filho [i].gameObject.SetActive (false);
				} else {
					triggersNota_Filho [i].gameObject.SetActive (true);
					if(triggersNota_Filho[i].interactedWith && !triggersNota_Filho [i].interagiuCorretamente){
						forward = false;
					}
				}
			}

			if (!triggersNota_Filho [oldAltura].isActiveAndEnabled && !triggersNota_Filho [oldAltura].interactedWith) {
				forward = false;
			}
		} else if (startPauta_Pai){ //Apenas se a pauta do filho não for ativada (o que significa que é o player que está tocando na pauta do pai).
			for (int i = 0; i < triggersNota_Pai.Length; i++) {
				if(i != altura){
					triggersNota_Pai [i].gameObject.SetActive (false);
				} else {
					triggersNota_Pai [i].gameObject.SetActive (true);
					if(triggersNota_Pai[i].interactedWith && !triggersNota_Pai [i].interagiuCorretamente){
						forward = false;
					}
				}
			}

			if (!triggersNota_Pai [oldAltura].isActiveAndEnabled && !triggersNota_Pai [oldAltura].interactedWith) {
				forward = false;
			}
		}
	}
}
