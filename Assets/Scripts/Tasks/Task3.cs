using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Task3 : MonoBehaviour, IKeyboard
{
	[SerializeField] private List<Task3Question> questions = new List<Task3Question>();
	[SerializeField] private Image[] fields = new Image[4];

	private TMP_Text[] tmpTexts = new TMP_Text[4];

	private string correctAnswer = "";
	private int numberCountNeeded;
	private int questionIndex;
	private string answerGiven = "";
	private TMP_Text answerText;
	private List<int> savedNumbers = new List<int>();

	private Color[] colors = { new Color32(189, 110, 203, 255), new Color32(166, 253, 132, 255), new Color32(111, 223, 227, 255), new Color32(250, 219, 110, 255) };

	private void Awake()
	{
		for (int i = 0; i < fields.Length; i++)
		{
			tmpTexts[i] = fields[i].GetComponentInChildren<TMP_Text>();
		}

		PopulateFieldData(questions[0]);
	}

	private void PopulateFieldData(Task3Question data)
	{
		SetButtonColor();
		int index = Random.Range(0, data.NumberSequences.Count);
		List<string> answers = data.NumberSequences[index].Options;
		index = Random.Range(0, data.NumberSequences[index].Options.Count);

		for (int i = 0; i < answers.Count; i++)
		{
			tmpTexts[i].text = answers[i];
		}

		answerText = tmpTexts[index];
		correctAnswer = answerText.text;
		numberCountNeeded = answerText.text.Length;
		answerText.text = "";
	}

	private void OnCorrectAnswer()
	{
		print("yay");
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

	private void SetButtonColor()
	{
		colors.Shuffle();

		for (int i = 0; i < fields.Length; i++)
		{
			fields[i].color = colors[i];
		}
	}

	private void ResetAnswerData()
	{
		answerGiven = "";
		savedNumbers.Clear();
		answerText.text = "";
	}

	private void CheckAnswer()
	{
		string answer = "";

		for (int i = 0; i < savedNumbers.Count; i++)
		{
			answer = answer + savedNumbers[i];
		}

		if (correctAnswer.Contains(answer))
		{
			OnCorrectAnswer();
		}

		ResetAnswerData();
		NextQuestion();
	}

	public void SelectNumber(int n)
	{
		savedNumbers.Add(n);
		answerGiven = answerGiven + n;
		answerText.text = answerGiven;

		if (savedNumbers.Count == numberCountNeeded)
		{
			CheckAnswer();
		}
	}
}
