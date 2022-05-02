using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Task4 : MonoBehaviour, IKeyboard
{
	[SerializeField] private TMP_Text equationTmpText;
	[SerializeField] private TMP_Text answerText;
	[SerializeField] private List<Task4Question> questions = new List<Task4Question>();


	private List<int> savedNumbers = new List<int>();
	private string correctAnswer = "";
	private int numberCountNeeded;
	private int questionIndex;
	private string answerGiven = "";

	private void OnEnable()
	{
		PopulateFieldData(questions[0]);
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

	private void PopulateFieldData(Task4Question data)
	{
		if (data.IsSubstraction)
		{
			equationTmpText.text = $"{data.FirstNumber} - {data.SecondNumber} =";
			correctAnswer = (data.FirstNumber - data.SecondNumber).ToString();
		}
		else
		{
			equationTmpText.text = $"{data.FirstNumber} + {data.SecondNumber} =";
			correctAnswer = (data.FirstNumber + data.SecondNumber).ToString();
		}

		numberCountNeeded = correctAnswer.Length;

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

	private void ResetAnswerData()
	{
		answerGiven = "";
		savedNumbers.Clear();
		answerText.text = "";
	}

	private void OnCorrectAnswer()
	{
		print("yay");
	}
}
