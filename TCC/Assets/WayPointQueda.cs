using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointQueda : MonoBehaviour {
	public quedaCollider colisorQueda;
	private Vector3 distPlayerFather;
	// Use this for initialization
	void Start () {
		distPlayerFather = new Vector3 (0, -200f, 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void GetPlayerPosition(Vector3 posicaoJogador){
		gameObject.transform.position = posicaoJogador + distPlayerFather;
	}
}
