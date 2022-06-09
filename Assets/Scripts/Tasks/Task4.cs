using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Task4 : MonoBehaviour, IKeyboard
{
	[SerializeField] private TMP_Text equationTmpText;
	[SerializeField] private TMP_Text answerText;
	[SerializeField] private List<Task4Question> questions = new List<Task4Question>();
	[SerializeField] private Button replayButton;


	private List<int> savedNumbers = new();
	private string correctAnswer = "";
	private int numberCountNeeded;
	private int questionIndex;
	private bool canListen;


	private void OnEnable()
	{
		PopulateFieldData(questions[0]);

		replayButton.onClick.AddListener(PlayQuestionAudio);
	}

	private IEnumerator NextQuestion()
	{
		canListen = false;
		yield return new WaitForSeconds(1);

		if (questionIndex >= questions.Count - 1)
		{
			if (!TaskHandler.Instance.IsLastTask())
				yield return StartCoroutine(StartAndAwaitAudioClipFinish("VO/VO Great"));

			TaskHandler.Instance.CompleteTask();
			yield break;
		}

		ResetAnswerData();

		PopulateFieldData(questions[questionIndex + 1]);
		questionIndex++;
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

		PlayQuestionAudio();
	}

	private void PlayQuestionAudio()
	{
		FMODUnity.RuntimeManager.PlayOneShot($"event:/VO/VO Type Answer");
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

	private void OnDisable()
	{
		replayButton.onClick.RemoveAllListeners();
	}
}
