using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherHeightCtrl : MonoBehaviour {

	#region Variaveis de Altura
	[Range(0.1f, 0.9f)]
	public float minStature = 0.5f;
	[Range(1.1f, 2f)]
	public float maxStature = 2f;

	[HideInInspector]
	public HeightState currentState;

	float defaultHeight = 1f;
	float currentHeight;
	float currentSize;
	float minDifference;
	float maxDifference;
	#endregion

	private Transform t;

	void Awake(){
		t = GetComponent<Transform> ();

		#region Inicialização das Vars de Altura
		currentState = HeightState.Default;
		currentHeight = defaultHeight;
		currentSize = defaultHeight;
		maxDifference = maxStature - defaultHeight;
		minDifference = defaultHeight - minStature;
		#endregion
	}

//	void Update(){
//		#region Controle de Altura
//		Vector3 newScale = t.localScale;
//		newScale.y = currentHeight;
//		newScale.x = newScale.z = currentSize;
//		t.localScale = newScale;
//		if (t.localScale.y >= maxStature - 0.2f * maxStature) {
//			currentState = HeightState.High;
//		}
//		if (t.localScale.y <= minStature + 0.2f * minStature) {
//			currentState = HeightState.Low;
//		}
//		#endregion
//	}

	public void UpdateHeight(float strength, Animator anim){
		anim.SetFloat ("Height", strength);

//		if(strength == 0f){
//			currentHeight = defaultHeight;
//			currentSize = defaultHeight;
//			currentState = HeightState.Default;
//			return;
//		}

		if(strength > 0.1f){
			currentState = HeightState.High;
		} else if(strength < -0.1f) {
			currentState = HeightState.Low;
		} else {
			currentState = HeightState.Default;
		}

//		float tempStrength = -strength;
//
//		currentHeight = defaultHeight;
//		currentHeight += (((Mathf.Abs (strength) + tempStrength) / -2) * minDifference) + (((Mathf.Abs (strength) - tempStrength) /2) * maxDifference);
//		currentSize = defaultHeight;
//		currentSize -= (((Mathf.Abs (strength) + tempStrength) / -2) * minDifference) + (((Mathf.Abs (strength) - tempStrength) /2) * maxDifference * 0.65f) * 0.75f;
	}


}
