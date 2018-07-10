using UnityEngine;
using System.Collections.Generic;

public class FlickerManager : MonoBehaviour
{
    private ExpSetup m_ExpSetup;
    private ExpCue m_ExpCue;

    [HideInInspector]
    public GameObject rightFlicker;
    [HideInInspector]
    public GameObject leftFlicker;
    private GameObject photocell;
    private List<GameObject> flickerObjects = new List<GameObject>();

    private Vector3 rightFlickerStartPosition;
    private Vector3 leftFlickerStartPosition;

    private void Awake ()
    {
        // Setup script references
        m_ExpSetup = this.GetComponent<ExpSetup>();
        m_ExpCue = this.GetComponent<ExpCue>();

        // Find flicker objects
        rightFlicker = GameObject.Find("RightMotion_Flicker");
        leftFlicker = GameObject.Find("LeftMotion_Flicker");
        photocell = GameObject.Find("Photocell");

        // Make list of both flicker objects
        flickerObjects.Add(rightFlicker);
        flickerObjects.Add(leftFlicker);
        flickerObjects.Add(photocell);

        rightFlickerStartPosition = rightFlicker.transform.position;
        leftFlickerStartPosition = leftFlicker.transform.position;
    }

    public void SetFlickerFreq()
    {
        // reset prefab locations in game world
        rightFlicker.transform.position = rightFlickerStartPosition;
        leftFlicker.transform.position = leftFlickerStartPosition;

        // temp list of possible frequencies
        List<float> tempFreqList = new List<float>(m_ExpSetup.frequencies);

        // access specific frequency lists
        List<float> rightfreqTrialsTemp = m_ExpSetup.rightfreqTrials;
        List<float> leftfreqTrialsTemp = m_ExpSetup.leftfreqTrials;

        // get target direction for current trial
        bool targDirection = m_ExpCue.activeTarget.GetComponent<TargetMotion>().moveRight;

        // if rightward motion
        if(targDirection)
        {
            // select one of the right flicker trials at random
            int n = Random.Range(0, rightfreqTrialsTemp.Count);

            // set right motion flicker frequency
            rightFlicker.GetComponent<FlickerMaterial>().Frequency = rightfreqTrialsTemp[n];
            //photocell.GetComponent<FlickerMaterial>().Frequency = rightfreqTrialsTemp[n];

            // remove used frequency from the list of possible frequencies
            for (int i = 0; i < tempFreqList.Count; i++)
            {
                if (tempFreqList[i] == rightfreqTrialsTemp[n])
                {
                    tempFreqList.RemoveAt(i);
                }
            }

            // set left motion flicker frequency with remaining frequency
            leftFlicker.GetComponent<FlickerMaterial>().Frequency = tempFreqList[0];

            // remove current motion-freq trial from overall list of right motion-freq trials
            rightfreqTrialsTemp.RemoveAt(n);
        }

        // if leftward motion
        else if(!targDirection)
        {
            int n = Random.Range(0, leftfreqTrialsTemp.Count);

            // set left motion flicker frequency
            leftFlicker.GetComponent<FlickerMaterial>().Frequency = leftfreqTrialsTemp[n];
            //photocell.GetComponent<FlickerMaterial>().Frequency = leftfreqTrialsTemp[n];
            
            // remove used frequency from the list of possible frequencies
            for (int i = 0; i < tempFreqList.Count; i++)
            {
                if (tempFreqList[i] == leftfreqTrialsTemp[n])
                {
                    tempFreqList.RemoveAt(i);
                }
            }

            // set right motion flicker frequency with remaining frequency
            rightFlicker.GetComponent<FlickerMaterial>().Frequency = tempFreqList[0];

            // remove current motion-freq trial from overall list of left motion-freq trials
            leftfreqTrialsTemp.RemoveAt(n);
        }
    }

    public void StartAllFlicker()
    {
        for (int i = 0; i < flickerObjects.Count; i++)
        {
            flickerObjects[i].GetComponent<FlickerMaterial>().StartFlicker();
        }
    }

    public void StopAllFlicker()
    {
        for (int i = 0; i < flickerObjects.Count; i++)
        {
            flickerObjects[i].GetComponent<FlickerMaterial>().StopFlicker();
        }
    }

}
