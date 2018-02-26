using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlePuzzleAto1 : MonoBehaviour {
	public bool polem;
	public bool agua;
	public bool polem_agua;
	public int tipoDeRecurso;
	public bool Completo;
	private int Teste = 0;
	private Color corFlor;
	private Color Azul;
	private Color Amarelo;
	private Color Cinza;
	private Color Verde;
	private Color Inicial;
	private Color Final;
	private Renderer rend;
	private float duracao = 3.0f;
	// Use this for initialization
	void Start () {
		Completo = false;
		tipoDeRecurso = 0;
		rend = GetComponent<Renderer>();
		Azul = Color.blue;
		Amarelo = Color.yellow;
		Cinza = Color.grey;
		Verde = Color.green;
		Final = Verde;
		PuzzleFlor ();
		rend.material.color = Inicial;
	}
		
	void PuzzleFlor(){
		if (agua) {
			Inicial = Azul;
			if (tipoDeRecurso == 1) {
				Completo = true;
			}
		} else if (polem) {
			Inicial = Amarelo;
			if (tipoDeRecurso == 2) {
				Completo = true;
			}
		} else if (polem_agua) {
			Inicial = Cinza;
			if (tipoDeRecurso == 1) {
				polem = true;
				polem_agua = false;
			} else if (tipoDeRecurso == 2) {
				agua = true;
				polem_agua = false;
			}
		}
		if (Completo && Teste == 0) {
			Teste++;
			MudançaDeCor ();
		}

	}
	void MudançaDeCor(){
		float lerp = Mathf.PingPong (Time.time, duracao) / duracao;
		rend.material.color = Color.Lerp (Inicial, Final, lerp);
	}
	void OnTriggerEnter(Collider colisor){
		if (colisor.name == "PlayerCollider") {
			tipoDeRecurso = 2;
			PuzzleFlor ();
		} else if (colisor.name == "agua") {
			tipoDeRecurso = 1;
			PuzzleFlor ();
			}
	}
}
