using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Task4 : MonoBehaviour, IKeyboard
{
	[SerializeField] private TMP_Text equationTmpText;
	[SerializeField] private TMP_Text answerText;
	[SerializeField] private List<Task4Question> questions = new List<Task4Question>();


	private List<int> savedNumbers = new();
	private string correctAnswer = "";
	private int numberCountNeeded;
	private int questionIndex;
	private bool canListen;


	private void OnEnable()
	{
		PopulateFieldData(questions[0]);
	}

	private IEnumerator NextQuestion()
	{
		canListen = false;
		yield return new WaitForSeconds(1);
		ResetAnswerData();

		if (questionIndex >= questions.Count - 1)
		{
			TaskHandler.Instance.CompleteTask();
			yield break;
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
		canListen = true;

	}

	public void SelectNumber(int n)
	{
		savedNumbers.Add(n);
		answerText.text = answerText.text + n;

		if (savedNumbers.Count == numberCountNeeded)
		{
			CheckAnswer();
		}
	}

	public void UndoNumber()
	{
		if (savedNumbers.Count != 0)
		{
			savedNumbers.RemoveAt(savedNumbers.Count - 1);

			Char[] items = answerText.text.ToCharArray();
			Array.Resize(ref items, items.Length - 1);

			answerText.text = new string(items);
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

		StartCoroutine(NextQuestion());
	}

	private void ResetAnswerData()
	{
		savedNumbers.Clear();
		answerText.text = "";
	}

	private void OnCorrectAnswer()
	{
		print("yay");
	}
}
