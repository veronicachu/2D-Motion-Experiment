using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using UnityEngine.SceneManagement;

public class TrialWaypointData : MonoBehaviour {

    StringBuilder csv;
    private string expPath;
    private string newLine;

    // Script References
    private GameObject expManagerRef;
    private ExperimentController m_experimentControllerRef;
    private ExpTrial m_ExpTrial;
    private List<GameObject> waypointObjects = new List<GameObject>();
    
    // Variable References
    private GameObject[] arrayItems;
    private List<string> arrayColor = new List<string>();
    private List<string> arrayShape = new List<string>();
    private List<Vector3> arrayLocs = new List<Vector3>();

    private void Start()
    {
        // Initialize variables
        expManagerRef = GameObject.Find("ExperimentManager");
        m_experimentControllerRef = expManagerRef.GetComponent<ExperimentController>();
        m_ExpTrial = expManagerRef.GetComponent<ExpTrial>();
    }

    public void NewFile(int trialnum)
    {
        // Collect all of the waypoint objects
        waypointObjects.AddRange(GameObject.FindGameObjectsWithTag("NearWaypoints"));
        waypointObjects.AddRange(GameObject.FindGameObjectsWithTag("FarWaypoints"));

        // Start new stream writer
        csv = new StringBuilder();
        expPath = FileName(trialnum + 1);

        // Write first line with data information
        string newLine = string.Format("{0},{1},{2},{3},{4}",
            "Color", "Shape", "X Location", "Y Location", "Z Location");
        csv.AppendLine(newLine);
    }

    public static string FileName(int trial)
    {
        Directory.CreateDirectory(Application.dataPath + "/ExpData/TrialWaypointData");

        // Creates the file path to store into the Data folder
        return string.Format("{0}/ExpData/TrialWaypointData/WaypointData_Trial{1}_{2}.csv",
                             Application.dataPath,
                             trial,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public void WriteData()
    {
        //// Get array item gameobjects and extract location, color, and orientation
        //arrayItems = m_ExpTrial.arrayItems;
        //arrayColor.Clear();
        //arrayShape.Clear();
        //arrayLocs.Clear();
        //for (int i = 0; i < arrayItems.Length; i++)
        //{
        //    arrayColor.Add(arrayItems[i].GetComponent<Renderer>().material.name);
        //    arrayShape.Add(arrayItems[i].name);
        //    arrayLocs.Add(arrayItems[i].transform.position);
        //}

        //// Convert information to string and store into csv
        //for (int i = 0; i < arrayLocs.Count; i++)
        //{
        //    string color = arrayColor[i].Remove(arrayColor[i].Length - 11);     // remove " (Instance)" at end
        //    string orient = arrayShape[i].Remove(arrayShape[i].Length - 9);     // remove "(_Clone)" at end
        //    string itemX = arrayLocs[i].x.ToString("F2");
        //    string itemY = arrayLocs[i].y.ToString("F2");
        //    string itemZ = arrayLocs[i].z.ToString("F2");

        //    // Writes a line for one item with location, color, and orientation data
        //    string newLine = string.Format("{0},{1},{2},{3},{4}",
        //        color, orient, itemX, itemY, itemZ);
        //    csv.AppendLine(newLine);
        //}
        //csv.AppendLine();
    }

    public void SaveData()
    {
        File.WriteAllText(expPath, csv.ToString());
    }
}
