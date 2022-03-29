using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskHandler : MonoBehaviour
{
	[SerializeField] private List<GameObject> tasksInOrder = new List<GameObject>();

	public static TaskHandler Instance { get; private set; }

	private int activeTask;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
		{
			Debug.LogError($"Multiple instance of singleton: {this}");
		}
	}

	private void Start()
	{
		if (tasksInOrder.Count == 0)
		{
			Debug.LogError($"List of tasksInOrder needs to be populated, at: {this}" );
		}
	}

	public void CompleteTask()
	{
		if (activeTask >= tasksInOrder.Count - 1)
		{
			OnAllTasksComplete();
			return;
		}

		tasksInOrder[activeTask].SetActive(false);
		tasksInOrder[activeTask + 1].SetActive(true);
		activeTask++;
	}

	private void OnAllTasksComplete()
	{

	}
}
