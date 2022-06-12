using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TaskButton : MonoBehaviour
{
    //possible better with guid
	[SerializeField] private  string fMODEvent;

	private Button button;
	private List<UnityAction> callbacks = new List<UnityAction>();
	public TMP_Text TmpText => tmpText;

	[SerializeField] private Sprite filled;
    [SerializeField] private Sprite nonFilled;

    [SerializeField] private Image imageField;
	[SerializeField] private TMP_Text tmpText;
    private FMOD.Studio.EventInstance fModInstance;
    private bool hasContext = false;
    private int context;

    private void OnEnable()
    {
	    button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        fModInstance = FMODUnity.RuntimeManager.CreateInstance($"event:/{fMODEvent}");
    }

    public void OnClick()
    {
	    if (!hasContext)
		    return;

	    fModInstance.start();

	    foreach (var unityAction in callbacks)
	    {
		    unityAction.Invoke();
	    }
    }

    public void AddListener(UnityAction action)
    {
	    callbacks.Add(action);
    }

    public void SetContext(string ctx, bool isFilled)
    {
	    tmpText.text = ctx;

	    if (!string.IsNullOrEmpty(ctx))
		    int.TryParse(ctx, out context);

	    imageField.sprite = isFilled ? nonFilled : filled;

	    hasContext = isFilled;
    }

    public void RemoveListener(UnityAction action)
    {
	    callbacks.Remove(action);
	    button.onClick.RemoveListener(action);
    }

    private void OnDisable()
    {
	    fModInstance.release();
    }
}
