using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Task1 : MonoBehaviour
{
	[SerializeField] private Button[] buttons = new Button[9];
	[SerializeField] private List<Task1Question> questions = new List<Task1Question>();

	private int questionIndex;
	private TMP_Text[] tmpTexts;
	private TMP_Text correctAnswerTMP;

	private void Awake()
	{
		tmpTexts = new TMP_Text[buttons.Length];

		for (var i = 0; i < buttons.Length; i++)
		{
			tmpTexts[i] = buttons[i].GetComponentInChildren<TMP_Text>();
		}
	}

	private void OnEnable()
	{
		PopulateFieldData(questions[0]);

		for (int i = 0; i < buttons.Length; i++)
		{
			int j = i;
			buttons[i].onClick.AddListener(() => OnButtonClick(j));
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
		if (string.IsNullOrEmpty(tmpTexts[index].text))
			return;

		if (tmpTexts[index] == correctAnswerTMP)
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

		TMP_Text[] shuffledTexts = Utils.NewShuffled(tmpTexts);

		for (int i = 0; i < answers.Count; i++)
		{
			if (!string.IsNullOrEmpty(answers[i]))
			{
				shuffledTexts[i].text = answers[i];

				int x = int.Parse(answers[i]);
				if (x > biggestValue)
				{
					biggestValue = x;
					correctAnswerTMP = shuffledTexts[i];
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
		for (int i = 0; i < buttons.Length; i++)
		{
			int j = i;
			buttons[i].onClick.RemoveListener(() => OnButtonClick(j));
		}
	}
}
