using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc_BeijaFlor : NPCBehaviour {

	private float distToPlayer;
	private float timerToPatrulha;
	private bool playerPerto;
	private bool dentroVeneno, podePegarObj;
	float timer_PegarObjeto = 0;

	List<Transform> collObjects = new List<Transform>();
	Transform objetoCarregado;

	protected override void Start(){
		base.Start ();
		estado = EstadosBeijaFro.Idle;
		dentroVeneno = false;
	}

	protected override void Update(){
		base.Update ();
		distToPlayer = Vector3.Distance (player.position, npcTransform.position);

		if (estado == EstadosBeijaFro.Idle) {
			timerToPatrulha += Time.deltaTime * 1;
		}
		
		if (timerToPatrulha >= 10f) {
			mudancaEstado (2);
		}

		if (timer_PegarObjeto > 0f) {
			timer_PegarObjeto -= Time.deltaTime;
			podePegarObj = false;
		} else {
			timer_PegarObjeto = 0f;
			podePegarObj = true;
		}

		print (estado);
	}

	public enum EstadosBeijaFro{
		Idle, 
		JogaSemente, 
		RoubaSemente, 
		Patrulha,
		Seguir
	}

	private EstadosBeijaFro estado;
	void mudancaEstado(int estadoAtual){

		switch (estadoAtual) {

		case 0:
			estado = EstadosBeijaFro.Idle;
			break;
				
		case 1:
			estado = EstadosBeijaFro.JogaSemente;
			break;

		case 2:
			estado = EstadosBeijaFro.Patrulha;
			break;

		case 3:
			estado = EstadosBeijaFro.RoubaSemente;
			break;

		case 4: 
			estado = EstadosBeijaFro.Seguir;
			break;

		default:
			estado = EstadosBeijaFro.Idle;
			break;
		}
		
	}

	IEnumerator PegarObjeto(){
		yield return new WaitForSeconds (0.25f);

		float dist = 1000f;
		int index = 0;
		for (int i = 0; i < collObjects.Count; i++) {
			float temp = Vector3.Distance (npcTransform.position, collObjects [i].position);
			if (temp < dist) {
				dist = temp;
				index = i;
			}
		}

		CarregarObjeto (collObjects [index]);
	}

	void CarregarObjeto (Transform obj){
		objetoCarregado = obj;

		objetoCarregado.SetParent (npcTransform);
		objetoCarregado.localPosition = new Vector3 (0, 2, 0);
	}

	void SoltarObjeto(){
		objetoCarregado.SetParent (null);
		objetoCarregado = null;
		timer_PegarObjeto = 2f;
	}

	protected override void Seguir(){
		if (dentroVeneno == false && distToPlayer <=15f) {
			base.Seguir ();
		}
		if (dentroVeneno == true) {
			PararDeSeguir ();
			mudancaEstado (0);
		}
	}

	void OnTriggerStay(Collider colisor){
		if (colisor.name == "PlayerCollider") {
			if (distToPlayer <= 15f && currentSong == PlayerSongs.Amizade) {
				playerPerto = true;
				timerToPatrulha = 0f;
				mudancaEstado (4);
				Seguir ();
			} else {
				playerPerto = false;
				PararDeSeguir ();
				if (timerToPatrulha <= 10) {
					mudancaEstado (0);
				}
			}
		}
//		else {
//			playerPerto = false;
//			PararDeSeguir ();
//			if (timerToPatrulha <= 10) {
//					mudancaEstado (0);
//			}
//		}

	}

	void OnTriggerEnter(Collider colisor){
		if (colisor.name == "Veneno") {
			dentroVeneno = true;
		}
		if (colisor.CompareTag ("Semente")) {
			if (objetoCarregado == null && podePegarObj) {
				if (!collObjects.Contains (colisor.transform)) {
					collObjects.Add (colisor.transform);
					StopCoroutine ("PegarObjeto");
					StartCoroutine ("PegarObjeto");
				}
			}
		}
		if (colisor.CompareTag("TerraFertil")) {
			print ("colidiu");
			SoltarObjeto ();
		}//TO DO(Mudar o jeito de procurar pelo kiwi pelo codigo do fabio)
	}
	void OnTriggerExit(Collider colisor){
		if (colisor.name == "Veneno") {
			dentroVeneno = false;
			if (colisor.CompareTag ("Semente")) {
				if (collObjects.Contains (colisor.transform)) {
					collObjects.Remove (colisor.transform);
				}
			}

		}

//		if (colisor.name == "PlayerCollider") {
//			playerPerto = false;
//			PararDeSeguir ();
//			if (timerToPatrulha <= 10) {
//				mudancaEstado (0);
//			}
//		}
	}
		//else {
			//dentroVeneno = false;
		//}
	
}
