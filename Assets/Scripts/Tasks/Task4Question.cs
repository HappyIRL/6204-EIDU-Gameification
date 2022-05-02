using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Task4Data")]
public class Task4Question : ScriptableObject
{
	[SerializeField] private bool isSubstraction = false;
	[SerializeField] private int firstNumber = 0;
	[SerializeField] private int secondNumber = 0;

	public bool IsSubstraction => isSubstraction;
	public int FirstNumber => firstNumber;
	public int SecondNumber => secondNumber;
}
