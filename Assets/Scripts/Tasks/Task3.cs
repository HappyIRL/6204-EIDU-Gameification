using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Task3 : MonoBehaviour, IKeyboard
{
	[SerializeField, InlineEditor, TableList] private List<Task3Question> questions = new List<Task3Question>();
	[SerializeField] private TMP_Text[] tmpTexts = new TMP_Text[4];
	[SerializeField] private Button replayButton;

	private string correctAnswer = "";
	private int numberCountNeeded;
	private int questionIndex;
	private TMP_Text answerText;
	private List<int> savedNumbers = new();
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

	private void PopulateFieldData(Task3Question data)
	{
		string[] answers = data.NumberSequence;
		int index = Random.Range(0, data.NumberSequence.Length);

		for (int i = 0; i < answers.Length; i++)
		{
			tmpTexts[i].text = answers[i];
			currentQuestionContent += answers[i] + ";";
		}

		currentQuestionContent = currentQuestionContent.Remove(currentQuestionContent.Length - 1, 1);
		answerText = tmpTexts[index];
		correctAnswer = answerText.text;
		numberCountNeeded = answerText.text.Length;
		answerText.text = "";
		canListen = true;
	}

	private void RepeatQuestionAudio()
	{
		print("in repeat");

		if (repeatCount > 2)
		{
			FailQuestion();
			return;
		}

		FMODUnity.RuntimeManager.PlayOneShot($"event:/VO/VO Type Answer");
		repeatCount++;
	}

	private void OnCorrectAnswer()
	{
		print("yay");
		StartCoroutine(TaskHandler.Instance.SummonStar());
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
			yield return StartCoroutine(StartAndAwaitAudioClipFinish("VO/VO New One"));
			TaskHandler.Instance.CompleteTask(taskData);
			yield break;
		}

		ResetAnswerData();

		PopulateFieldData(questions[questionIndex + 1]);
		questionIndex++;

		repeatCoroutine = StartCoroutine(RepeatInterval());
	}

	private void ResetAnswerData()
	{
		currentQuestionContent = "";
		savedNumbers.Clear();
		if(answerText != null)
			answerText.text = "";
	}

	private void CheckAnswer()
	{
		string answer = "";

		if (repeatCoroutine != null)
			StopCoroutine(repeatCoroutine);

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

		taskData.Add($"M.1.3.{questionIndex}, {currentQuestionContent}, {correctAnswer}, {isCorrectAnswer}, {answer}");

		CheckForAnswers();

		StartCoroutine(NextQuestion(1));
	}

	private IEnumerator RepeatInterval()
	{
		if(repeatCoroutine != null)
			StopCoroutine(repeatCoroutine);

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

		repeatCoroutine = StartCoroutine(RepeatInterval());
	}

	private void FailQuestion()
	{
		taskData.Add($"M.1.3.{questionIndex}, {currentQuestionContent}, {correctAnswer}, false, N/G");
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
				taskData.Add($"M.1.3.{questionIndex}, , , ,SKIPPED");
				questionIndex++;
			}
		}
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

	private void OnDisable()
	{
		StopAllCoroutines();

		replayButton.onClick.RemoveAllListeners();
	}
}
