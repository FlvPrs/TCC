using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkScreen : MonoBehaviour {

	Image screenFX;

	void Awake (){
		screenFX = GetComponent<Image> ();
	}

	public void Blink (){
		StartCoroutine (BlinkFX ());
	}

	IEnumerator BlinkFX (){
		screenFX.enabled = true;
		yield return new WaitForSeconds (Time.deltaTime * 2f);
		screenFX.enabled = false;
	}
}
