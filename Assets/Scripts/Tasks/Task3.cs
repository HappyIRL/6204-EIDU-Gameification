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

	private void OnEnable()
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
		StartCoroutine(TaskHandler.Instance.SummonStar());
	}

	private IEnumerator NextQuestion()
	{
		canListen = false;

		yield return new WaitForSeconds(1);

		if (questionIndex >= questions.Count - 1)
		{
			yield return StartCoroutine(StartAndAwaitAudioClipFinish("VO/VO New One"));
			TaskHandler.Instance.CompleteTask();
			yield break;
		}

		ResetAnswerData();

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
		replayButton.onClick.RemoveAllListeners();
	}
}
