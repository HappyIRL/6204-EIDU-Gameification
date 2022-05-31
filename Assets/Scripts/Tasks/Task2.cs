using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Task2 : MonoBehaviour
{
	[SerializeField] private TaskButton[] taskFields = new TaskButton[4];
	[SerializeField] private List<Task2Question> questions = new List<Task2Question>();
	[SerializeField] private Button replayButton;

	private int questionIndex;
	private int correctAnswerIndex;

	private void Start()
	{
		PopulateButtonsData(questions[0]);

		replayButton.onClick.AddListener(PlayQuestionAudio);


		for (int i = 0; i < taskFields.Length; i++)
		{
			int j = i;
			taskFields[i].AddListener(() => OnButtonClick(j));
		}
	}

	private void PopulateButtonsData(Task2Question data)
	{
		string[] answers = Utils.NewShuffled(data.NumberOptions);
		float biggestValue = Mathf.NegativeInfinity;

		for (int i = 0; i < answers.Length; i++)
		{
			if (!string.IsNullOrEmpty(answers[i]))
			{
				taskFields[i].SetContext(answers[i]);

				int x = int.Parse(answers[i]);

				if (x > biggestValue)
				{
					biggestValue = x;
					correctAnswerIndex = i;
				}
			}
		}

		PlayQuestionAudio();
	}
	private void PlayQuestionAudio()
	{
		FMODUnity.RuntimeManager.PlayOneShot($"event:/VO/VO Bigest Number");
	}

	private void OnButtonClick(int i)
	{
		if (string.IsNullOrEmpty(taskFields[i].TmpText.text))
			return;

		if (correctAnswerIndex == i)
			OnCorrectAnswer();

		NextQuestion();
	}

	private void NextQuestion()
	{
		if (questionIndex >= questions.Count - 1)
		{
			TaskHandler.Instance.CompleteTask();
			return;
		}

		PopulateButtonsData(questions[questionIndex + 1]);
		questionIndex++;
	}

	private void OnCorrectAnswer()
	{
		print("yay");
	}

	private void OnDisable()
	{
		replayButton.onClick.RemoveAllListeners();

		for (int i = 0; i < taskFields.Length; i++)
		{
			int j = i;
			taskFields[i].RemoveListener(() => OnButtonClick(j));
		}
	}
}
