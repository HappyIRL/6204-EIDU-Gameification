using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Task1 : MonoBehaviour
{
	[SerializeField] private TaskField[] taskFields = new TaskField[9];
	[SerializeField] private List<Task1Question> questions = new List<Task1Question>();

	private int questionIndex;
	private TMP_Text correctAnswerTMP;

	private void Start()
	{
		PopulateFieldData(questions[0]);

		for (int i = 0; i < taskFields.Length; i++)
		{
			int j = i;
			taskFields[i].Button.onClick.AddListener(() => OnButtonClick(j));
		}
	}

	private void NextQuestion()
	{
		if (questionIndex >= questions.Count - 1)
		{
			TaskHandler.Instance.CompleteTask();
			return;
		}

		PopulateFieldData(questions[questionIndex + 1]);
		questionIndex++;
	}

	private void OnButtonClick(int index)
	{
		if (string.IsNullOrEmpty(taskFields[index].TmpText.text))
			return;

		if (taskFields[index].TmpText == correctAnswerTMP)
			OnCorrectAnswer();

		NextQuestion();
	}

	private void OnCorrectAnswer()
	{
		print("yay");
	}

	private void PopulateFieldData(Task1Question data)
	{
		List<string> answers = data.Options;
		int biggestValue = 0;

		TaskField[] shuffledTexts = Utils.NewShuffled(taskFields);

		for (int i = 0; i < answers.Count; i++)
		{
			if (!string.IsNullOrEmpty(answers[i]))
			{
				shuffledTexts[i].TmpText.text = answers[i];
				shuffledTexts[i].HasContext();

				int x = int.Parse(answers[i]);
				if (x > biggestValue)
				{
					biggestValue = x;
					correctAnswerTMP = shuffledTexts[i].TmpText;
				}
			}
			else
			{
				Debug.LogError($"Options for answers can't be empty. At: {data}");
			}
		}
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
