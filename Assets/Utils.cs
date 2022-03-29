using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public static class Utils
{
	public static void Shuffle<T>(this T[] originalArray)
	{
		Random rng = new Random();

		int n = originalArray.Length;
		while (n > 1)
		{
			int k = rng.Next(n--);
			(originalArray[n], originalArray[k]) = (originalArray[k], originalArray[n]);
		}
	}

	public static List<T> Shuffle<T>(List<T> originalArray)
	{
		Random rng = new Random();

		int n = originalArray.Capacity;
		while (n > 1)
		{
			int k = rng.Next(n--);
			(originalArray[n], originalArray[k]) = (originalArray[k], originalArray[n]);
		}

		return originalArray;
	}
}
