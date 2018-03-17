using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoteiroSongsList : MonoBehaviour {

	public SongList[] listaDePartituras;
}

[System.Serializable]
public class SongList
{
	public string name;
	public PartituraInfo[] partitura;
}