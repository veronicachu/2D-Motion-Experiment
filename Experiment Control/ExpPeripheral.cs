using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpPeripheral : MonoBehaviour {

    private ExpSetup m_ExpSetup;
    private ExpCue m_ExpCue;

    private GameObject rightFlicker;
    private GameObject leftFlicker;
    private GameObject photocell;
    
    void Start () {
        // Script references
        m_ExpSetup = this.GetComponent<ExpSetup>();
        m_ExpCue = this.GetComponent<ExpCue>();

        // Find flicker objects
        rightFlicker = GameObject.Find("RightMotion_Flicker");
        leftFlicker = GameObject.Find("LeftMotion_Flicker");
        photocell = GameObject.Find("Photocell");
    }

    public string SetupPeripheral()
    {
        // get target direction for current trial
        bool targDirection = m_ExpCue.activeTarget.GetComponent<TargetMotion>().moveRight;

        // get flicker frequencies
        float rightFreq = rightFlicker.GetComponent<FlickerMaterial>().Frequency;
        float leftFreq = leftFlicker.GetComponent<FlickerMaterial>().Frequency;

        // access peripheral lists
        List<bool> right12 = m_ExpSetup.right12peripheralTrials;
        List<bool> right18 = m_ExpSetup.right18peripheralTrials;
        List<bool> left12 = m_ExpSetup.left12peripheralTrials;
        List<bool> left18 = m_ExpSetup.left18peripheralTrials;

        string peripheralSetting = "Null";
        // if rightward motion
        if (targDirection)
        {
            // if rightward motion set to 12.5Hz
            if (rightFreq == m_ExpSetup.frequencies[0])
            {
                // select one of the peripheral flicker trials at random
                int n = Random.Range(0, right12.Count);

                if (right12[n] == true)
                    peripheralSetting = "Right";
                else if (right12[n] == false)
                    peripheralSetting = "Left";
                    
                // remove used trial from list
                right12.RemoveAt(n);
            }
            // if rightward motion set to 18.75Hz
            else if (rightFreq == m_ExpSetup.frequencies[1])
            {
                // select one of the peripheral flicker trials at random
                int n = Random.Range(0, right18.Count);

                if (right18[n] == true)
                    peripheralSetting = "Right";
                else if (right18[n] == false)
                    peripheralSetting = "Left";

                // remove used trial from list
                right18.RemoveAt(n);
            }
        }
        // if leftward motion
        else if (!targDirection)
        {
            // if leftward motion set to 12.5Hz
            if (leftFreq == m_ExpSetup.frequencies[0])
            {
                // select one of the peripheral flicker trials at random
                int n = Random.Range(0, left12.Count);

                if (left12[n] == true)
                    peripheralSetting = "Right";
                else if (left12[n] == false)
                    peripheralSetting = "Left";

                // remove used trial from list
                left12.RemoveAt(n);
            }
            // if leftward motion set to 18.75Hz
            else if (leftFreq == m_ExpSetup.frequencies[1])
            {
                // select one of the peripheral flicker trials at random
                int n = Random.Range(0, left18.Count);

                if (left18[n] == true)
                    peripheralSetting = "Right";
                else if (left18[n] == false)
                    peripheralSetting = "Left";

                // remove used trial from list
                left18.RemoveAt(n);
            }
        }

        if(peripheralSetting == "Right")
            photocell.GetComponent<FlickerMaterial>().Frequency = rightFreq;    // photocell same freq as right motion
        else if(peripheralSetting == "Left")
            photocell.GetComponent<FlickerMaterial>().Frequency = leftFreq;    // photocell same freq as left motion

        return peripheralSetting;
    }

}
