using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField nameInput;

    public void OnStartButtonPressed()
    {
        // use this to log Student Results on test completion
        string studentInfo = nameInput.text + "_" + DateTime.Now;

        //debug print
        print(studentInfo);

        // Test Enviorment should be in build index 1
        SceneManager.LoadScene(1);
    }
}
