using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExperimentControllerConj : MonoBehaviour {

    #region Setup variables
    // Changeable variables
    public float cueTime;
    public float fixationTime;
    public float trialTime;
    public float targetTime;
    public float aftereffectTime;
    public float timeBetweenPeripheralSpawn;
    public bool adjustCoherence;
    public int nextSceneNum;

    // Unchangeable variables
    private float generalTimer;
    public List<float> targetTimes;
    private float targetShowTimer;
    private float peripheralTimer;
    public int trialNumber;

    // LSL marker variables
    [HideInInspector] public int trialStartMarker = 0;
    [HideInInspector] public int trialEndMarker = 0;
    [HideInInspector] public int targetStartMarker = 0;
    [HideInInspector] public int targetEndMarker = 0;

    // Trial progression flag variables
    bool recordingSetupDone = false;
    bool cueDone = false;
    bool fixationDone = false;
    bool searchDone = false;
    bool responseDone = false;

    // Within-trial function flag variables
    bool e_cueCall = false;
    bool e_fixationCall = false;
    bool e_targetCall1 = false;
    bool e_targetCall2 = false;
    bool e_targetCall3 = false;
    bool e_targetHide1 = false;
    bool e_targetHide2 = false;
    bool e_targetHide3 = false;
    bool e_trialCall = false;
    bool e_respCall = false;
    bool e_feedbackCall = false;
    bool e_flickStartCall = false;

    // Experiment script references
    private ExpSetup m_ExpSetup;
    private ExpCueConj m_ExpCueConj;
    private ExpPeripheralConj m_ExpPeripheralConj;
    private ExpTrialConj m_ExpTrialConj;
    private TrialInfoDataConj m_TrialInfoDataConj;
    private SpawnPeripheral m_SpawnPeriperal;
    private FlickerManager m_FlickerManager;
    private TrialNumber m_TrialNumber;
    private AdjustCoherenceConj m_AdjustCoherenceConj;

    // Gameobject references
    private List<GameObject> occluderRef = new List<GameObject>();
    private GameObject peripheralDisplayRef;
    private GameObject fixationObjectRef;
    private GameObject trialCounterRef;
    private GameObject inputGameobjectRef;
    private GameObject feedbackGameobjectRef;
    private GameObject instructionsRef;
    private GameObject coherenceManagerRef;

    // UI references 
    public InputField inputRef;
    public Text feedbackRef;

    // Temporary data storage list
    private bool rightDirection;
    private string peripheralDirection;
    private int totalNumRR;
    private int totalNumLL;
    private int totalNumRL;
    private int totalNumLR;
    private int distNumTemp;
    private int targetNumTemp;
    private int targetCount;
    private List<float> targetApperanceTime = new List<float>();
    private int resp;
    private int numCorrect = 0;
    #endregion

    void Start()
    {
        // Setup script references
        m_ExpSetup = this.GetComponent<ExpSetup>();
        m_ExpCueConj = this.GetComponent<ExpCueConj>();
        m_ExpPeripheralConj = this.GetComponent<ExpPeripheralConj>();
        m_ExpTrialConj = this.GetComponent<ExpTrialConj>();

        m_TrialInfoDataConj = this.GetComponent<TrialInfoDataConj>();
        m_FlickerManager = this.GetComponent<FlickerManager>();

        // Setup peripheral display references
        occluderRef.AddRange(GameObject.FindGameObjectsWithTag("Occluder"));
        peripheralDisplayRef = GameObject.Find("Peripheral Display");
        m_SpawnPeriperal = peripheralDisplayRef.GetComponent<SpawnPeripheral>();

        // Setup fixation gameobject reference
        fixationObjectRef = GameObject.Find("FixationCross");

        // Setup trial counter references
        trialCounterRef = GameObject.Find("TrialCounter");
        m_TrialNumber = trialCounterRef.GetComponent<TrialNumber>();
        trialCounterRef.SetActive(false);                           // hide trial counter

        // Setup input field references
        inputGameobjectRef = GameObject.Find("InputField");
        inputGameobjectRef.SetActive(false);                        // hide text input field

        // Setup feedback references
        feedbackGameobjectRef = GameObject.Find("Feedback");
        feedbackGameobjectRef.SetActive(false);                     // hide feedback object

        // Setup instruction references
        instructionsRef = GameObject.Find("Instructions");

        // Setup coherence manager references
        coherenceManagerRef = GameObject.Find("CoherenceManager");
        if (coherenceManagerRef != null)
            m_AdjustCoherenceConj = coherenceManagerRef.GetComponent<AdjustCoherenceConj>();

    }

    void Update()
    {
        // Allow time to setup recording
        if (!recordingSetupDone)
        {
            fixationObjectRef.SetActive(false);         // hide fixation cross
            instructionsRef.SetActive(true);            // show wait instructions

            if (Input.GetKeyDown(KeyCode.Space))
            {
                instructionsRef.SetActive(false);                           // hide instructions text
                recordingSetupDone = true;
            }
        }

        // Once recording setup, then start experiment
        if (recordingSetupDone)
        {
            if (e_cueCall && !searchDone)
                StartPeripheral(peripheralDirection);                   // keep peripheral dots spawning and moving

            // Calls the different stages of the trial
            if (!cueDone && !fixationDone && !searchDone && !responseDone)
                if (m_ExpSetup.loadComplete == true)
                    CuePhase();
            if (cueDone && !fixationDone && !searchDone && !responseDone)
                FixationPhase();
            if (cueDone && fixationDone && !searchDone && !responseDone)
                SearchPhase();
            if (cueDone && fixationDone && searchDone && !responseDone)
                ResponsePhase();
            if (cueDone && fixationDone && searchDone && responseDone)
                TrialUpdate();
        }
    }

    void CuePhase()
    {
        // Display the trial's cue
        if (!e_cueCall)
        {
            Time.timeScale = 6;                             // x6 the speed for peripheral to get into place
            m_ExpCueConj.ShowCue();                             // spawn cue for trial
            fixationObjectRef.SetActive(false);             // hide fixation cross

            // activate occluders as peripheral sets up
            for (int i = 0; i < occluderRef.Count; i++)
            {
                occluderRef[i].SetActive(true);
            }

            // set peripheral apperance as right or left
            peripheralDirection = m_ExpPeripheralConj.SetupPeripheral();

            // start flickering elements
            m_FlickerManager.StartAllFlicker();

            // get current trial's target
            rightDirection = m_ExpCueConj.activeTarget.GetComponent<TargetMotion>().moveRight;

            // set total num of distractors for trial
            SetTotalNum();

            e_cueCall = true;
        }

        // Keep track of time for when minimum cue phase reached
        generalTimer += Time.deltaTime;
        if (generalTimer >= cueTime)
        {
            Time.timeScale = 1;                             // reset the speed to one

            cueDone = true;
            generalTimer = 0f;
            Debug.Log("cuephase done");
        }
    }

    void FixationPhase()
    {
        if (!e_fixationCall)
        {
            DestroyClones("Clone");                     // destroy cue object
            fixationObjectRef.SetActive(true);          // show fixation cross

            e_fixationCall = true;
        }

        // Keep track of time for when minimum fixation phase reached
        generalTimer += Time.deltaTime;
        if (generalTimer >= fixationTime)
        {
            fixationDone = true;
            generalTimer = 0f;
            Debug.Log("fixationphase done");
        }
    }

    void SearchPhase()
    {
        generalTimer += Time.deltaTime;
        targetShowTimer += Time.deltaTime;

        // Reset LSL markers
        trialStartMarker = 0;
        trialEndMarker = 0;
        targetStartMarker = 0;
        targetEndMarker = 0;

        // regularly check that the number of items are constant
        DestroyExtra("DistractorClone", distNumTemp);
        DestroyExtra("TargetClone", targetNumTemp);

        // Start the central task
        if (!e_trialCall)
        {
            // deactivate occluders as peripheral sets up
            for (int i = 0; i < occluderRef.Count(); i++)
            {
                occluderRef[i].SetActive(false);
            }

            // get number of targets to appear in trial
            int n = Random.Range(0, m_ExpSetup.numTarget.Count);
            int numTarg = m_ExpSetup.numTarget[n];
            m_ExpSetup.numTarget.RemoveAt(n);
            Debug.Log("Number of Targets: " + numTarg);

            // get times for target spawns
            targetTimes = m_ExpTrialConj.TargetTimes(numTarg);
            
            // spawn center shapes
            m_ExpTrialConj.SpawnShapes(distNumTemp);

            // set number of target direction distractors
            targetNumTemp = m_ExpTrialConj.targetNum;

            trialStartMarker = 1;                                               // set LSL trialStartMarker as 1
            e_trialCall = true;
        }

        // If 1 target and proper time, show target 1
        if (!e_targetCall1 && targetTimes.Count > 0 && generalTimer > targetTimes[0])
        {
            // destroy and replace distractors with targets
            targetNumTemp = m_ExpTrialConj.ShowTarget(targetNumTemp);
            Debug.Log("show target #1");

            targetCount++;                                              // increase target count
            targetApperanceTime.Add(generalTimer);                      // add target appear time to record
            targetStartMarker = 1;                                      // set LSL targetStartMarker as 1

            targetShowTimer = 0;                                        // reset targetShowTimer to 0
            e_targetCall1 = true;                                       // signal target shown
        }

        // If 2 targets and proper time, show target 2
        if (!e_targetCall2 && targetTimes.Count > 1 && generalTimer > targetTimes[1])
        {
            // destroy and replace distractors with targets
            targetNumTemp = m_ExpTrialConj.ShowTarget(targetNumTemp);
            Debug.Log("show target #2");

            targetCount++;                                              // increase target count
            targetApperanceTime.Add(generalTimer);                      // add target appear time to record
            targetStartMarker = 1;                                      // set LSL targetStartMarker as 1

            targetShowTimer = 0;                                        // reset targetShowTimer to 0
            e_targetCall2 = true;                                       // signal target shown
        }

        // If 3 targets and proper time, show target 3
        if (!e_targetCall3 && targetTimes.Count > 2 && generalTimer > targetTimes[2])
        {
            // destroy and replace distractors with targets
            targetNumTemp = m_ExpTrialConj.ShowTarget(targetNumTemp);
            Debug.Log("show target #3");

            targetCount++;                                              // increase target count
            targetApperanceTime.Add(generalTimer);                      // add target appear time to record
            targetStartMarker = 1;                                      // set LSL targetStartMarker as 1

            targetShowTimer = 0;                                        // reset targetShowTimer to 0
            e_targetCall3 = true;                                       // signal target shown
        }

        // If target time has reached max time, hide target #1
        if (e_targetCall1 && !e_targetHide1 && targetShowTimer > targetTime && targetCount == 1)
        {
            // destroy and replace targets with distractors
            targetNumTemp = m_ExpTrialConj.HideTarget(targetNumTemp);
            Debug.Log("hide target #1");

            targetEndMarker = 1;                                            // set LSL targetEndMarker as 1
            e_targetHide1 = true;                                           // signal target hidden
        }

        // If target time has reached max time, hide target #2
        if (e_targetCall2 && !e_targetHide2 && targetShowTimer > targetTime && targetCount == 2)
        {
            // destroy and replace targets with distractors
            targetNumTemp = m_ExpTrialConj.HideTarget(targetNumTemp);
            Debug.Log("hide target #2");

            targetEndMarker = 1;                                            // set LSL targetEndMarker as 1
            e_targetHide2 = true;                                           // signal target hidden
        }

        // If target time has reached max time, hide target #3
        if (e_targetCall3 && !e_targetHide3 && targetShowTimer > targetTime && targetCount == 3)
        {
            // destroy and replace targets with distractors
            targetNumTemp = m_ExpTrialConj.HideTarget(targetNumTemp);
            Debug.Log("hide target #3");

            targetEndMarker = 1;                                            // set LSL targetEndMarker as 1
            e_targetHide3 = true;                                           // signal target hidden
        }

        // End trial when trial time reaches max
        if (e_trialCall && generalTimer > trialTime)
        {
            m_FlickerManager.StopAllFlicker();                              // stop flickering elements
            targetNumTemp = m_ExpTrialConj.targetNum;                       // reset num of targets allowed

            trialEndMarker = 1;                                             // set LSL trialEndMarker to 1
            generalTimer = 0;                                               // reset the general timer
            searchDone = true;
            Debug.Log("searchphase done");
        }
    }

    void ResponsePhase()
    {
        generalTimer += Time.deltaTime;

        // make sure no extra objects are floating around
        DestroyExtra("DistractorClone", 0);
        DestroyExtra("TargetClone", 0);

        if (!e_respCall)
        {
            DestroyClones("DistractorClone");           // destroy central objects (distractors)
            DestroyClones("TargetClone");               // destroy central objects (targets)
            DestroyClones("FlickerClone");              // destroy peripheral objects
            fixationObjectRef.SetActive(false);         // hide fixation cross
            inputGameobjectRef.SetActive(true);         // show text input field
            inputRef.ActivateInputField();              // activate text input field
            inputRef.text = "";                         // set text input field to be blank

            e_respCall = true;
            Debug.Log("Waiting for subject response...");
        }

        // Wait for the subject's response
        inputRef.Select();                              // select text input field
        if (!e_feedbackCall && Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            if (inputRef.text == "")
            {
                Debug.Log("input is empty");
            }
            else if (inputRef.text != "")
            {
                resp = int.Parse(inputRef.text);                            // convert string input to int

                // store the trial's info into the trialinfo csv
                bool writeDataDone = m_TrialInfoDataConj.WriteData(peripheralDirection, distNumTemp,
                    targetCount, resp, targetApperanceTime);

                // Make sure data written before moving on
                if (writeDataDone)
                {
                    // add number of correct trials
                    numCorrect = numCorrect + m_TrialInfoDataConj.CorrectResponseCalc(targetCount, resp);
                    Debug.Log("Num Trials Correct = " + numCorrect);

                    inputRef.DeactivateInputField();                            // deactivate text input field
                    inputGameobjectRef.SetActive(false);                        // hide text input field

                    if (adjustCoherence)
                    {
                        m_AdjustCoherenceConj.AdjustTargetNum(targetCount, resp, rightDirection, peripheralDirection);
                        feedbackGameobjectRef.SetActive(true);                      // activate feedback text
                        feedbackRef.text = targetCount.ToString();                  // show targetcount as string input
                    }
                    else
                    {
                        feedbackGameobjectRef.SetActive(true);                      // activate feedback text
                        feedbackRef.text = targetCount.ToString();                  // show targetcount as string input
                    }

                    trialNumber++;                                                          // increment trial number by one
                    trialCounterRef.SetActive(true);                                        // show trial counter
                    m_TrialNumber.UpdateTrialNumber(trialNumber, m_ExpSetup.totalTrials);   // update trial number

                    e_feedbackCall = true;
                }
            }
        }

        // Wait to move on from feedback
        if (e_feedbackCall && Input.GetKeyDown(KeyCode.Space) && generalTimer > aftereffectTime)
        {
            feedbackGameobjectRef.SetActive(false);         // hide feedback
            trialCounterRef.SetActive(false);               // hide trial counter

            generalTimer = 0;
            responseDone = true;
        }
    }

    void TrialUpdate()
    {
        // Close the experiment out on the last trial
        if (trialNumber == m_ExpSetup.totalTrials)
        {
            // Save the written data to their respective csv files
            m_TrialInfoDataConj.SaveData();

            // Calculate correct percentage in log output
            m_TrialInfoDataConj.WriteSavePercent(numCorrect, m_ExpSetup.totalTrials);

            // Continue to next scene
            SceneManager.LoadScene(nextSceneNum);
        }

        // Close the experiment out when Esc key hit
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Save the written data to their respective csv files
            m_TrialInfoDataConj.SaveData();

            // Continue to next scene
            SceneManager.LoadScene(nextSceneNum);
        }

        // Reset all experiment phase progression flags to false for the next trial
        cueDone = false;
        fixationDone = false;
        searchDone = false;
        responseDone = false;

        // Reset all calls to exp and flicker code to false for the next trial
        e_cueCall = false;
        e_fixationCall = false;
        e_targetCall1 = false;
        e_targetCall2 = false;
        e_targetCall3 = false;
        e_targetHide1 = false;
        e_targetHide2 = false;
        e_targetHide3 = false;
        e_trialCall = false;
        e_respCall = false;
        e_feedbackCall = false;
        e_flickStartCall = false;

        // Reset changing variables to 0 for the next trial
        generalTimer = 0;
        targetCount = 0;

        // Clear lists
        targetTimes.Clear();
        targetApperanceTime.Clear();
    }

    #region Appendix Methods
    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Appendix Methods
    //////////////////////////////////////////////////////////////////////////////////////////////////
    void StartPeripheral(string direction)
    {
        peripheralTimer += Time.deltaTime;

        if (peripheralTimer > timeBetweenPeripheralSpawn)
        {
            timeBetweenPeripheralSpawn = Random.Range(0.1f, 0.15f); // **hard coded time range for now**
            if (direction == "Right")
                m_SpawnPeriperal.SpawnRightwardMotion();
            if (direction == "Left")
                m_SpawnPeriperal.SpawnLeftwardMotion();
            if (direction == "Null")
                m_SpawnPeriperal.SpawnFromBothColumns();
            peripheralTimer = 0;
        }
    }

    private void DestroyClones(string taglabel)
    {
        // Find all the cube clones and destroy them
        GameObject[] arrayClones = GameObject.FindGameObjectsWithTag(taglabel);
        for (int i = 0; i < arrayClones.Length; i++)
        {
            Destroy(arrayClones[i]);
        }
    }

    private void DestroyExtra(string taglabel, int maxNum)
    {
        // Find all the same object
        List<GameObject> listClones = new List<GameObject>();
        listClones.AddRange(GameObject.FindGameObjectsWithTag(taglabel));
        //Debug.Log(listClones.Count);

        if (listClones.Count > maxNum)
            Destroy(listClones[0]);
    }

    private void SetTotalNum()
    {
        // Change coherence
        if (adjustCoherence)
        {
            totalNumRR = m_AdjustCoherenceConj.coherenceNumRR;
            totalNumLL = m_AdjustCoherenceConj.coherenceNumLL;
            totalNumRL = m_AdjustCoherenceConj.coherenceNumRL;
            totalNumLR = m_AdjustCoherenceConj.coherenceNumLR;
        }
        else if (!adjustCoherence && coherenceManagerRef != null)   // **hard coded to grab last 10 coherence numbers**
        {
            totalNumRR = m_AdjustCoherenceConj.avgNumRR;
            totalNumLL = m_AdjustCoherenceConj.avgNumLL;
            totalNumRL = m_AdjustCoherenceConj.avgNumRL;
            totalNumLR = m_AdjustCoherenceConj.avgNumLR;
        }
        else
        {
            totalNumRR = 15;
            totalNumLL = 15;
            totalNumRL = 15;
            totalNumLR = 15;
        }

        // Change TotalNum of distractors
        if (rightDirection && peripheralDirection == "Right")
            distNumTemp = totalNumRR;
        else if (!rightDirection && peripheralDirection == "Left")
            distNumTemp = totalNumLL;
        else if (rightDirection && peripheralDirection == "Left")
            distNumTemp = totalNumRL;
        else if (!rightDirection && peripheralDirection == "Right")
            distNumTemp = totalNumLR;
    }
    #endregion
}
