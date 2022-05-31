using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Task1 : MonoBehaviour
{
	[SerializeField] private TaskButton[] taskFields = new TaskButton[9];
	[SerializeField] private List<Task1Question> questions = new List<Task1Question>();
	[SerializeField] private Button replayButton;

	private int questionIndex;
    private FMOD.Studio.EventInstance fModInstance;
	private TMP_Text correctAnswerTMP;
	private string correctAnswer;

	private void Start()
	{
		PopulateFieldData(questions[0]);

		replayButton.onClick.AddListener(PlayNumberAudio);

		for (int i = 0; i < taskFields.Length; i++)
		{
			int j = i;
			taskFields[i].AddListener(() => OnButtonClick(j));
		}
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

	private void OnButtonClick(int index)
	{
		if (string.IsNullOrEmpty(taskFields[index].TmpText.text))
			return;

		if (taskFields[index].TmpText == correctAnswerTMP)
			OnCorrectAnswer();

		NextQuestion();
	}

	private void OnCorrectAnswer()
	{
		print("yay");
	}

	private void PopulateFieldData(Task1Question data)
	{
		List<string> answers = data.Options;

		TaskButton[] shuffledTexts = Utils.NewShuffled(taskFields);

		for (int i = 0; i < answers.Count; i++)
		{
			if (!string.IsNullOrEmpty(answers[i]))
			{
				int x = int.Parse(answers[i]);

				shuffledTexts[i].SetContext(answers[i]);

				if (answers[i] == data.CorrectAnswer)
				{
					correctAnswer = data.CorrectAnswer;
					correctAnswerTMP = shuffledTexts[i].TmpText;
				}

			}
			else
			{
				Debug.LogError($"Options for answers can't be empty. At: {data}");
			}
		}

		PlayNumberAudio();
	}

	private IEnumerator AudioNumberQueue()
	{
		PLAYBACK_STATE state = PLAYBACK_STATE.PLAYING;

		while (state != PLAYBACK_STATE.STOPPED)
		{
			fModInstance.getPlaybackState(out state);
			yield return null;
		}

		fModInstance.release();
		FMODUnity.RuntimeManager.PlayOneShot($"event:/VO/VO {correctAnswer}");
	}

	private void PlayNumberAudio()
	{
		fModInstance = FMODUnity.RuntimeManager.CreateInstance($"event:/VO/VO Number Prompt");
		fModInstance.start();
		StartCoroutine(AudioNumberQueue());

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
