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
	private List<string> taskData = new List<string>();
	private string currentQuestionContent;

	private Coroutine repeatCoroutine;
	private int repeatCount;
	private int wrongAnswerCount;

	private void OnEnable()
	{
		taskCompleted = false;

		StartCoroutine(NextQuestion());

		StartCoroutine(PlayFullNumberAudio());

		replayButton.onClick.AddListener(OnClick_RepeatAudio);

		for (int i = 0; i < taskFields.Length; i++)
		{
			int j = i;
			taskFields[i].AddListener(() => OnButtonClick(j));
		}
	}

	private IEnumerator NextQuestion()
	{
		if (repeatCoroutine != null)
			StopCoroutine(repeatCoroutine);

		repeatCount = 0;

		if (questionIndex >= questions.Count - 1)
		{
			taskCompleted = true;
			yield return StartCoroutine(StartAndAwaitAudioClipFinish("VO/VO Completed Task"));
			TaskHandler.Instance.CompleteTask(taskData);
			yield break;
		}

		PopulateFieldData(questions[questionIndex]);
		questionIndex++;

		repeatCoroutine = StartCoroutine(RepeatInterval());
	}

	private IEnumerator RepeatInterval()
	{
		if (repeatCount > 2)
		{
			FailQuestion();
			yield break;
		}

		yield return new WaitForSeconds(10f);
		OnClick_RepeatAudio();
		yield return new WaitForSeconds(10f);
		OnClick_RepeatAudio();
		yield return new WaitForSeconds(10f);

		FailQuestion();
	}

	private void FailQuestion()
	{
		taskData.Add($"M.1.1.{questionIndex}, {currentQuestionContent}, {correctAnswer}, false, N/G");
		fModInstance.stop(STOP_MODE.IMMEDIATE);
		fModInstance.release();
		wrongAnswerCount++;
		CheckForAnswers();
		StartCoroutine(NextQuestion());
	}


	private void ResetPopulationData()
	{
		currentQuestionContent = "";

		for (int i = 0; i < taskFields.Length; i++)
		{
			taskFields[i].SetContext("", false);
		}
	}

	private void OnButtonClick(int index)
	{
		TMP_Text tmp = taskFields[index].TmpText;
		string tmpText = tmp.text;

		if (string.IsNullOrEmpty(tmpText) || taskCompleted)
			return;

		if (repeatCoroutine != null)
			StopCoroutine(repeatCoroutine);

		bool isCorrectAnswer = false;

		if (tmp == correctAnswerTMP)
		{
			OnCorrectAnswer();
			isCorrectAnswer = true;
		}
		else
		{
			wrongAnswerCount++;
		}

		fModInstance.stop(STOP_MODE.IMMEDIATE);
		fModInstance.release();

		taskData.Add($"M.1.1.{questionIndex}, {currentQuestionContent}, {correctAnswer}, {isCorrectAnswer}, {tmpText}");

		CheckForAnswers();

		StartCoroutine(NextQuestion());
	}

	private void CheckForAnswers()
	{
		if (wrongAnswerCount > 1 && questionIndex != questions.Count - 1)
		{
			questionIndex += 1;

			while (questionIndex < questions.Count - 1)
			{
				taskData.Add($"M.1.1.{questionIndex}, , , ,SKIPPED");
				questionIndex++;
			}
		}
	}

	private void OnCorrectAnswer()
	{
		StartCoroutine(TaskHandler.Instance.SummonStar());
	}

	private void PopulateFieldData(Task1Question data)
	{
		ResetPopulationData();

		List<string> answers = data.Options;

		TaskButton[] shuffledTexts = Utils.NewShuffled(taskFields);

		for (int i = 0; i < answers.Count; i++)
		{
			if (!string.IsNullOrEmpty(answers[i]))
			{
				currentQuestionContent += answers[i] + ";";

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

		currentQuestionContent = currentQuestionContent.Remove(currentQuestionContent.Length - 1, 1);

		if (questionIndex > 0)
			StartCoroutine(PlayNumberAudio());
	}

	private IEnumerator PlayNumberAudio()
	{
		yield return new WaitForSeconds(0.5f);
		FMODUnity.RuntimeManager.PlayOneShot($"event:/VO/VO {correctAnswer}");
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
		if(repeatCoroutine != null)
			StopCoroutine(repeatCoroutine);

		if (repeatCount > 2)
		{
			FailQuestion();
			return;
		}

		StartCoroutine(PlayFullNumberAudio());
		repeatCount++;

		repeatCoroutine = StartCoroutine(RepeatInterval());
	}

	private IEnumerator PlayFullNumberAudio()
	{
		yield return StartCoroutine(StartAndAwaitAudioClipFinish("VO/VO Number Prompt"));

		if (fModInstance.isValid())
		{
			FMODUnity.RuntimeManager.PlayOneShot($"event:/VO/VO {correctAnswer}");
			fModInstance.release();
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();

		replayButton.onClick.RemoveAllListeners();

		for (int i = 0; i < taskFields.Length; i++)
		{
			int j = i;
			taskFields[i].RemoveListener(() => OnButtonClick(j));
		}
	}
}
