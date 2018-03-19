using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFatherSustainInteractable {

	/// <summary>
	/// É chamada todo frame que este behaviour detectar que o Pai está tocando nota em sustain.
	/// </summary>
	/// <param name="song">Song.</param>
	void FatherSustainInteraction(PlayerSongs song);

}
