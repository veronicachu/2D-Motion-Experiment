using System.Collections.Generic;
using UnityEngine;

public class ExpPeripheralConj : MonoBehaviour
{

    private ExpSetup m_ExpSetup;
    private ExpCueConj m_ExpCueConj;

    public GameObject rightFlicker;
    public GameObject leftFlicker;
    private GameObject photocell;

    void Start()
    {
        // Script references
        m_ExpSetup = this.GetComponent<ExpSetup>();
        m_ExpCueConj = this.GetComponent<ExpCueConj>();

        photocell = GameObject.Find("Photocell");
    }

    public string SetupPeripheral()
    {
        // get target direction for current trial
        bool targDirection = m_ExpCueConj.activeTarget.GetComponent<TargetMotion>().moveRight;

        // get flicker frequencies
        float rightFreq = rightFlicker.GetComponent<FlickerMaterial>().Frequency;
        float leftFreq = leftFlicker.GetComponent<FlickerMaterial>().Frequency;

        // access peripheral lists
        List<bool> rightperipheral = m_ExpSetup.rightPeripheralTrials;
        List<bool> leftperipheral = m_ExpSetup.leftPeripheralTrials;

        string peripheralSetting = "Null";
        // if rightward motion
        if (targDirection)
        {
            // select one of the peripheral flicker trials at random
            int n = Random.Range(0, rightperipheral.Count);

            if (rightperipheral[n] == true)
                peripheralSetting = "Right";
            else if (rightperipheral[n] == false)
                peripheralSetting = "Left";

            // remove used trial from list
            rightperipheral.RemoveAt(n);
        }
        // if leftward motion
        else if (!targDirection)
        {
            // select one of the peripheral flicker trials at random
            int n = Random.Range(0, leftperipheral.Count);

            if (leftperipheral[n] == true)
                peripheralSetting = "Right";
            else if (leftperipheral[n] == false)
                peripheralSetting = "Left";

            // remove used trial from list
            leftperipheral.RemoveAt(n);
        }
        
        return peripheralSetting;
    }

}
