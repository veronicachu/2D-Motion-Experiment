/// <summary>
/// This code inputs the current trial number in a UI text object
/// Place on a gameobject in the scene and drag the gameobject 
/// containing the target UI object into the textObject field
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrialNumber : MonoBehaviour {

    public GameObject textObject;   // input gameobject that contains target UI text object
    private Text UItext;

    void Start()
    {
        UItext = textObject.GetComponent<Text>();   // access text component in the UI text object
    }

    // Method that updates the trial number
    // Call in an Update() method in main ExperimentController script
    public void UpdateTrialNumber (int trialnum, int totalnum)
    {
        // Produce the string in the format of "[trial num] of [total trials]"
        UItext.text = "Trial " + trialnum.ToString() + " of " + totalnum.ToString();
	}
}
