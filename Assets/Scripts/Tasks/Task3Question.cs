using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Task3Data")]
public class Task3Question : ScriptableObject
{
	[SerializeField] private string[] numberSequence;
	public string[] NumberSequence => numberSequence;

}
