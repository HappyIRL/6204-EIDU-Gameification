using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskHandler : MonoBehaviour
{
	[SerializeField] private List<GameObject> tasksInOrder = new List<GameObject>();
	[SerializeField] private GameObject keyboardGO;
	[SerializeField] private ProgressionMap progressMap;
	[SerializeField] private GameObject finishScreen;
	[SerializeField] private Button restartButton;
	[SerializeField] private GameObject goldStar;
    [SerializeField, Range(0.1f, 3f)] private float starTimer;
	
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

		NextTask(false);
	}

	public void CompleteTask()
	{
		GameObject taskGO = tasksInOrder[activeTask];
		taskGO.SetActive(false);
		keyboardGO.SetActive(false);

		if (IsLastTask())
		{
			OnAllTasksComplete();
			return;
		}

		NextTask(true);
	}

	public bool IsLastTask()
	{
		if (activeTask >= tasksInOrder.Count - 1)
		{
			return true;
		}

		return false;
	}

	private void NextTask(bool isProgression)
	{
		if(isProgression)
			StartCoroutine(progressMap.NextStep(ShowNextTask));
		else
			ShowNextTask();
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

	public void RestartTest()
	{
		finishScreen.SetActive(false);
		activeTask = 0;
		NextTask(false);
	}

	private void OnAllTasksComplete()
	{
		FMODUnity.RuntimeManager.PlayOneShot("event:/VO/VO Completed Test");
		finishScreen.SetActive(true);
	}

	public IEnumerator SummonStar()
    {
		goldStar.SetActive(true);
		yield return new WaitForSeconds(starTimer);
		goldStar.SetActive(false );
		yield return null;
    }
}
