using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskField : MonoBehaviour
{
	public Button Button { get; set; }
	public TMP_Text TmpText => tmpText;

    [SerializeField] private Sprite filled;
    [SerializeField] private Sprite nonFilled;

    private Image imageField;
    private TMP_Text tmpText;

    private void Awake()
    {
	    imageField = GetComponent<Image>();
	    Button = GetComponent<Button>();
	    tmpText = GetComponentInChildren<TMP_Text>();
	    imageField.sprite = filled;

    }

    public void HasContext()
    {
	    imageField.sprite = nonFilled;
    }
}
