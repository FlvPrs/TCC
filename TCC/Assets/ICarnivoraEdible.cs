using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICarnivoraEdible {

	void Carnivora_GetReadyToBeEaten ();
	void Carnivora_Release ();
	void Carnivora_Shoot (Vector3 dir);
}
