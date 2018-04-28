using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_PlantaPlataforma : PlantaBehaviour {

	NavMeshSurface nmSurface;

	private bool playerPerto, idleAbertaOuFechada, stateIdle, primeiraTroca;
	private float distToPlayer;
	public int alturaFlor;
	private float velocidade = 2f;
	private Transform t;
	public int alturaInicial;
	public bool acaoTerminada, isClosed;
	private Rigidbody r;
	private Vector3 upPlataform, originalPosition, finalDestination, currentDestination;
	protected GameObject MDL_Fechada;
	// Use this for initialization
	protected override void Awake(){
		base.Awake ();
		MDL_Fechada = plantaTransform.Find ("MDL_Fechada").gameObject;

		nmSurface = FindObjectOfType<NavMeshSurface> ();

		if (isClosed) {
			currentState = Planta_CurrentState.Dormindo;
			MDL_Fechada.SetActive (true);
			MDL_Broto.SetActive (false);
			MDL_Crescida.SetActive (false);
			MDL_Murcha.SetActive (false);
			Dormir ();
		} else {
			MDL_Fechada.SetActive (false);
		}

		upPlataform = new Vector3 (0, 1, 0);
		acaoTerminada = true;
		t = GetComponent<Transform> ();
		r = GetComponent<Rigidbody> ();
		originalPosition = t.position;
		currentDestination = originalPosition;
		primeiraTroca = true;
		alturaFlor = alturaInicial;
		if (alturaFlor > 4) {
			alturaFlor = 4;
		} else if (alturaFlor < 1) {
			alturaFlor = 1;
		}
		TrocaAlturaInicial (alturaInicial);
		//TrocaAltura (1);


		//algo vai fazer ele termianr a acao, final da animação ou tempo.
	}
	
	// Update is called once per frame
	protected override void Update () {
		//////print (currentState);
		if (currentState != Planta_CurrentState.Dormindo && !isClosed && !isBroto && !isMurcha) {
			MDL_Crescida.SetActive (true);
			MDL_Fechada.SetActive (false);
		} 
//		if (primeiraTroca) {
//			TrocaAlturaInicial (alturaInicial);
//		}
		base.Update ();
	}

	protected override void Dormir(){
		if (acaoTerminada) {
			if (currentState == Planta_CurrentState.Dormindo)
				return;
			if (currentState == Planta_CurrentState.DefaultState) {
				currentState = Planta_CurrentState.Dormindo;
				MDL_Broto.SetActive (false);
				MDL_Crescida.SetActive (false);
				MDL_Murcha.SetActive (false);
				MDL_Fechada.SetActive (true);

				isBroto = false;
				isMurcha = false;
				isClosed = true;

			}
		}
	}
	protected override void ChamarAtencao ()
	{
		base.ChamarAtencao ();

		isClosed = false;
	}

	protected override void CrescerBroto(){
		MDL_Broto.SetActive (false);
		MDL_Crescida.SetActive (true);
		MDL_Murcha.SetActive (false);
		MDL_Fechada.SetActive (false);
		isClosed = false;
		isBroto = false;
		isMurcha = false;
	}

	public override void MurcharPlanta(){
		if(currentState != Planta_CurrentState.Murcho && !isBroto){
			currentState = Planta_CurrentState.Murcho;
			isMurcha = true;

			MDL_Broto.SetActive (false);
			MDL_Crescida.SetActive (false);
			MDL_Murcha.SetActive (true);
			MDL_Fechada.SetActive (false);
		} else {
			return;
		}
	}

	public override void RevigorarPlanta(GameObject fruta){
		if(currentState == Planta_CurrentState.Murcho){
			currentState = Planta_CurrentState.DefaultState;
			isMurcha = false;
			Destroy (fruta);

			MDL_Broto.SetActive (false);
			MDL_Crescida.SetActive (true);
			MDL_Murcha.SetActive (false);
			MDL_Fechada.SetActive (false);
		} else {
			return;
		}
	}

	protected override void Crescer(){
		if (alturaFlor <= 4) {
			////print ("crescendo111");
			if (acaoTerminada) {
				if (currentState == Planta_CurrentState.Crescendo) {
					return;
				}

				//StartCoroutine ("TimeAcao");
				//transform.localPosition += upPlataform; 
				currentState = Planta_CurrentState.Crescendo;

				if (alturaFlor < 4) {
					alturaFlor++;
				}
				acaoTerminada = false;

			} else {
				TrocaAltura (alturaFlor);

			}
		}
	}

	protected override void Encolher(){
		if (alturaFlor >= 1) {
			////print ("crescendo222");
			if (acaoTerminada) {
				if (currentState == Planta_CurrentState.Encolhendo) {
					return;
				}
				//StartCoroutine ("TimeAcao");
				currentState = Planta_CurrentState.Encolhendo;

				if (alturaFlor > 1) {
					alturaFlor--;
				}
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
	void TrocaAlturaInicial(int levelInicial){
		
		TrocaAltura (levelInicial);

	}
	void TrocaAltura(int nivel){
		//nmSurface.BuildNavMesh ();

		switch (nivel) {
		case 1:
			////print ("rolando1");
			Vector3 dir1 = Vector3.zero;
			upPlataform = originalPosition;
			if (!primeiraTroca) { 
				isClosed = false;
				if (Vector3.Distance (t.position, upPlataform) <= 0.1f) {
					acaoTerminada = true;
					currentState = Planta_CurrentState.DefaultState;
					t.position = upPlataform;
					return;
				} else {
					dir1 = Vector3.Lerp (t.position, upPlataform, velocidade * Time.deltaTime);
					acaoTerminada = false;
					t.position = dir1;
				}
			} else if (primeiraTroca) {
				////print ("inicio1");
				t.position = upPlataform;
				primeiraTroca = false;
			}
			break;
		case 2:
			////print ("rolando2");
			Vector3 dir2 = Vector3.zero;
			upPlataform = originalPosition;
			upPlataform.y += 4.0f;
			if (!primeiraTroca) {
				isClosed = false;
				if (Vector3.Distance (t.position, upPlataform) <= 0.1f) {
					acaoTerminada = true;
					currentState = Planta_CurrentState.DefaultState;
					//primeiraTroca = false;
					t.position = upPlataform;
					return;
				} else {
					dir2 = Vector3.Lerp (t.position, upPlataform, velocidade * Time.deltaTime);
					//acaoTerminada = false;
					t.position = dir2;
				}
			} else if (primeiraTroca) {
				////print ("inicio2");
				t.position = upPlataform;
				primeiraTroca = false;
			}
			break;
		case 3:
			////print ("rolando3");
			Vector3 dir3 = Vector3.zero;
			upPlataform = originalPosition;
			upPlataform.y += 8.0f;
			if (!primeiraTroca) {
				isClosed = false;
				if (Vector3.Distance (t.position, upPlataform) <= 0.1f) {
					acaoTerminada = true;
					currentState = Planta_CurrentState.DefaultState;
					//primeiraTroca = false;
					t.position = upPlataform;
					return;
				} else {
					dir3 = Vector3.Lerp (t.position, upPlataform, velocidade * Time.deltaTime);
					t.position = dir3;
				}
			} else if (primeiraTroca) {
				////print ("inicio3");
				t.position = upPlataform;
				primeiraTroca = false;
			}
			//(upPlataform);
			//plantaTransform.position = Vector3.MoveTowards (originalPosition, upPlataform, velocidade*Time.deltaTime);
			break;
		case 4:
			////print ("rolando4");
			Vector3 dir4 = Vector3.zero;
			upPlataform = originalPosition;
			upPlataform.y += 12.0f;
			if (!primeiraTroca) {
				isClosed = false;
				if (Vector3.Distance (t.position, upPlataform) <= 0.1f) {
					acaoTerminada = true;
					currentState = Planta_CurrentState.DefaultState;
					//primeiraTroca = false;
					t.position = upPlataform;
					return;
				} else {
					dir4 = Vector3.Lerp (t.position, upPlataform, velocidade * Time.deltaTime);
					t.position = dir4;
				}
			} else if (primeiraTroca) {
				t.position = upPlataform;
				primeiraTroca = false;
			}
			break;

		default:
			////print ("rolando0");
			break;
		}
	}

	protected override void OnTriggerEnter(Collider colisor){
		base.OnTriggerEnter (colisor);

		if(colisor.CompareTag("Player")){
			colisor.transform.parent.parent = t;
		}
	}

	protected override void OnTriggerExit(Collider colisor){
		base.OnTriggerExit (colisor);

		if (colisor.CompareTag ("Player")) {
			colisor.transform.parent.parent = null;
		}

		if(colisor.GetComponent<NPC_Kiwi>() != null){
			colisor.GetComponent<NPC_Kiwi> ().OnMovingPlat (true, null);
		}
	}

	protected override void OnTriggerStay (Collider col)
	{
		base.OnTriggerStay (col);

		if(col.GetComponent<NPC_Kiwi>() != null){
			col.GetComponent<NPC_Kiwi> ().OnMovingPlat (acaoTerminada, t);
		}
	}

}
