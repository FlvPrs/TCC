using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentiladorAutomatico_Ctrl : MonoBehaviour {

	GameObject vento;

	public float auto_OnOffTimer = 2f;
	[Range(0.01f, 10f)]
	public float auto_OnOffRatio = 1f; 	//É multiplicado por auto_onOffTimer. O resultado indica quanto tempo ficará ligada.
										//1 significa o mesmo tempo ligado e desligado. 
										//2 significa que ficará ligado 2 vezes mais do que desligado. 
										//0.5 significa que ficará 2 vezes mais desligado do que ligado.
	public float delayAutoBy = 0f;
	float delayTimer;
	bool canStartVentiladorAutomatico;
	float ventiladorCooldown;

	// Use this for initialization
	void Start () {
		vento = transform.GetChild (0).gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (delayTimer <= 0f) {
			ventiladorCooldown += Time.deltaTime;
			if (ventiladorCooldown <= auto_OnOffRatio * auto_OnOffTimer || auto_OnOffTimer == 0f) {
				vento.SetActive(true);
			} else if (ventiladorCooldown <= auto_OnOffTimer + (auto_OnOffRatio * auto_OnOffTimer)) {
				vento.SetActive(false);
			} else {
				ventiladorCooldown = 0f;
			}
		} else {
			delayTimer -= Time.deltaTime;
		}
	}
}
