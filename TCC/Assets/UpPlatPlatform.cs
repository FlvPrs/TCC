using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpPlatPlatform : MonoBehaviour {
	public NPC_PlantaPlataforma plantaPlataforma;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter(Collider colisor){
		//base.OnTriggerEnter (colisor);

		if(colisor.CompareTag("Player")){
			colisor.transform.parent.parent = plantaPlataforma.transform;
		}
	}

	void OnTriggerExit(Collider colisor){
		//base.OnTriggerExit (colisor);

		if (colisor.CompareTag ("Player")) {
			colisor.transform.parent.parent = null;
		}

//		if(colisor.GetComponent<NPC_Kiwi>() != null){
//			colisor.GetComponent<NPC_Kiwi> ().OnMovingPlat (true, null);
//		}
//
//		if(colisor.GetComponent<Npc_BeijaFlor>() != null){
//			colisor.GetComponent<Npc_BeijaFlor> ().OnMovingPlat (true, null);
//		}

		if(colisor.GetComponent<IPlatformMovable>() != null){
			colisor.GetComponent<IPlatformMovable> ().OnMovingPlat (true, null);
		}
	}

	void OnTriggerStay (Collider col)
	{
		//base.OnTriggerStay (col);

//		if(col.GetComponent<NPC_Kiwi>() != null){
//			col.GetComponent<NPC_Kiwi> ().OnMovingPlat (plantaPlataforma.acaoTerminada, plantaPlataforma.transform);
//		}
//
//		if(col.GetComponent<Npc_BeijaFlor>() != null){
//			//print ("deuok");
//			col.GetComponent<Npc_BeijaFlor> ().OnMovingPlat (plantaPlataforma.acaoTerminada, plantaPlataforma.transform);
//		}

		if(col.GetComponent<IPlatformMovable>() != null){
			col.GetComponent<IPlatformMovable> ().OnMovingPlat (plantaPlataforma.acaoTerminada, plantaPlataforma.transform);
		}
	}
}
