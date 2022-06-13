using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Task1 : MonoBehaviour
{
	[SerializeField] private TaskButton[] taskFields = new TaskButton[9];
	[SerializeField, InlineEditor, TableList] private List<Task1Question> questions = new List<Task1Question>();
	[SerializeField] private Button replayButton;

	private FMOD.Studio.EventInstance fModInstance;
	private int questionIndex;
	private TMP_Text correctAnswerTMP;
	private string correctAnswer;
	private bool taskCompleted;
	private bool promptHasPlayed = false;

	private void OnEnable()
	{
		taskCompleted = false;

		PopulateFieldData(questions[0]);

		replayButton.onClick.AddListener(OnClick_RepeatAudio);

		for (int i = 0; i < taskFields.Length; i++)
		{
			int j = i;
			taskFields[i].AddListener(() => OnButtonClick(j));
		}
	}

	private IEnumerator NextQuestion()
	{
		if (questionIndex >= questions.Count - 1)
		{
			taskCompleted = true;
			yield return StartCoroutine(StartAndAwaitAudioClipFinish("VO/VO Completed Task"));
			TaskHandler.Instance.CompleteTask();
			yield break;
		}

		PopulateFieldData(questions[questionIndex + 1]);
		questionIndex++;
	}

	private void ResetFieldData()
	{
		for (int i = 0; i < taskFields.Length; i++)
		{
			taskFields[i].SetContext("", false);
		}
	}

	private void OnButtonClick(int index)
	{
		if (string.IsNullOrEmpty(taskFields[index].TmpText.text) || taskCompleted)
			return;

		if (taskFields[index].TmpText == correctAnswerTMP)
			OnCorrectAnswer();

		fModInstance.stop(STOP_MODE.IMMEDIATE);
		fModInstance.release();

		StartCoroutine(NextQuestion());
	}

	private void OnCorrectAnswer()
	{
		print("yay");
		StartCoroutine(TaskHandler.Instance.SummonStar());
	}

	private void PopulateFieldData(Task1Question data)
	{
		ResetFieldData();

		List<string> answers = data.Options;

		TaskButton[] shuffledTexts = Utils.NewShuffled(taskFields);

		for (int i = 0; i < answers.Count; i++)
		{
			if (!string.IsNullOrEmpty(answers[i]))
			{
				int x = int.Parse(answers[i]);

				shuffledTexts[i].SetContext(answers[i], true);

				if (answers[i] == data.CorrectAnswer)
				{
					correctAnswerTMP = shuffledTexts[i].TmpText;
					correctAnswer = data.CorrectAnswer;
				}
			}
			else
			{
				Debug.LogError($"Options for answers can't be empty. At: {data}");
			}
		}

		StartCoroutine(PlayNumberAudio());
	}

	private IEnumerator StartAndAwaitAudioClipFinish(string audioClip)
	{
		fModInstance = FMODUnity.RuntimeManager.CreateInstance($"event:/{audioClip}");
		fModInstance.start();

		PLAYBACK_STATE state = PLAYBACK_STATE.PLAYING;

		while (state != PLAYBACK_STATE.STOPPED && fModInstance.isValid())
		{
			fModInstance.getPlaybackState(out state);
			yield return null;
		}
	}

	private void OnClick_RepeatAudio()
	{
		StartCoroutine(PlayNumberAudio());
	}

	private IEnumerator PlayNumberAudio()
	{
		// TODO: prompt should only play once!!!!
		yield return StartCoroutine(StartAndAwaitAudioClipFinish("VO/VO Number Prompt"));

		if (fModInstance.isValid())
		{
			FMODUnity.RuntimeManager.PlayOneShot($"event:/VO/VO {correctAnswer}");
			fModInstance.release();
		}
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
