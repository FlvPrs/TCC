using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFatherStaccatoInteractable {

	/// <summary>
	/// É chamada no primeiro frame que este behaviour detectar que o pai tocou uma nota em staccato.
	/// </summary>
	/// <param name="song">Song.</param>
	void FatherStaccatoInteraction(PlayerSongs song);

}
