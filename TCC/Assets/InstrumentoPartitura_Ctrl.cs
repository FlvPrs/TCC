using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PartituraState { Empty, High, Medium, Low }

/// <summary>
/// A função deste script é popular o instrumento com as notas definidas no Inspector.
/// </summary>
public class InstrumentoPartitura_Ctrl : MonoBehaviour {

	public GameObject[] noteType;

	[Tooltip("Notas Empty ocupam o espaço de uma nota, mas não contam como notas reais (não precisam ser tocadas). " +
		"Deve ser menor do que a quantidade de elementos em \"partitura\".")]
	public int start_AddEmptyNotes = 0;

	public Transform pauta;
	public PartituraState[] partitura;


	void Start () {
		//float circunference = 2 * Mathf.PI * 15f;
		float individualRot = -360f / (partitura.Length + start_AddEmptyNotes);

		for (int i = start_AddEmptyNotes; i < partitura.Length; i++) {
			GameObject nota;

			switch (partitura[i]) {
			case PartituraState.High:
				nota = Instantiate (noteType [2]);
				nota.transform.SetParent (pauta);
				nota.transform.localPosition = Vector3.zero;
				break;
			case PartituraState.Medium:
				nota = Instantiate (noteType [1]);
				nota.transform.SetParent (pauta);
				nota.transform.localPosition = Vector3.zero;
				break;
			case PartituraState.Low:
				nota = Instantiate (noteType [0]);
				nota.transform.SetParent (pauta);
				nota.transform.localPosition = Vector3.zero;
				break;
			default:
				continue;
			}

			Quaternion newRot = new Quaternion ();
			newRot.eulerAngles = Vector3.up * individualRot * i;
			nota.transform.localRotation = newRot;
		}

		for (int i = 0; i < noteType.Length; i++) {
			noteType [i].SetActive (false);
		}
	}
}
