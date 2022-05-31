using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Redcode.Paths;
using UnityEngine;

public class ProgressionMap : MonoBehaviour
{
	[SerializeField] private Path path;
	[SerializeField] private GameObject progressionIcon;
	[SerializeField] private GameObject background;

	[SerializeField] private List<float> steps = new List<float>();

	private float currentDistance = 0f;

	private int stepCounter = 0;

	public void NextStep(Action callback)
	{
		background.SetActive(true);
		progressionIcon.SetActive(true);
		StartCoroutine(MoveByDistanceOnPath(Mathf.Clamp(currentDistance + steps[stepCounter], 0f, 1f),2f, callback));

		stepCounter++;
	}

	private IEnumerator MoveByDistanceOnPath(float distance, float totalMoveTime, Action callback)
	{
		float time = 0;
		PointData totalDistance = path.GetPointAtDistance(distance, true);

		while (time < totalMoveTime)
		{
			time += Time.deltaTime;
			float lerpT = time / totalMoveTime;

			PointData currentPoint = path.GetPointAtDistance(currentDistance, true);

			progressionIcon.transform.position = Vector3.Lerp(currentPoint.Position, totalDistance.Position, lerpT);
			yield return null;
		}

		progressionIcon.transform.position = totalDistance.Position;

		yield return new WaitForSeconds(1f);
		background.SetActive(false);
		progressionIcon.SetActive(false);
		callback?.Invoke();
	}
}
