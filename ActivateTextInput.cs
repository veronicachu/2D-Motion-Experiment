using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ActivateTextInput : MonoBehaviour {

    public InputField mainInputField;
    public Text text;

    // Activate the main input field when the scene starts.
    void Start()
    {
        mainInputField.ActivateInputField();
    }

    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(Convert.ToInt32(text.text));
        }
    }
}
