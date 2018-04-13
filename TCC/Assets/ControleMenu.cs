using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControleMenu : MonoBehaviour {
	private float timerToSkip;
	private int numeroImagem;
	public GameObject bbLogo, cimologo, joystick, flowerLoading, aviso1, aviso2;
	// Use this for initialization
	void Start () {
		numeroImagem = 1;
	}
	
	// Update is called once per frame
	void Update () {
		timerToSkip += Time.deltaTime * 1;
		if (timerToSkip >= 3f) {
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
			// Use this for initialization
		}else if (numeroImagem == 2) {
			bbLogo.SetActive (false);
			cimologo.SetActive (false);
			joystick.SetActive (true);
			aviso1.SetActive (true);
			aviso2.SetActive (false);
			flowerLoading.SetActive (false);
		}else if (numeroImagem == 3) {
			bbLogo.SetActive (false);
			cimologo.SetActive (false);
			joystick.SetActive (false);
			aviso1.SetActive (false);
			aviso2.SetActive (true);
			flowerLoading.SetActive (true);
		}
		if (numeroImagem == 4) {
			SceneManager.LoadScene (1);
		}

	}

}
