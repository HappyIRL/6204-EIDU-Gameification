using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Task3 : MonoBehaviour, IKeyboard
{
	[SerializeField] private List<Task3Question> questions = new List<Task3Question>();
	[SerializeField] private TMP_Text[] tmpTexts = new TMP_Text[4];
	[SerializeField] private Button replayButton;

	private string correctAnswer = "";
	private int numberCountNeeded;
	private int questionIndex;
	private TMP_Text answerText;
	private List<int> savedNumbers = new();
	private bool canListen;

	private void Awake()
	{
		PopulateFieldData(questions[0]);

		replayButton.onClick.AddListener(PlayQuestionAudio);
	}

	private void PopulateFieldData(Task3Question data)
	{
		string[] answers = data.NumberSequence;
		int index = Random.Range(0, data.NumberSequence.Length);

		for (int i = 0; i < answers.Length; i++)
		{
			tmpTexts[i].text = answers[i];
		}

		answerText = tmpTexts[index];
		correctAnswer = answerText.text;
		numberCountNeeded = answerText.text.Length;
		answerText.text = "";
		canListen = true;

		PlayQuestionAudio();
	}

	private void PlayQuestionAudio()
	{
		FMODUnity.RuntimeManager.PlayOneShot($"event:/VO/VO Type Answer");
	}

	private void OnCorrectAnswer()
	{
		print("yay");
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

	private void ResetAnswerData()
	{
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

		StartCoroutine(NextQuestion());
	}

	public void SelectNumber(int n)
	{
		if (!canListen)
			return;

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

	private void OnDisable()
	{
		replayButton.onClick.RemoveAllListeners();
	}
}
