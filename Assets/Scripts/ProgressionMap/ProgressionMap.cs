using System;
using System.Collections;
using Redcode.Paths;
using UnityEngine;

public class ProgressionMap : MonoBehaviour
{
	[SerializeField] private Path path;
	[SerializeField] private GameObject progressionIcon;
	[SerializeField] private GameObject background;

	private float currentDistance = 0f;

	private int stepCounter = 0;

	private void Start()
	{
		NextStep(null);
	}

	public void NextStep(Action callback)
	{
		background.SetActive(true);
		progressionIcon.SetActive(true);
		StartCoroutine(MoveByDistanceOnPath(Mathf.Clamp(currentDistance + 0.15f, 0f, 1f), callback));
	}

	private IEnumerator MoveByDistanceOnPath(float distance, Action callback)
	{
		while (currentDistance < distance)
		{
			PointData point = path.GetPointAtDistance(currentDistance, true);
			progressionIcon.transform.position = point.Position;
			currentDistance += 0.0001f;
			yield return null;
		}

		yield return new WaitForSeconds(1f);
		background.SetActive(false);
		progressionIcon.SetActive(false);
		callback?.Invoke();
	}
}
