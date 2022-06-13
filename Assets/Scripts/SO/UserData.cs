using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/UserData")]
public class UserData : ScriptableObject
{
	[HideInInspector]
	public string StudentInfo;
}
