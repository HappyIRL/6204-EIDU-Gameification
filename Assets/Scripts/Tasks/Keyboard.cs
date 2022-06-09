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

	private void Awake()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			int j = i;
			buttons[i].onClick.AddListener(() => OnButtonClick(j));
		}
	}

	public void SetUser(IKeyboard user)
	{
		this.user = user;
	}

	private void OnButtonClick(int index)
	{
		if (user == null)
			return;
		if(index == 10)
		{
			user.UndoNumber();
			return;
		}

		user.SelectNumber(index);
	}
}