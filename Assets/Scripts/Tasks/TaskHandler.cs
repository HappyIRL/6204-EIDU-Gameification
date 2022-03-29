using System.Collections.Generic;
using UnityEngine;

public class TaskHandler : MonoBehaviour
{
	[SerializeField] private List<GameObject> tasksInOrder = new List<GameObject>();
	[SerializeField] private GameObject keyboardGO;

	private Keyboard keyboard;

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

		keyboard = keyboardGO.GetComponent<Keyboard>();
	}

	private void Start()
	{
		if (tasksInOrder.Count == 0)
		{
			Debug.LogError($"List of tasksInOrder needs to be populated, at: {this}" );
		}

		tasksInOrder[0].SetActive(true);
	}

	public void CompleteTask()
	{
		GameObject taskGO = tasksInOrder[activeTask];
		taskGO.SetActive(false);

		if (activeTask >= tasksInOrder.Count - 1)
		{
			OnAllTasksComplete();
			return;
		}

		GameObject nextTaskGO = tasksInOrder[activeTask + 1];
		nextTaskGO.SetActive(true);

		if (nextTaskGO.TryGetComponent(out IKeyboard user))
		{
			keyboardGO.SetActive(true);
			keyboard.SetUser(user);
		}
		else
		{
			keyboardGO.SetActive(false);
		}

		activeTask++;
	}

	private void OnAllTasksComplete()
	{
		Debug.Log("All tasks complete");
	}
}
