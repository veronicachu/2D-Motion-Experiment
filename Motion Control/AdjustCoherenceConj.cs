using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustCoherenceConj : MonoBehaviour
{
    public int coherenceNumRR;
    public int coherenceNumLL;
    public int coherenceNumRL;
    public int coherenceNumLR;

    [HideInInspector] public int avgNumRR = 0;
    [HideInInspector] public int avgNumLL = 0;
    [HideInInspector] public int avgNumRL = 0;
    [HideInInspector] public int avgNumLR = 0;

    public int step;
    [HideInInspector] public List<int> numRecordRR = new List<int>();
    [HideInInspector] public List<int> numRecordLL = new List<int>();
    [HideInInspector] public List<int> numRecordRL = new List<int>();
    [HideInInspector] public List<int> numRecordLR = new List<int>();

    private List<int> correctRespListRR = new List<int>();
    private List<int> correctRespListLL = new List<int>();
    private List<int> correctRespListRL = new List<int>();
    private List<int> correctRespListLR = new List<int>();

    private GameObject experimentManagerRef;
    private ExpTrialConj m_ExpTrialConj;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        experimentManagerRef = GameObject.Find("ExperimentManager");
        m_ExpTrialConj = experimentManagerRef.GetComponent<ExpTrialConj>();

        coherenceNumRR = m_ExpTrialConj.targetNum * 2;     // start at 50% coherent motion
        coherenceNumLL = m_ExpTrialConj.targetNum * 2;     // start at 50% coherent motion
        coherenceNumRL = m_ExpTrialConj.targetNum * 2;     // start at 50% coherent motion
        coherenceNumLR = m_ExpTrialConj.targetNum * 2;     // start at 50% coherent motion
    }

    public void AdjustTargetNum(int nTargets, int response, bool rightDirection, string peripheralDirection)
    {
        // Determine if response correct/incorrect
        int correct = -99;
        if (nTargets == response)
            correct = 1;
        else if (nTargets != response)
            correct = 0;

        // Right target + right peripheral
        if (rightDirection && peripheralDirection == "Right")
        {
            correctRespListRR.Add(correct);
            int lastIndxRR = correctRespListRR.Count - 1;

            // Decrease num target dots for incorrect
            if (correctRespListRR[lastIndxRR] == 0)
                coherenceNumRR = coherenceNumRR - step;
            // Increase num target dots for correct twice
            else if (correctRespListRR[lastIndxRR] == 1)
            {
                if (correctRespListRR.Count > 1)
                {
                    if (correctRespListRR[lastIndxRR - 1] == 1)
                    {
                        coherenceNumRR = coherenceNumRR + step;
                        numRecordRR.Add(coherenceNumRR);
                    }
                }
            }
        }
        // Left target + left peripheral
        else if (!rightDirection && peripheralDirection == "Left")
        {
            correctRespListLL.Add(correct);
            int lastIndxLL = correctRespListLL.Count - 1;

            // Decrease num target dots for incorrect
            if (correctRespListLL[lastIndxLL] == 0)
                coherenceNumLL = coherenceNumLL - step;
            // Increase num target dots for correct twice
            else if (correctRespListLL[lastIndxLL] == 1)
            {
                if (correctRespListLL.Count > 1)
                {
                    if (correctRespListLL[lastIndxLL - 1] == 1)
                    {
                        coherenceNumLL = coherenceNumLL + step;
                        numRecordLL.Add(coherenceNumLL);
                    }
                }
            }
        }
        // Right target + left peripheral
        else if (rightDirection && peripheralDirection == "Left")
        {
            correctRespListRL.Add(correct);
            int lastIndxRL = correctRespListRL.Count - 1;

            // Decrease num target dots for incorrect
            if (correctRespListRL[lastIndxRL] == 0)
                coherenceNumRL = coherenceNumRL - step;
            // Increase num target dots for correct twice
            else if (correctRespListRL[lastIndxRL] == 1)
            {
                if (correctRespListRL.Count > 1)
                {
                    if (correctRespListRL[lastIndxRL - 1] == 1)
                    {
                        coherenceNumRL = coherenceNumRL + step;
                        numRecordRL.Add(coherenceNumRL);
                    }
                }
            }
        }
        // Left target + right peripheral
        else if (!rightDirection && peripheralDirection == "Right")
        {
            correctRespListLR.Add(correct);
            int lastIndxLR = correctRespListLR.Count - 1;

            // Decrease num target dots for incorrect
            if (correctRespListLR[lastIndxLR] == 0)
                coherenceNumLR = coherenceNumLR - step;
            // Increase num target dots for correct twice
            else if (correctRespListLR[lastIndxLR] == 1)
            {
                if (correctRespListLR.Count > 1)
                {
                    if (correctRespListLR[lastIndxLR - 1] == 1)
                    {
                        coherenceNumLR = coherenceNumLR + step;
                        numRecordLR.Add(coherenceNumLR);
                    }
                }
            }
        }

        // Change step size as difficulty increases
        //if (coherenceNum > m_ExpTrial.targetNum * 3)    // 30%
        //    step = 2;
        //if (coherenceNum > m_ExpTrial.targetNum * 5)    // 20%
        //    step = 1;
    }

    public void ExpCoherence()
    {
        // Calculate for RR
        int lastIndxRR = numRecordRR.Count - 1;
        for (int i = lastIndxRR; i > lastIndxRR - 10; i--)
        {
            avgNumRR = numRecordRR[i] + avgNumRR;
        }
        avgNumRR = avgNumRR / 10;

        // Calculate for LL
        int lastIndxLL = numRecordLL.Count - 1;
        for (int i = lastIndxLL; i > lastIndxLL - 10; i--)
        {
            avgNumLL = numRecordLL[i] + avgNumLL;
        }
        avgNumLL = avgNumLL / 10;

        // Calculate for RL
        int lastIndxRL = numRecordRL.Count - 1;
        for (int i = lastIndxRL; i > lastIndxRL - 10; i--)
        {
            avgNumRL = numRecordRL[i] + avgNumRL;
        }
        avgNumRL = avgNumRL / 10;

        // Calculate for LR
        int lastIndxLR = numRecordLR.Count - 1;
        for (int i = lastIndxLR; i > lastIndxLR - 10; i--)
        {
            avgNumLR = numRecordLR[i] + avgNumLR;
        }
        avgNumLR = avgNumLR / 10;
    }
}
