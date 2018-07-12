using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustCoherence : MonoBehaviour {

    public int coherenceNum;
    public int step;
    [HideInInspector] public List<int> numRecord = new List<int>();
    private List<int> correctRespList = new List<int>();
    
    private GameObject experimentManagerRef;
    private ExperimentController m_ExperimentController;
    private ExpTrial m_ExpTrial;

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start () {
        experimentManagerRef = GameObject.Find("ExperimentManager");
        m_ExpTrial = experimentManagerRef.GetComponent<ExpTrial>();

        coherenceNum = m_ExpTrial.targetNum * 2;     // start at 50% coherent motion
	}
	
	public int AdjustTargetNum(int nTargets, int response)
    {
        // Determine if response correct/incorrect
        int correct = -99;
        if (nTargets == response)
            correct = 1;
        else if (nTargets != response)
            correct = 0;

        // Record as correct/incorrect response
        correctRespList.Add(correct);
        int lastIndx = correctRespList.Count - 1;

        // Change step size as get lower
        if (coherenceNum > m_ExpTrial.targetNum * 3)    // 30%
            step = 2;
        if (coherenceNum > m_ExpTrial.targetNum * 5)    // 20%
            step = 1;
        
        // If response incorrect, decrease num target dots
        if (correctRespList[lastIndx] == 0)
        {
            coherenceNum = coherenceNum - step;
            Debug.Log("incorrect step up");
        }

        // If response correct, check previous response
        else if (correctRespList[lastIndx] == 1)
        {
            Debug.Log("correct wait");
            // If previous response correct too, increase num target dots
            if (correctRespList.Count > 1)
            {
                if (correctRespList[lastIndx - 1] == 1)
                coherenceNum = coherenceNum + step;
                Debug.Log("correct step down");
            }
        }

        numRecord.Add(coherenceNum);
        
        return coherenceNum;
    }
}
