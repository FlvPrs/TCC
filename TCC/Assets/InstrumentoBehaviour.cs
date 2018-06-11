using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentoBehaviour : MonoBehaviour {

	public bool startPauta_Filho, startPauta_Pai, playerHasToPlayBoth;
	public float duration;

	public Transform[] raycastOrigins;
	public InstrumentoTrigger_Ctrl[] triggersNota_Filho;
	public InstrumentoTrigger_Ctrl[] triggersNota_Pai;
	public GameObject[] finishLights;
	public LayerMask raycastMask = -1;

	public AudioClip musica_Clip;
	public AudioClip musicaFinalOpcional_Clip;

	public GameObject objToAppear;
	public Ventilador_SpinPetalas openDoor;
	bool ativouPorta = false;

	public bool canStart;

	bool forward = false;
	float speed;
	Transform myT;
	Quaternion originalRotation;
	Transform pauta_Filho, pauta_Pai;

	float rotationAmount_Filho, rotationAmount_Pai;

	bool isFinished_Filho, isFinished_Pai;

	int oldAltura_Filho = -1;
	int oldAltura_Pai = -1;

	Color defaultColor;

	bool playerCantouEmCima = false;
	bool playerCantouEmBaixo = false;

	bool trocouMusica = false;

	public int maxMistakes = 3;
	int currentNumberOfMistakes = 0;

	bool errouNota;
	bool oldFoundNote;

	public bool CHEAT_FinishInstrumento = false;

	void Start () {
		speed = 360f / duration;

		GetComponent<AudioSource> ().clip = musica_Clip;

		myT = GetComponent<Transform> ();
		originalRotation = myT.rotation;

		pauta_Filho = myT.GetChild (0);
		pauta_Pai = myT.GetChild (1);

		finishLights [0].SetActive (false);
		finishLights [1].SetActive (false);

		for (int i = 0; i < raycastOrigins.Length; i++) {
			RaycastHit hit;
			bool hitNote = Physics.Raycast (raycastOrigins[i].position, raycastOrigins[i].forward, out hit, 1f, raycastMask);
			if(hitNote){
				defaultColor = hit.transform.Find("Mandala_Glow").GetComponent<MeshRenderer> ().material.GetColor("_TintColor");
				break;
			}
		}

		//hitNota = oldHitNote = hitNote;
	}
		
	void Update () {
//		if(playerHasToPlayBoth && !startPauta_Filho && !startPauta_Pai){
//			if(playerCantouEmCima){
//				startPauta_Filho = true;
//				startPauta_Pai = false;
//			} else if (playerCantouEmBaixo){
//				startPauta_Pai = true;
//				startPauta_Filho = false;
//			}
//		}

		int altura = 3; //3 é o índice da nota vazia.
		int altura_Pai = 3;
		RaycastHit hit_Filho = new RaycastHit();
		RaycastHit hit_Pai = new RaycastHit();
		int startAt = (startPauta_Filho) ? 0 : 3;
		bool foundNote = false;
		int endAt = (startPauta_Pai) ? raycastOrigins.Length : (startAt + 3);
		for (int i = startAt; i < endAt; i++) {
			RaycastHit hitTest;
			bool hitNota = Physics.Raycast (raycastOrigins[i].position, raycastOrigins[i].forward, out hitTest, 1f, raycastMask);
			Debug.DrawRay (raycastOrigins[i].position, raycastOrigins[i].forward, Color.red);
			if(hitNota){
				if (i < 3) {
					altura = i;
					oldAltura_Filho = i;
					hit_Filho = hitTest;
					if(playerCantouEmCima)
						foundNote = true;
				} else {
					oldAltura_Pai = i - 3;
					altura_Pai = i - 3;
					hit_Pai = hitTest;
					if(playerCantouEmBaixo)
						foundNote = true;
				}
//				if (i > 3) {
//					if (!startPauta_Filho && !foundNote) { //Oq significa que é o player tocando no lugar do pai...
//						altura = i - 3;
//						oldAltura = i - 3;
//						hit = hitTest;
//						foundNote = true;
//					} else {
//						altura_Pai = i - 3;
//					}
//				} else {
//					altura = i;
//					oldAltura = i;
//					hit = hitTest;
//					foundNote = true;
//				}
				//break;
			}
		}

		if(forward && !foundNote && oldFoundNote){
			if(errouNota){
				currentNumberOfMistakes++;
				errouNota = false;
				if(currentNumberOfMistakes >= maxMistakes){
					//TODO
					forward = false;
				}
			}
		}
		oldFoundNote = foundNote;


		if (canStart || CHEAT_FinishInstrumento) {
			if (isFinished_Filho && startPauta_Filho) {
				startPauta_Filho = false;
				playerCantouEmCima = false;
				canStart = false;
				if(isFinished_Pai){
					GetComponent<AudioSource> ().loop = true;
					//GetComponent<AudioSource> ().Play ();
				}
				//pauta_Filho.Rotate (Vector3.up * speed * Time.deltaTime);
			}
			if (isFinished_Pai && startPauta_Pai) {
				startPauta_Pai = false;
				playerCantouEmBaixo = false;
				canStart = false;
				if(isFinished_Filho){
					GetComponent<AudioSource> ().loop = true;
					//GetComponent<AudioSource> ().Play ();
				}
				//pauta_Pai.Rotate (Vector3.up * speed * Time.deltaTime);
			}

			if ((isFinished_Filho && isFinished_Pai) || CHEAT_FinishInstrumento) {
				if(CHEAT_FinishInstrumento){
					finishLights [0].SetActive (true);
					finishLights [1].SetActive (true);
				}
				if(!trocouMusica){
					trocouMusica = true;
					if (musicaFinalOpcional_Clip != null)
						GetComponent<AudioSource> ().clip = musicaFinalOpcional_Clip;
					GetComponent<AudioSource> ().Play ();
				}
				canStart = true;
				pauta_Filho.Rotate (Vector3.up * speed * Time.deltaTime);
				pauta_Pai.Rotate (Vector3.up * speed * Time.deltaTime);
				if (objToAppear != null)
					objToAppear.SetActive (true);
				if (!ativouPorta && openDoor != null) {
					openDoor.enabled = true;
					ativouPorta = true;
				}
				return;
			} else if(!playerCantouEmBaixo && !playerCantouEmCima) {
				canStart = false;
				forward = false;
			}
		}
		else {
			currentNumberOfMistakes = 0;

			if(startPauta_Filho && !isFinished_Filho){ //Não importa se com ou sem o pai, pois com certeza é a pauta na qual o player está tocando.
				for (int i = 0; i < triggersNota_Filho.Length; i++) {
					if(i != altura){
						triggersNota_Filho [i].gameObject.SetActive (false);
					} else {
						triggersNota_Filho [i].gameObject.SetActive (true);
						if(triggersNota_Filho[i].interactedWith && triggersNota_Filho[i].interagiuCorretamente){
							if (!forward) {
								GetComponent<AudioSource> ().Play ();
								playerCantouEmCima = true;

							}
							forward = true;
							canStart = true;
						} else {
							triggersNota_Filho [i].interactedWith = false;
							//currentNumberOfMistakes++;
						}
					}
				}
			}
			if (startPauta_Pai && !isFinished_Pai && (playerHasToPlayBoth || !startPauta_Filho)) { //Apenas se a pauta do filho não for ativada (o que significa que é o player que está tocando na pauta do pai).
				for (int i = 0; i < triggersNota_Pai.Length; i++) {
					if(i != altura_Pai){
						triggersNota_Pai [i].gameObject.SetActive (false);
					} else {
						triggersNota_Pai [i].gameObject.SetActive (true);
						if(triggersNota_Pai[i].interactedWith && triggersNota_Pai[i].interagiuCorretamente){
							if (!forward) {
								GetComponent<AudioSource> ().Play ();
								playerCantouEmBaixo = true;
							}
							forward = true;
							canStart = true;
						} else {
							triggersNota_Pai [i].interactedWith = false;
							//currentNumberOfMistakes++;
						}
					}
				}
			}
			return;
		}

		if(forward){
			if (startPauta_Filho && ((playerHasToPlayBoth && playerCantouEmCima) || !playerHasToPlayBoth)) {
				pauta_Filho.Rotate (Vector3.up * speed * Time.deltaTime);
				rotationAmount_Filho += speed * Time.deltaTime;
			} 
			if (startPauta_Pai && ((playerHasToPlayBoth && playerCantouEmBaixo) || !playerHasToPlayBoth)) {
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
			currentNumberOfMistakes = 0;

			GetComponent<AudioSource> ().Stop ();

			if (altura != 3) {
				hit_Filho.transform.Find ("Mandala_Glow").GetComponent<MeshRenderer> ().material.SetColor ("_TintColor", defaultColor);
			}
			if(altura_Pai != 3) {
				hit_Pai.transform.Find ("Mandala_Glow").GetComponent<MeshRenderer> ().material.SetColor ("_TintColor", defaultColor);
			}

			if (startPauta_Filho) {
				if (rotationAmount_Filho > 0) {
					pauta_Filho.Rotate (Vector3.up * -speed * 6f * Time.deltaTime);
					rotationAmount_Filho -= speed * 6f * Time.deltaTime;
				} else {
					pauta_Filho.rotation = originalRotation;
					rotationAmount_Filho = 0f;
					playerCantouEmCima = false;
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
					playerCantouEmBaixo = false;
					if(!playerCantouEmCima)
						canStart = false;
				}
			}
		}

		if(startPauta_Filho && playerCantouEmCima && !isFinished_Filho){ //Não importa se com ou sem o pai, pois com certeza é a pauta na qual o player está tocando.
			for (int i = 0; i < triggersNota_Filho.Length; i++) {
				if(i != altura){
					triggersNota_Filho [i].gameObject.SetActive (false);
				} else {
					triggersNota_Filho [i].gameObject.SetActive (true);
					if(triggersNota_Filho[i].interactedWith && !triggersNota_Filho [i].interagiuCorretamente){
						//forward = false;
						//currentNumberOfMistakes++;
						errouNota = true;
					} else if (i != 3 && triggersNota_Filho[i].interactedWith && triggersNota_Filho [i].interagiuCorretamente) {
						hit_Filho.transform.Find("Mandala_Glow").GetComponent<MeshRenderer> ().material.SetColor("_TintColor", new Color (0f, 0.71f, 1f, 0.24f));
					}
				}
			}

			if (!playerHasToPlayBoth) {
				for (int i = 0; i < triggersNota_Pai.Length; i++) {
					if (i != altura_Pai) {
						triggersNota_Pai [i].gameObject.SetActive (false);
					} else {
						triggersNota_Pai [i].gameObject.SetActive (true);
					}
				}
			}

			if (!triggersNota_Filho [oldAltura_Filho].isActiveAndEnabled && !triggersNota_Filho [oldAltura_Filho].interactedWith) {
				//forward = false;
				//currentNumberOfMistakes++;
				errouNota = true;
			}
		}
		if (startPauta_Pai && playerCantouEmBaixo && !isFinished_Pai){
			for (int i = 0; i < triggersNota_Pai.Length; i++) {
				if(i != altura_Pai){
					triggersNota_Pai [i].gameObject.SetActive (false);
				} else {
					triggersNota_Pai [i].gameObject.SetActive (true);
					if(triggersNota_Pai[i].interactedWith && !triggersNota_Pai [i].interagiuCorretamente){
						//forward = false;
						//currentNumberOfMistakes++;
						errouNota = true;
					} else if (i != 3 && triggersNota_Pai[i].interactedWith && triggersNota_Pai [i].interagiuCorretamente) {
						hit_Pai.transform.Find("Mandala_Glow").GetComponent<MeshRenderer> ().material.SetColor("_TintColor", new Color (0f, 0.71f, 1f, 0.24f));
					}
				}
			}

			if (!triggersNota_Pai [oldAltura_Pai].isActiveAndEnabled && !triggersNota_Pai [oldAltura_Pai].interactedWith) {
				//forward = false;
				//currentNumberOfMistakes++;
				errouNota = true;
			}
		}
	}
}
