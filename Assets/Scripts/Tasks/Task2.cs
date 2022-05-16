using System.Collections.Generic;using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Task2 : MonoBehaviour
{
	[SerializeField] private Button[] buttons = new Button[4];
	[SerializeField] private List<Task2Question> questions = new List<Task2Question>();

	private TMP_Text[] tmpTexts = new TMP_Text[4];

	private int questionIndex;
	private int correctAnswerIndex;

	private void Awake()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			tmpTexts[i] = buttons[i].GetComponentInChildren<TMP_Text>();
		}
	}

	private void OnEnable()
	{
		PopulateButtonsData(questions[0]);

		for (int i = 0; i < buttons.Length; i++)
		{
			int j = i;
			buttons[i].onClick.AddListener(() => OnButtonClick(j));
		}
	}

	private void PopulateButtonsData(Task2Question data)
	{
		string[] answers = Utils.NewShuffled(data.NumberOptions);
		float biggestValue = Mathf.NegativeInfinity;

		for (int i = 0; i < answers.Length; i++)
		{
			tmpTexts[i].text = answers[i];

			if (!string.IsNullOrEmpty(answers[i]))
			{
				int x = int.Parse(answers[i]);
				if (x > biggestValue)
				{
					biggestValue = x;
					correctAnswerIndex = i;
				}
			}
		}
	}

	private void OnButtonClick(int index)
	{
		if (string.IsNullOrEmpty(tmpTexts[index].text))
			return;

		if (correctAnswerIndex == index)
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
		for (int i = 0; i < buttons.Length; i++)
		{
			int j = i;
			buttons[i].onClick.RemoveListener(() => OnButtonClick(j));
		}
	}
}
