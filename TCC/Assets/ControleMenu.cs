using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControleMenu : MonoBehaviour {
	private float timerToSkip = -1f;
	private int numeroImagem;
	public GameObject bbLogo, cimologo, joystick, flowerLoading, aviso1, aviso2;

	bool extendCimoTime = true;

	// Use this for initialization
	void Start () {
		numeroImagem = 1;
	}
	
	// Update is called once per frame
	void Update () {
		timerToSkip += Time.deltaTime * 1;
		if (timerToSkip >= 2f) {
				numeroImagem++;
				timerToSkip = 0f;
		}

		if (numeroImagem == 1) {
			bbLogo.SetActive (true);
			cimologo.SetActive (false);
			joystick.SetActive (false);
			aviso1.SetActive (false);
			aviso2.SetActive (false);
			flowerLoading.SetActive (false);
		}else if (numeroImagem == 2) {
			bbLogo.SetActive (false);
			cimologo.SetActive (true);
			joystick.SetActive (false);
			aviso1.SetActive (false);
			aviso2.SetActive (false);
			flowerLoading.SetActive (false);
		}
		else if (numeroImagem == 3) {
			if(extendCimoTime){
				extendCimoTime = false;
				numeroImagem = 2;
				timerToSkip = 1f;
				return;
			}
			bbLogo.SetActive (false);
			cimologo.SetActive (false);
			joystick.SetActive (true);
			aviso1.SetActive (true);
			aviso2.SetActive (false);
			flowerLoading.SetActive (false);
		}else if (numeroImagem == 4) {
			bbLogo.SetActive (false);
			cimologo.SetActive (false);
			joystick.SetActive (false);
			aviso1.SetActive (false);
			aviso2.SetActive (true);
			flowerLoading.SetActive (true);
		}
		if (numeroImagem == 5) {
			SceneManager.LoadScene (1);
		}

	}

}
