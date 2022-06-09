using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

public class Task0 : MonoBehaviour
{
	[SerializeField] private Button startButton;
	[SerializeField] private Button replayButton;

	private FMOD.Studio.EventInstance fModInstance;

	private void Awake()
	{
		startButton.onClick.AddListener(OnStartClick);
		replayButton.onClick.AddListener(PlayTaskAudio);
	}

	private void Start()
    {
	    fModInstance = FMODUnity.RuntimeManager.CreateInstance("event:/VO/VO Start Prompt");
	    PlayTaskAudio();
    }

	private void PlayTaskAudio()
	{
		fModInstance.getPlaybackState(out PLAYBACK_STATE state);

		if (state == PLAYBACK_STATE.PLAYING)
			return;

		fModInstance.start();
	}

	private void OnStartClick()
	{
		FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/ButtonSparkleMAS");
		fModInstance.stop(STOP_MODE.ALLOWFADEOUT);
		fModInstance.release();

		TaskHandler.Instance.CompleteTask();
	}
}
