using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class TrialInfoDataConj : MonoBehaviour {

    StringBuilder csv = new StringBuilder();
    StringBuilder csv2 = new StringBuilder();
    private string expPath;

    // Script references
    private ExpCueConj expCueRef;
    public GameObject rightFlickerObject;
    public GameObject leftFlickerObject;

    // Variable references
    private FlickerMaterial rightFlickerRef;
    private FlickerMaterial leftFlickerRef;

    void Start()
    {
        // Initialize variables
        expCueRef = GameObject.Find("ExperimentManager").GetComponent<ExpCueConj>();
        rightFlickerRef = rightFlickerObject.GetComponent<FlickerMaterial>();
        leftFlickerRef = leftFlickerObject.GetComponent<FlickerMaterial>();

        // Start new stream writer
        expPath = FileName();
        //Debug.Log(expPath);

        // Write first line with data information
        string newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
            "Target Direction", "Target Shape", "Right Freq", "Left Freq", "Peripheral Direction", 
            "Total Central Dots","Number Targets", "Response", "Target Times");
        csv.AppendLine(newLine);
    }

    public static string FileName()
    {
        Directory.CreateDirectory(Application.dataPath + "/ExpData");

        // Creates the file path to store into the Data folder
        return string.Format("{0}/ExpData/TrialInfo_{1}.csv",
                             Application.dataPath,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public bool WriteData(string peripheralDirection, int nCoherent, int nTargets, int response, List<float> targetTime)
    {
        bool finisedRunning = false;
        string targName = expCueRef.activeTarget.name;

        // Collect target direction
        char targDirection = targName[0];     // first letter of prefab name should be "L" or "R"

        // Collect target shape
        int a = targName.Length - 4;
        string targShape = targName.Substring(a);

        // Collect color frequencies in the trial
        string rightFreq = rightFlickerRef.Frequency.ToString();
        string leftFreq = leftFlickerRef.Frequency.ToString();

        // Writes a line with target and frequency information
        string newLine = string.Format("{0},{1},{2},{3},{4},{5},{6}",
            targDirection, targShape, rightFreq, leftFreq, peripheralDirection, nCoherent, nTargets, response);
        for (int i = 0; i < targetTime.Count; i++)
        {
            newLine = newLine + "," + targetTime[i].ToString();
        }

        csv.AppendLine(newLine);
        finisedRunning = true;
        return finisedRunning;
    }

    public void SaveData()
    {
        File.WriteAllText(expPath, csv.ToString());
    }

    public int CorrectResponseCalc(int nTargets, int response)
    {
        int correct = -99;
        if (nTargets == response)
            correct = 1;
        else if (nTargets != response)
            correct = 0;

        return correct;
    }

    public void WriteSavePercent(int totalCorrect, int totalTrials)
    {
        csv2.AppendLine("TotalCorrect,TotalTrials");
        csv2.AppendLine(totalCorrect.ToString() + "," + totalTrials.ToString());

        File.WriteAllText("Correct_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv",
            csv2.ToString());
    }

}
