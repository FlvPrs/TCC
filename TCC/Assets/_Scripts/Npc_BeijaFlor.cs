using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc_BeijaFlor : NPCBehaviour {

	private float distToPlayer;
	private float timerToPatrulha;
	private bool playerPerto;
	private bool dentroVeneno, podePegarObj, seguindo;
	float timer_PegarObjeto = 0;

	private bool stopUpdate = false;

	List<Transform> collObjects = new List<Transform>();
	Transform objetoCarregado;

	public float maxBaseOffset = 8f;
	float originalBaseOffset;
	bool isCloseToCarnivora = false;

	protected override void Awake(){
		base.Awake ();
		estado = EstadosBeijaFro.Idle;
		dentroVeneno = false;

		originalBaseOffset = nmAgent.baseOffset;

		animCtrl.SetFloat ("idleStartAt", Random.Range (0f, 1f));
	}

	protected override void Update(){
		if (stopUpdate)
			return;
		
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

		if(isCloseToCarnivora || dentroVeneno){
			if (nmAgent.baseOffset < maxBaseOffset)
				nmAgent.baseOffset += Time.deltaTime * 7f;
			else
				nmAgent.baseOffset = maxBaseOffset;
		} else {
			if (nmAgent.baseOffset > originalBaseOffset)
				nmAgent.baseOffset -= Time.deltaTime * 5f;
			else
				nmAgent.baseOffset = originalBaseOffset;
		}
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
		objetoCarregado.localPosition = new Vector3 (0, 0, 1);
	}

	void SoltarObjeto(Transform obj){
		if(objetoCarregado != null)
			objetoCarregado.SetParent (obj);
		//objetoCarregado.transform = obj.transform;
		objetoCarregado = null;
		timer_PegarObjeto = 2f;
	}

	protected override void Seguir(){
		//if (dentroVeneno == false) {
			base.Seguir ();
			animCtrl.SetBool ("isSeguindo", true);
		//}
//		if (dentroVeneno == true) {
//			PararDeSeguir ();
//			mudancaEstado (0);
//		}
	}

	protected override void PararDeSeguir ()
	{
		base.PararDeSeguir ();

		animCtrl.SetBool ("isSeguindo", false);
	}


	public void OnMovingPlat (bool enableNavMesh, Transform plat){
		nmAgent.enabled = enableNavMesh;
		npcTransform.parent = plat;
		//rb.isKinematic = enableNavMesh;
		stopUpdate = !enableNavMesh;
	}


	void OnTriggerStay(Collider colisor){
		if (colisor.name == "PlayerCollider") {
			playerPerto = true;
			if (currentSong == PlayerSongs.Amizade) {
				timerToPatrulha = 0f;
				mudancaEstado (4);
				seguindo = true;
				//Seguir ();
			} else if(currentSong != PlayerSongs.Empty && currentSong != PlayerSongs.Amizade) {
				seguindo = false;
				print("pareiSeguir");
				//playerPerto = false;
				//PararDeSeguir ();
				if (timerToPatrulha <= 10) {
					mudancaEstado (0);
				}
			}
			if (seguindo) {
				Seguir ();
			}else if(!seguindo) {
				//PararDeSeguir ();
			}
		}

		if(colisor.CompareTag ("Carnivora")){
			isCloseToCarnivora = true;
		}
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
			////print ("colidiu");
			SoltarObjeto (colisor.transform);
		}
//		if (colisor.name == "SementePlantaPlataforma") {
//			if (objetoCarregado == null && podePegarObj) {
//				if (!collObjects.Contains (colisor.transform)) {
//					collObjects.Add (colisor.transform);
//					StopCoroutine ("PegarObjeto");
//					StartCoroutine ("PegarObjeto");
//				}
//			}
//		}//TODO(Mudar o jeito de procurar pelo kiwi pelo codigo do fabio)
	}
	void OnTriggerExit (Collider colisor)
	{
		if (colisor.name == "Veneno") {
			dentroVeneno = false;
			if (colisor.CompareTag ("Semente")) {
				if (collObjects.Contains (colisor.transform)) {
					collObjects.Remove (colisor.transform);
				}
			}

		}
		if (colisor.name == "PlayerCollider") {
			playerPerto = false;
			//seguindo = false;
			//mudancaEstado (0);
		}

		if(colisor.CompareTag ("Carnivora")){
			isCloseToCarnivora = false;
		}
	}
}

