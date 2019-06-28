/// <summary>
/// Code for writing and saving CSVs containing trial data information
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class TrialInfoDataConj : MonoBehaviour {

    public GameObject rightFlickerObject;   // input the original (not clone) rightward moving peripheral flicker gameobject
    public GameObject leftFlickerObject;    // input the original (not clone) leftward moving peripheral flicker gameobject

    // Variables for CSV writing
    private string expPath;
    StringBuilder csv = new StringBuilder();
    StringBuilder csv2 = new StringBuilder();
    
    // Script references
    private ExpCueConj expCueRef;
    private FlickerMaterial rightFlickerRef;
    private FlickerMaterial leftFlickerRef;

    void Start()
    {
        // Initialize variables
        expCueRef = GameObject.Find("ExperimentManager").GetComponent<ExpCueConj>();    // access the ExpCueConj.m script on ExperimentManager gameobject
        rightFlickerRef = rightFlickerObject.GetComponent<FlickerMaterial>();           // access the FlickerMaterial.m script on rightward gameobject
        leftFlickerRef = leftFlickerObject.GetComponent<FlickerMaterial>();             // access the FlickerMaterial.m script on leftward gameobject

        // Create filepath/filename for CSV file
        expPath = FileName();
        //Debug.Log(expPath);

        // Write first line of CSV with variable labels corresponding to the following lines
        string newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
            "Target Direction", "Target Shape", "Right Freq", "Left Freq", "Peripheral Direction", 
            "Total Central Dots","Number Targets", "Response", "Target Times");
        csv.AppendLine(newLine);
    }

    // Method for creating a string that results in a filepath/filename
    public static string FileName()
    {
        // If ExpData folder does not exist in the directory, then create it
        Directory.CreateDirectory(Application.dataPath + "/ExpData");

        // Creates the filepath/filename for storage into the ExpData folder with date and time info in filename 
        return string.Format("{0}/ExpData/TrialInfo_{1}.csv",
                             Application.dataPath,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    // Method for writing data into the CSV file
    // Called in the ExperimentController.m after each trial
    public bool WriteData(string peripheralDirection, int nCoherent, int nTargets, int response, List<float> targetTime)
    {
        // INPUT: peripheralDirection = direction of peripheral object
        //        nCoherent = number of times coherent movement was present in the trial
        //        nTargets = number of target objects when there was coherent movement
        //        response = response input of how many times user saw coherent movement
        //        targetTime = times that the coherent movement started

        // OUTPUT: finishedRunning = bool that will be true at the end of the method

        bool finisedRunning = false;
        string targName = expCueRef.activeTarget.name;  // get target gameobject's name to determine movemnet direction and shape

        // Collect target direction
        char targDirection = targName[0];               // get direction - first letter of prefab name should be "L" or "R"

        // Collect target shape
        int a = targName.Length - 4;                    // get shape - the last four characters in the name (Cube/Star)
        string targShape = targName.Substring(a);       // get the substring of the last four characters

        // Collect color frequencies in the trial
        string rightFreq = rightFlickerRef.Frequency.ToString();    // get frequency from variable in FlickerMaterial script
        string leftFreq = leftFlickerRef.Frequency.ToString();      // get frequency from variable in FlickerMaterial script

        // Writes a line with target and frequency information
        string newLine = string.Format("{0},{1},{2},{3},{4},{5},{6}",
            targDirection, targShape, rightFreq, leftFreq, peripheralDirection, nCoherent, nTargets, response);
        for (int i = 0; i < targetTime.Count; i++)
        {
            newLine = newLine + "," + targetTime[i].ToString();
        }
        csv.AppendLine(newLine);

        // Return finishedRunning flagged as true
        finisedRunning = true;
        return finisedRunning;
    }

    // Method for saving the data written in the CSV file
    // Called in the ExperimentController.m after the last experiment
    public void SaveData()
    {
        File.WriteAllText(expPath, csv.ToString());
    }

    // Method for determining whether the user's response of percieved number of targets
    // is correct or incorrect with the actual number of targets
    public int CorrectResponseCalc(int nTargets, int response)
    {
        // Input: nTargets = actual number of targets
        //        response = user's response of percieved number of targets

        // Output: correct = 0 is incorrect, 1 is correct

        int correct = -99;
        if (nTargets == response)
            correct = 1;
        else if (nTargets != response)
            correct = 0;

        return correct;
    }

    // Method for a quick output of the accuracy of the user
    public void WriteSavePercent(int totalCorrect, int totalTrials)
    {
        // INPUT: totalCorrect = total number of trials user was correct
        //        totalTrials = total number of trials

        // Write first line of CSV with the variable names
        csv2.AppendLine("TotalCorrect,TotalTrials");

        // Write second line of CSV with the number of correct and the number of trials
        csv2.AppendLine(totalCorrect.ToString() + "," + totalTrials.ToString());

        // Save the CSV file
        File.WriteAllText("Correct_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv",
            csv2.ToString());
    }

}
