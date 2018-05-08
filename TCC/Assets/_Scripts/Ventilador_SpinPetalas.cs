using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ventilador_SpinPetalas : MonoBehaviour {

	public float speed = 5f;
	public bool aroundX, aroundY, aroundZ;

	private Transform t;
	private Planta_Ventilador ventiladorCtrl;

	void Awake(){
		t = GetComponent<Transform> ();
		ventiladorCtrl = t.root.GetComponent<Planta_Ventilador> ();
	}

	void Update(){
		if ((!aroundX && !aroundY && !aroundZ))
			return;
		
		if(aroundX){
			t.Rotate (Vector3.right, speed * Time.deltaTime);
		}
		if(aroundY){
			t.Rotate (Vector3.up, speed * Time.deltaTime);
		}
		if(aroundZ){
			t.Rotate (Vector3.forward, speed * Time.deltaTime);
		}
	}
}
