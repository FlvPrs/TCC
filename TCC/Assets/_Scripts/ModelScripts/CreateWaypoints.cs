using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CreateWaypoints : MonoBehaviour {

	public GameObject wpPrefab;
	public bool closeLoop = false;
	public int wpAmount = 0;
	public bool update = false;

	[HideInInspector]
	public GameObject[] waypoints = new GameObject[0];

	void Update(){
		if (!update)
			return;
		else
			update = false;

		int count = transform.childCount;

		//Salva as posições dos waypoints atuais em Temp
		GameObject[] temp = new GameObject[count];
		for (int i = count - 1; i >= 0; i--) {
			temp [i] = transform.GetChild (i).gameObject;
		}

		//Se o numero de filhos for maior do que o numero de waypoints desejados, destrua os excedentes armazenados em Temp
		for (int i = wpAmount; i < temp.Length; i++) {
			DestroyImmediate (temp[i]);
		}

		//Redefinindo o array
		waypoints = new GameObject[wpAmount];

		for (int i = 0; i < wpAmount; i++) {
			if(i < count){
				waypoints [i] = temp[i];
			} else {
				waypoints [i] = Instantiate (wpPrefab, transform);
			}
			waypoints [i].name = "WP " + i;

			if (i - 1 >= 0)
				waypoints [i - 1].GetComponent<WaypointControl> ().next = waypoints [i].transform;

			if(i + 1 >= wpAmount){
				if(closeLoop)
					waypoints [i].GetComponent<WaypointControl> ().next = waypoints[0].transform;
				else
					waypoints [i].GetComponent<WaypointControl> ().next = null;
			}

		}
	}
}
