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
	private List<string> taskData = new List<string>();
	private string currentQuestionContent;
	private string correctAnswer;

	private Coroutine repeatCoroutine;
	private int repeatCount;
	private int wrongAnswerCount;

	private void OnEnable()
	{
		StartCoroutine(NextQuestion());

		replayButton.onClick.AddListener(RepeatQuestionAudio);

		FMODUnity.RuntimeManager.PlayOneShot($"event:/VO/VO Bigest Number");

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
				currentQuestionContent += answers[i] + ";";

				taskFields[i].SetContext(answers[i], true);

				if (int.TryParse(answers[i], out int result))
				{
					if (result > biggestValue)
					{
						biggestValue = result;
						correctAnswerIndex = i;
						correctAnswer = answers[i];
					}
				}
			}
		}

		currentQuestionContent = currentQuestionContent.Remove(currentQuestionContent.Length - 1, 1);
	}

	private void ResetFieldData()
	{
		currentQuestionContent = "";

		for (int i = 0; i < taskFields.Length; i++)
		{
			taskFields[i].SetContext("", false);
		}
	}

	private void RepeatQuestionAudio()
	{
		if(repeatCoroutine != null)
			StopCoroutine(repeatCoroutine);

		if (repeatCount > 2)
		{
			FailQuestion();
			return;
		}

		FMODUnity.RuntimeManager.PlayOneShot($"event:/VO/VO Bigest Number");
		repeatCount++;

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

	private void OnButtonClick(int i)
	{
		string tmpText = taskFields[i].TmpText.text;

		if (string.IsNullOrEmpty(tmpText) || taskCompleted)
			return;

		if (repeatCoroutine != null)
			StopCoroutine(repeatCoroutine);

		bool isCorrectAnswer = false;

		if (correctAnswerIndex == i)
		{
			OnCorrectAnswer();
			isCorrectAnswer = true;
		}
		else
		{
			wrongAnswerCount++;
		}

		taskData.Add($"M.1.2.{questionIndex}, {currentQuestionContent}, {correctAnswer}, {isCorrectAnswer}, {tmpText}");

		CheckForAnswers();

		StartCoroutine(NextQuestion());
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

	private void CheckForAnswers()
	{
		if (wrongAnswerCount > 1 && questionIndex != questions.Count - 1)
		{
			questionIndex += 1;

			while (questionIndex < questions.Count - 1)
			{
				taskData.Add($"M.1.2.{questionIndex}, , , ,SKIPPED");
				questionIndex++;
			}
		}
	}

	private void FailQuestion()
	{
		taskData.Add($"M.1.2.{questionIndex}, {currentQuestionContent}, {correctAnswer}, false, N/G");
		wrongAnswerCount++;
		CheckForAnswers();
		StartCoroutine(NextQuestion());
	}

	private IEnumerator NextQuestion()
	{
		repeatCount = 0;

		if (repeatCoroutine != null)
			StopCoroutine(repeatCoroutine);

		if (questionIndex >= questions.Count - 1)
		{
			taskCompleted = true;
			yield return StartCoroutine(StartAndAwaitAudioClipFinish("VO/VO Excellent"));
			TaskHandler.Instance.CompleteTask(taskData);
			yield break;
		}

		PopulateButtonsData(questions[questionIndex + 1]);
		questionIndex++;

		repeatCoroutine = StartCoroutine(RepeatInterval());
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

		for (int i = 0; i < taskFields.Length; i++)
		{
			int j = i;
			taskFields[i].RemoveListener(() => OnButtonClick(j));
		}
	}
}
