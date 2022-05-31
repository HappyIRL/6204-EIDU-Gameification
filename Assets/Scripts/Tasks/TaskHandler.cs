using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskHandler : MonoBehaviour
{
	[SerializeField] private List<GameObject> tasksInOrder = new List<GameObject>();
	[SerializeField] private GameObject keyboardGO;
	[SerializeField] private ProgressionMap progressMap;
	private Keyboard keyboard;

	public static TaskHandler Instance { get; private set; }

	private int activeTask = -1;

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
		{
			Debug.LogError($"Multiple instance of singleton: {this}");
		}

		keyboard = keyboardGO.GetComponent<Keyboard>();
	}

	private void Start()
	{
		if (tasksInOrder.Count == 0)
		{
			Debug.LogError($"List of tasksInOrder needs to be populated, at: {this}" );
		}

		NextTask();
	}

	public void CompleteTask()
	{
		GameObject taskGO = tasksInOrder[activeTask];
		taskGO.SetActive(false);
		keyboardGO.SetActive(false);

		if (activeTask >= tasksInOrder.Count - 1)
		{
			OnAllTasksComplete();
			return;
		}

		NextTask();
	}

	private void NextTask()
	{
		progressMap.NextStep(ShowNextTask);
	}

	private void ShowNextTask()
	{
		GameObject nextTaskGO = tasksInOrder[activeTask + 1];
		nextTaskGO.SetActive(true);

		if (nextTaskGO.TryGetComponent(out IKeyboard user))
		{
			keyboardGO.SetActive(true);
			keyboard.SetUser(user);
		}

		activeTask++;
	}

	private void OnAllTasksComplete()
	{
		keyboardGO.SetActive(false);
		Debug.Log("All tasks complete");
	}
}
