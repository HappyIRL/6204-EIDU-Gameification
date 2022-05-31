using FMOD;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VCAController : MonoBehaviour
{
	private FMOD.Studio.VCA vcaController;
	[SerializeField] private string vcaName = "";

	private void Start()
	{
		vcaController = FMODUnity.RuntimeManager.GetVCA($"vca:/{vcaName}");
	}

	public void SetVolume(float volume)
	{
		vcaController.setVolume(volume);
	}
}
