using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Task2 : MonoBehaviour
{
	[SerializeField] private Button[] buttons = new Button[4];
	[SerializeField] private List<Task2Question> questions = new List<Task2Question>();

	private TMP_Text[] tmpTexts = new TMP_Text[4];
	private Image[] buttonImages = new Image[4];

	private int questionIndex;
	private int correctAnswerIndex;
	private Color[] colors = { new Color32(189,110, 203, 255), new Color32(166, 253, 132, 255), new Color32(111 , 223, 227, 255), new Color32(250, 219, 110, 255) };

	private void Awake()
	{
		print(colors[1]);

		for (int i = 0; i < buttons.Length; i++)
		{
			tmpTexts[i] = buttons[i].GetComponentInChildren<TMP_Text>();
			buttonImages[i] = buttons[i].GetComponent<Image>();
		}

		PopulateButtonsData(questions[0]);
	}

	private void OnEnable()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			int j = i;
			buttons[i].onClick.AddListener(() => OnButtonClick(j));
		}
	}

	private void PopulateButtonsData(Task2Question data)
	{
		SetButtonColor();

		int index = Random.Range(0, data.NumberOptions.Count);
		List<string> answers = Utils.Shuffle(data.NumberOptions[index].Options);
		int biggestValue = 0;

		for (int i = 0; i < answers.Count; i++)
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

	private void SetButtonColor()
	{
		colors.Shuffle();

		for (int i = 0; i < buttonImages.Length; i++)
		{
			buttonImages[i].color = colors[i];
		}
	}

	private void OnButtonClick(int index)
	{
		if (string.IsNullOrEmpty(tmpTexts[index].text))
			return;

		if (correctAnswerIndex == index)
			OnCorrectAnswer();

		if (questionIndex >= questions.Count - 1)
		{
			TaskHandler.Instance.CompleteTask();
			return;
		}

		NextQuestion();
	}

	private void NextQuestion()
	{
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
