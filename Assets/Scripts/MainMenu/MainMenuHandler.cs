using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Globalization;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField nameInput;

    [SerializeField] private UserData userData;

    private Button button;

    private void Start()
    {
	    button = GetComponentInChildren<Button>();

        button.onClick.AddListener(OnStartButtonPressed);
    }

    public void OnStartButtonPressed()
    {
	    if (string.IsNullOrEmpty(nameInput.text))
		    return;

        // use this to log Student Results on test completion
        string studentInfo = $"{nameInput.text} - {DateTime.Now.ToString("yyyy.MM.dd-HH;mm;ss", CultureInfo.InvariantCulture)}";

       userData.StudentInfo = studentInfo;

        // Test Enviorment should be in build index 1
        SceneManager.LoadScene(1);
    }

    private void OnDisable()
    {
	    button.onClick.RemoveAllListeners();
    }
}
