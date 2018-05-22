using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlatformMovable {

	void OnMovingPlat (bool enableNavMesh, Transform plat);
}
