using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Task3Data")]
public class Task3Question : ScriptableObject
{
	[SerializeField] private List<String4> m_NumberSequences;
	public List<String4> NumberSequences => m_NumberSequences;

}
