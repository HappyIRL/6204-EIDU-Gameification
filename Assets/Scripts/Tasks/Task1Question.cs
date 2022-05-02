using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Task1Data")]
public class Task1Question : ScriptableObject
{
	[SerializeField] private List<string> options = new List<string>();
	public List<string> Options => options;
}
