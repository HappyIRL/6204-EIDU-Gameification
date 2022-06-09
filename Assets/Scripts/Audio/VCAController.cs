using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VCAController : MonoBehaviour
{
	private FMOD.Studio.VCA vcaController;
	[SerializeField] private string vcaName = "";
	[SerializeField] private Slider slider;

	private string SettingsPath = "VCAMasterSettings";

	private void Awake()
	{
		vcaController = FMODUnity.RuntimeManager.GetVCA($"vca:/{vcaName}");

		if (PlayerPrefs.HasKey(SettingsPath))
		{
			slider.value = PlayerPrefs.GetFloat(SettingsPath);
			vcaController.setVolume(slider.value);
		}
	}

	public void SetVolume(float volume)
	{
		vcaController.setVolume(volume);
	}

	private void OnDisable()
	{
		float value = slider.value;
		PlayerPrefs.SetFloat(SettingsPath, value);
	}
}
