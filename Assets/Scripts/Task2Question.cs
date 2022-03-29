using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Task2Data")]
public class Task2Question : ScriptableObject
{
	[SerializeField] private List<String4> numberOptions;
	public List<String4> NumberOptions => numberOptions;
}

[Serializable]
public struct String4
{
	public List<string> Options;

	public String4(string a, string b, string c, string d)
	{
		Options = new List<string>
		{
			a,b,c,d
		};
	}
}
