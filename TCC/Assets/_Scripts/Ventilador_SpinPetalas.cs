using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ventilador_SpinPetalas : MonoBehaviour {

	public float speed = 5f;
	public bool aroundX, aroundY, aroundZ;

	private Transform t;
	private Planta_Ventilador ventiladorCtrl;

	public bool endWhenReachedTarget;
	public Vector3 targetRotation;

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

		if(endWhenReachedTarget){
			
			if(aroundX){
				if (t.localEulerAngles.x > targetRotation.x)
					enabled = false;
			}
			if(aroundY){
				if (t.localEulerAngles.y > targetRotation.y)
					enabled = false;
			}
			if(aroundZ){
				if (t.localEulerAngles.z > targetRotation.z)
					enabled = false;
			}
		}
	}
}
