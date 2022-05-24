using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Task2 : MonoBehaviour
{
	[SerializeField] private TaskField[] taskFields = new TaskField[4];
	[SerializeField] private List<Task2Question> questions = new List<Task2Question>();

	private int questionIndex;
	private int correctAnswerIndex;

	private void Start()
	{
		PopulateButtonsData(questions[0]);

		for (int i = 0; i < taskFields.Length; i++)
		{
			int j = i;
			taskFields[i].Button.onClick.AddListener(() => OnButtonClick(j));
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
				taskFields[i].TmpText.text = answers[i];
				taskFields[i].HasContext();
				int x = int.Parse(answers[i]);
				if (x > biggestValue)
				{
					biggestValue = x;
					correctAnswerIndex = i;
				}
			}
		}
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
		for (int i = 0; i < taskFields.Length; i++)
		{
			int j = i;
			taskFields[i].Button.onClick.RemoveListener(() => OnButtonClick(j));
		}
	}
}
