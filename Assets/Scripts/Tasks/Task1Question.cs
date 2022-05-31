using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Task1Data")]
public class Task1Question : ScriptableObject
{
	[SerializeField] private List<string> options = new List<string>();
	[SerializeField] private string correctAnswer;

	public string CorrectAnswer => correctAnswer;
	public List<string> Options => options;
}
