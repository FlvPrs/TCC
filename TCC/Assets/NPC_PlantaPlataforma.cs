using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_PlantaPlataforma : PlantaBehaviour {
	private bool playerPerto, idleAbertaOuFechada, stateIdle;
	private float distToPlayer;
	public int alturaFlor;
	private float velocidade = 2f;
	private Transform t;
	public bool acaoTerminada;
	private Rigidbody r;
	private Vector3 upPlataform, originalPosition, finalDestination, currentDestination;
	// Use this for initialization
	protected override void Awake(){
		base.Awake ();
		upPlataform = new Vector3 (0, 1, 0);
		acaoTerminada = true;
		t = GetComponent<Transform> ();
		r = GetComponent<Rigidbody> ();
		originalPosition = t.position;
		currentDestination = originalPosition;
		TrocaAltura (1);
		alturaFlor = 1;
		//algo vai fazer ele termianr a acao, final da animação ou tempo.
	}
	
	// Update is called once per frame
	protected override void Update () {
		print (currentState);
		base.Update ();
	}

	protected override void Dormir(){
		if (acaoTerminada) {
			if (currentState == Planta_CurrentState.Dormindo)
				return;
			if (currentState == Planta_CurrentState.DefaultState) {
				currentState = Planta_CurrentState.Dormindo;
			}
		}
	}

	protected override void Crescer(){
		if (alturaFlor <= 4) {
			print ("crescendo111");
			if (acaoTerminada) {
				if (currentState == Planta_CurrentState.Crescendo) {
					return;
				}

				StartCoroutine ("TimeAcao");
				//transform.localPosition += upPlataform; 
				currentState = Planta_CurrentState.Crescendo;
				alturaFlor++;
				acaoTerminada = false;

			} else {
				TrocaAltura (alturaFlor);

			}
		}
	}

	protected override void Encolher(){
		if (alturaFlor >= 1) {
			print ("crescendo222");
			if (acaoTerminada) {
				if (currentState == Planta_CurrentState.Encolhendo) {
					return;
				}

				StartCoroutine ("TimeAcao");
	
				currentState = Planta_CurrentState.Encolhendo;
				alturaFlor--;
				acaoTerminada = false;

			} else {
				TrocaAltura (alturaFlor);
			}
		}
	}

	IEnumerator TimeAcao(){
		yield return new WaitForSecondsRealtime (3.0f);
		acaoTerminada = true;
		currentState = Planta_CurrentState.DefaultState;
	}

	void TrocaAltura(int nivel){
		switch (nivel) {
		case 1:
			print ("rolando1");
			Vector3 dir1;
			upPlataform = originalPosition;

			if (Vector3.Distance (t.position, upPlataform) <= 0.1f) {
				return;
			}
			else {
				dir1 = Vector3.Lerp (t.position, upPlataform, velocidade * Time.deltaTime);
			}
			t.position = dir1;

			break;
		case 2:
			print ("rolando2");
			Vector3 dir2;
			upPlataform = originalPosition;
			upPlataform.y += 4.0f;
			if (Vector3.Distance (t.position, upPlataform) <= 0.1f) {
				return;
			}
			else {
				dir2 = Vector3.Lerp (t.position, upPlataform, velocidade * Time.deltaTime);
			}
			t.position = dir2;
			break;
		case 3:
			print ("rolando3");
			Vector3 dir3;
			upPlataform = originalPosition;
			upPlataform.y += 10.0f;
			if (Vector3.Distance (t.position, upPlataform) <= 0.1f) {
				return;
			}
			else {
				dir3 = Vector3.Lerp (t.position, upPlataform, velocidade * Time.deltaTime);
			}
			t.position = dir3;
			//(upPlataform);
			//plantaTransform.position = Vector3.MoveTowards (originalPosition, upPlataform, velocidade*Time.deltaTime);
			break;
		case 4:
			print ("rolando4");
			Vector3 dir4;
			upPlataform = originalPosition;
			upPlataform.y += 16.0f;
			if (Vector3.Distance (t.position, upPlataform) <= 0.1f) {
				return;
			}
			else {
				dir4 = Vector3.Lerp (t.position, upPlataform, velocidade * Time.deltaTime);
			}
			t.position = dir4;
			break;

		default:
			print ("rolando0");
			break;
		}
	}

	protected override void OnTriggerEnter(Collider colisor){
		if(colisor.CompareTag("Player")){
			colisor.transform.parent.SetParent (this.transform);
		}
	}

	protected override void OnTriggerExit(Collider colisor){
		if (colisor.CompareTag ("Player")) {
			colisor.transform.parent.SetParent (null);
		}
	}

}
