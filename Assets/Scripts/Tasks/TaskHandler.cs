using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private UserData studentInfo;

    private List<List<string>> userTestData = new List<List<string>>();
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

		print(studentInfo.StudentInfo);

		NextTask(false);
	}

	public void CompleteTask(List<string> data)
	{
		if(data != null)
			userTestData.Add(data);

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
		SceneManager.LoadScene(0);
	}

	private void OnAllTasksComplete()
	{
		FMODUnity.RuntimeManager.PlayOneShot("event:/VO/VO Completed Test");

		finishScreen.SetActive(true);

		if (userTestData.Count > 0)
		{
			if (!Directory.Exists($"{Application.persistentDataPath}/UserData"))
			{
				Directory.CreateDirectory($"{Application.persistentDataPath}/UserData");
			}

			string saveName = studentInfo.StudentInfo;

			TextWriter tw = new StreamWriter($"{Application.persistentDataPath}/UserData/{saveName}.csv", false);

			tw.WriteLine("Question Code,Question Content,Desired Answer,Answer Given,IsCorrect?");

			for (int i = 0; i < userTestData.Count; i++)
			{
				for (int j = 0; j < userTestData[i].Count; j++)
				{
					tw.WriteLine(userTestData[i][j]);
				}
			}

			tw.Close();
		}
	}

	public IEnumerator SummonStar()
    {
		goldStar.SetActive(true);
		yield return new WaitForSeconds(starTimer);
		goldStar.SetActive(false );
		yield return null;
    }
}
