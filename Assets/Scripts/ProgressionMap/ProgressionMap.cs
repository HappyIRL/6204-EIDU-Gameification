using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using PathCreation;
using UnityEngine;
using UnityEngine.Rendering;

public class ProgressionMap : MonoBehaviour
{
	//[SerializeField] private List<PathCreator> paths = new List<PathCreator>();
	[SerializeField] private GameObject progressionIcon;
	[SerializeField] private GameObject background;
	[SerializeField] private List<RectTransform> positions = new List<RectTransform>();

	private FMOD.Studio.EventInstance fModInstance;

	private int segmentIndex;


	public IEnumerator NextStep(Action callback)
	{
		background.SetActive(true);
		progressionIcon.SetActive(true);

		if (segmentIndex == 0)
			yield return StartCoroutine(StartAndAwaitAudioClipFinish("VO/VO Intro"));

		StartCoroutine(MoveByDistanceOnPath( callback));
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

	private IEnumerator MoveByDistanceOnPath(Action callback)
	{
		float time = 0;

		fModInstance = FMODUnity.RuntimeManager.CreateInstance($"event:/SFX/BusMAS");
		fModInstance.start();
		yield return new WaitForSeconds(5f);

		//while (time < totalMoveTime)
		//{
		//	time += Time.deltaTime;

		//	float percentileOfTotalTime = time / totalMoveTime;

		//	Vector3 nextPoint = pathCreator.path.GetPointAtTime(percentileOfTotalTime, EndOfPathInstruction.Stop);

		//	progressionIcon.transform.position = nextPoint;

		//	yield return null;
		//}

		RectTransform rt = progressionIcon.GetComponent<RectTransform>();
		rt.position = positions[segmentIndex].position;

		fModInstance.stop(STOP_MODE.ALLOWFADEOUT);
		fModInstance.release();

		yield return new WaitForSeconds(1f);
		background.SetActive(false);
		progressionIcon.SetActive(false);
		callback?.Invoke();
		segmentIndex++;
	}
}
