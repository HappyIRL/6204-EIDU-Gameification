using UnityEngine;
using UnityEngine.UI;

public interface IKeyboard
{
	public void SelectNumber(int n);
	public void UndoNumber();
}

public class Keyboard : MonoBehaviour
{
	[SerializeField] private Button[] buttons = new Button[10];

	private IKeyboard user;
	private FMOD.Studio.EventInstance fModInstance;

	private void OnEnable()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			int j = i;
			buttons[i].onClick.AddListener(() => OnButtonClick(j));
		}

		fModInstance = FMODUnity.RuntimeManager.CreateInstance($"event:/SFX/ButtonMAS");
	}

	public void SetUser(IKeyboard user)
	{
		this.user = user;
	}

	private void OnButtonClick(int index)
	{
		if (user == null)
			return;

		fModInstance.start();

		if (index == 10)
		{
			user.UndoNumber();
			return;
		}

		user.SelectNumber(index);
	}

	private void OnDisable()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			int j = i;
			buttons[i].onClick.RemoveAllListeners();;
		}

		fModInstance.release();
	}
}