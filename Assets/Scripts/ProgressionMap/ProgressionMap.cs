using System;
using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;
using UnityEngine.Rendering;

public class ProgressionMap : MonoBehaviour
{
	[SerializeField] private List<PathCreator> paths = new List<PathCreator>();
	[SerializeField] private GameObject progressionIcon;
	[SerializeField] private GameObject background;

	private int segmentIndex = 0;

	public void NextStep(Action callback)
	{
		background.SetActive(true);
		progressionIcon.SetActive(true);

		StartCoroutine(MoveByDistanceOnPath(paths[segmentIndex],1f, callback));
		segmentIndex++;
	}

	private IEnumerator MoveByDistanceOnPath(PathCreator pathCreator, float totalMoveTime, Action callback)
	{
		float time = 0;

		while (time < totalMoveTime)
		{
			time += Time.deltaTime;

			float percentileOfTotalTime = time / totalMoveTime;

			Vector3 nextPoint = pathCreator.path.GetPointAtTime(percentileOfTotalTime, EndOfPathInstruction.Stop);

			progressionIcon.transform.position = nextPoint;

			yield return null;
		}

		yield return new WaitForSeconds(1f);
		background.SetActive(false);
		progressionIcon.SetActive(false);
		callback?.Invoke();
	}
}
