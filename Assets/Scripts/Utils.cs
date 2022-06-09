using System;
using System.Collections.Generic;
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

	public static T[] NewShuffled<T>(T[] originalArray)
	{
		Random rng = new Random();
		T[] newArray = (T[])originalArray.Clone();

		int n = newArray.Length;
		while (n > 1)
		{
			int k = rng.Next(n--);
			(newArray[n], newArray[k]) = (newArray[k], newArray[n]);
		}

		return newArray;
	}

	public static List<T> NewShuffled<T>(List<T> originalList)
	{
		List<T> newList = new List<T>(originalList);

		Random rng = new Random();

		int n = newList.Count;
		while (n > 1)
		{
			int k = rng.Next(n--);
			(newList[n], newList[k]) = (newList[k], newList[n]);
		}

		return newList;
	}

	/// <summary>
	/// if buttonFlipped is false, closes button, if true, opens button
	/// </summary>
	public static void ButtonFlip(GameObject target, bool buttonFlipped = false)
    {
        if (!buttonFlipped)
        {
			// close button
        }
        else
        {
			// open button
        }
    }
}
