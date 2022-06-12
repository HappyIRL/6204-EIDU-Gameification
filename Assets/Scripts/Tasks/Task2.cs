using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Task2 : MonoBehaviour
{
	[SerializeField] private TaskButton[] taskFields = new TaskButton[4];
	[SerializeField, InlineEditor, TableList] private List<Task2Question> questions = new List<Task2Question>();
	[SerializeField] private Button replayButton;

	private int questionIndex;
	private int correctAnswerIndex;
	private bool taskCompleted;

	private void OnEnable()
	{
		PopulateButtonsData(questions[0]);

		replayButton.onClick.AddListener(PlayQuestionAudio);


		for (int i = 0; i < taskFields.Length; i++)
		{
			int j = i;
			taskFields[i].AddListener(() => OnButtonClick(j));
		}
	}

	private void PopulateButtonsData(Task2Question data)
	{
		ResetFieldData();

		string[] answers = Utils.NewShuffled(data.NumberOptions);
		float biggestValue = Mathf.NegativeInfinity;

		for (int i = 0; i < answers.Length; i++)
		{
			if (!string.IsNullOrEmpty(answers[i]))
			{
				taskFields[i].SetContext(answers[i], true);

				int x = int.Parse(answers[i]);

				if (x > biggestValue)
				{
					biggestValue = x;
					correctAnswerIndex = i;
				}
			}
		}

		PlayQuestionAudio();
	}

	private void ResetFieldData()
	{
		for (int i = 0; i < taskFields.Length; i++)
		{
			taskFields[i].SetContext("", false);
		}
	}

	private void PlayQuestionAudio()
	{
		FMODUnity.RuntimeManager.PlayOneShot($"event:/VO/VO Bigest Number");
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

	private void OnButtonClick(int i)
	{
		if (string.IsNullOrEmpty(taskFields[i].TmpText.text) || taskCompleted)
			return;

		if (correctAnswerIndex == i)
			OnCorrectAnswer();

		StartCoroutine(NextQuestion());
	}

	private IEnumerator NextQuestion()
	{
		if (questionIndex >= questions.Count - 1)
		{
			taskCompleted = true;
			yield return StartCoroutine(StartAndAwaitAudioClipFinish("VO/VO Excellent"));
			TaskHandler.Instance.CompleteTask();
			yield break;
		}

		PopulateButtonsData(questions[questionIndex + 1]);
		questionIndex++;
	}

	private void OnCorrectAnswer()
	{
		print("yay");
		StartCoroutine(TaskHandler.Instance.SummonStar());
	}

	private void OnDisable()
	{
		replayButton.onClick.RemoveAllListeners();

		for (int i = 0; i < taskFields.Length; i++)
		{
			int j = i;
			taskFields[i].RemoveListener(() => OnButtonClick(j));
		}
	}
}
