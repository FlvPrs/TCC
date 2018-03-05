using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionMethods
{
	public static int[] SeparateIntToArray (this int integer){
		List<int> separatedNumbers = new List<int>();
		while (integer != 0) {
			separatedNumbers.Insert (0, integer % 10);
			integer /= 10;
		}
		return separatedNumbers.ToArray ();
	}
}