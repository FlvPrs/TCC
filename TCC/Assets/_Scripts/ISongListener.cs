﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISongListener {

	void DetectSong (PlayerSongs song, bool isSingingSomething, bool isFather = false, HeightState height = HeightState.Default);
}
