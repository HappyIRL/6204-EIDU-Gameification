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

    private Image imageField;
    private TMP_Text tmpText;
    private FMOD.Studio.EventInstance fModInstance;
    private bool hasContext = false;
    private int context;

    private void Start()
    {
	    fModInstance = FMODUnity.RuntimeManager.CreateInstance($"event:/{fMODEvent}");
    }

    private void Awake()
    {
	    imageField = GetComponent<Image>();
	    button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
	    tmpText = GetComponentInChildren<TMP_Text>();
	    imageField.sprite = filled;
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

    public void SetContext(string ctx)
    {
	    tmpText.text = ctx;

	    context = int.Parse(ctx);

        imageField.sprite = nonFilled;
	    hasContext = true;
    }

    public void RemoveListener(UnityAction action)
    {
	    callbacks.Remove(action);
	    button.onClick.RemoveListener(action);
    }

    private void OnDestroy()
    {
	    fModInstance.release();
    }
}
