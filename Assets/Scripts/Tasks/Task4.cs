using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Task4 : MonoBehaviour, IKeyboard
{
	[SerializeField] private TMP_Text equationTmpText;
	[SerializeField] private TMP_Text answerText;
	[SerializeField, InlineEditor, TableList] private List<Task4Question> questions = new List<Task4Question>();
	[SerializeField] private Button replayButton;


	private List<int> savedNumbers = new();
	private string correctAnswer = "";
	private int numberCountNeeded;
	private int questionIndex;
	private bool canListen;
	private List<string> taskData = new List<string>();
	private string currentQuestionContent;

	private Coroutine repeatCoroutine;
	private int repeatCount;
	private int wrongAnswerCount;



	private void OnEnable()
	{
		StartCoroutine(NextQuestion(0));

		FMODUnity.RuntimeManager.PlayOneShot($"event:/VO/VO Type Answer");

		replayButton.onClick.AddListener(RepeatQuestionAudio);
	}

	private IEnumerator NextQuestion(int secondsToWait)
	{
		repeatCount = 0;

		if (repeatCoroutine != null)
			StopCoroutine(repeatCoroutine);

		canListen = false;

		if(secondsToWait > 0)
			yield return new WaitForSeconds(secondsToWait);

		if (questionIndex >= questions.Count - 1)
		{
			if (!TaskHandler.Instance.IsLastTask())
				yield return StartCoroutine(StartAndAwaitAudioClipFinish("VO/VO Great"));

			TaskHandler.Instance.CompleteTask(taskData);
			yield break;
		}

		ResetAnswerData();

		PopulateFieldData(questions[questionIndex + 1]);
		questionIndex++;

		repeatCoroutine = StartCoroutine(RepeatInterval());
	}

	private IEnumerator StartAndAwaitAudioClipFinish(string audioClip)
	{
		FMOD.Studio.EventInstance fModInstance = FMODUnity.RuntimeManager.CreateInstance($"event:/{audioClip}");
		fModInstance.start();

		PLAYBACK_STATE state = PLAYBACK_STATE.PLAYING;

		while (state != PLAYBACK_STATE.STOPPED)
		{
			fModInstance.getPlaybackState(out state);
			yield return null;
		}

		fModInstance.release();
	}

	private void PopulateFieldData(Task4Question data)
	{
		string equation;

		if (data.IsSubstraction)
		{
			equation = $"{data.FirstNumber} - {data.SecondNumber} =";
			correctAnswer = (data.FirstNumber - data.SecondNumber).ToString();
		}
		else
		{
			equation = $"{data.FirstNumber} + {data.SecondNumber} =";
			correctAnswer = (data.FirstNumber + data.SecondNumber).ToString();
		}

		equationTmpText.text = equation;
		currentQuestionContent = equation;

		numberCountNeeded = correctAnswer.Length;
		canListen = true;
	}

	private void RepeatQuestionAudio()
	{
		if (repeatCount > 2)
		{
			FailQuestion();
			return;
		}

		FMODUnity.RuntimeManager.PlayOneShot($"event:/VO/VO Type Answer");
		repeatCount++;
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
		if (!canListen)
			return;

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
		if (repeatCoroutine != null)
			StopCoroutine(repeatCoroutine);

		string answer = "";

		for (int i = 0; i < savedNumbers.Count; i++)
		{
			answer = answer + savedNumbers[i];
		}

		bool isCorrectAnswer = false;

		if (correctAnswer.Contains(answer))
		{
			OnCorrectAnswer();
			isCorrectAnswer = true;
		}
		else
		{
			wrongAnswerCount++;
		}

		taskData.Add($"M.1.4-5.{questionIndex}, {currentQuestionContent}, {correctAnswer}, {isCorrectAnswer}, {answer}");

		CheckForAnswers();

		StartCoroutine(NextQuestion(1));
	}

	private IEnumerator RepeatInterval()
	{
		if (repeatCount > 2)
		{
			FailQuestion();
			yield break;
		}

		yield return new WaitForSeconds(7f);
		RepeatQuestionAudio();
		yield return new WaitForSeconds(7f);
		RepeatQuestionAudio();
		yield return new WaitForSeconds(7f);

		FailQuestion();
	}

	private void FailQuestion()
	{
		taskData.Add($"M.1.4-5.{questionIndex}, {currentQuestionContent}, {correctAnswer}, false, N/G");
		wrongAnswerCount++;
		CheckForAnswers();
		StartCoroutine(NextQuestion(1));
	}

	private void CheckForAnswers()
	{
		if (wrongAnswerCount > 1 && questionIndex != questions.Count - 1)
		{
			questionIndex += 1;

			while (questionIndex < questions.Count - 1)
			{
				taskData.Add($"M.1.4-5.{questionIndex}, , , ,SKIPPED");
				questionIndex++;
			}
		}
	}

	private void ResetAnswerData()
	{
		currentQuestionContent = "";
		savedNumbers.Clear();

		if(answerText != null)
			answerText.text = "";
	}

	private void OnCorrectAnswer()
	{
		print("yay");
		StartCoroutine(TaskHandler.Instance.SummonStar());
	}

	private void OnDisable()
	{
		StopAllCoroutines();

		replayButton.onClick.RemoveAllListeners();
	}
}
