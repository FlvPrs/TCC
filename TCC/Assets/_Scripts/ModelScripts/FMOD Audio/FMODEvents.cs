using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODEvents : MonoBehaviour {

	[FMODUnity.EventRef]
	public string HarpaPuloEvent;
	FMOD.Studio.EventInstance HarpaPulo;

	void Start () {
		HarpaPulo = FMODUnity.RuntimeManager.CreateInstance (HarpaPuloEvent);
		HarpaPulo.set3DAttributes (FMODUnity.RuntimeUtils.To3DAttributes (gameObject));
		HarpaPulo.start ();
		print ("HarpaPuloEvent");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
