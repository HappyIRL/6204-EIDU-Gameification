using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Task2Data")]
public class Task2Question : ScriptableObject
{
	[SerializeField] private string[] numberOptions;
	public string[] NumberOptions => numberOptions;
}
