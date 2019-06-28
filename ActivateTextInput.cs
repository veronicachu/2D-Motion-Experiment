/// <summary>
/// This code activates an inputfield UI gameobject and allows user 
/// to input a number to navigate to the next scene when the 'Enter'
/// key is hit
/// 
/// Use for an intro scene when the next possible scene could vary 
/// between users/each run of the program
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ActivateTextInput : MonoBehaviour {

    public InputField mainInputField;   // input the inputfield gameobject
    public Text text;                   // input the UI text gameobject
    
    void Start()
    {
        // Activate the main input field when the scene starts
        mainInputField.ActivateInputField();
    }

    private void FixedUpdate()
    {
        // When 'Enter' key hit, use the number inputted to navigate to the specified scene number
        if(Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(Convert.ToInt32(text.text));
        }
    }
}
