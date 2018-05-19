using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Este script deve ser colocado em cada um dos 3 triggers do instrumento (Alto, Baixo e Médio). Ele me diz se o player interagiu corretamente ou não.
/// </summary>
public class InstrumentoTrigger_Ctrl : MonoBehaviour, ISongListener {

	public PartituraState noteType;
	public bool fatherCanInteract;

	public bool interactedWith = false;
	public bool interagiuCorretamente = false;

	public void DetectSong (PlayerSongs song, bool isSingingSomething, bool isFather = false, HeightState height = HeightState.Default){
		if (isFather && !fatherCanInteract)
			return;
		
		if(isSingingSomething && !interactedWith){
			interactedWith = true;
			switch (noteType) {
			case PartituraState.High:
				if(height == HeightState.High){
					interagiuCorretamente = true;
				}
				break;
			case PartituraState.Medium:
				if(height == HeightState.Default){
					interagiuCorretamente = true;
				}
				break;
			case PartituraState.Low:
				if(height == HeightState.Low){
					interagiuCorretamente = true;
				}
				break;
			default:
				interagiuCorretamente = false;
				break;
			}
		}
	}

	void OnEnable (){
		interactedWith = false;
		interagiuCorretamente = false;
	}
//	void OnDisable (){
//		
//	}
}
