using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExperimentController : MonoBehaviour
{
    #region Setup variables
    // Changeable variables
    public float cueTime;
    public float fixationTime;
    public float trialTime;
    public float targetTime;
    public float targetBuffer;
    public float timeBetweenPeripheralSpawn;
    public bool adjustCoherence;
    public int nextSceneNum;

    // Unchangeable variables
    private float generalTimer;
    private float targetBufferTimer;
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
    bool e_targetCall = false;
    bool e_trialCall = false;
    bool e_respCall = false;
    bool e_feedbackCall = false;
    bool e_flickStartCall = false;

    // Experiment script references
    private ExpSetup m_ExpSetup;
    private ExpCue m_ExpCue;
    private ExpPeripheral m_ExpPeripheral;
    private ExpTrial m_ExpTrial;
    private TrialInfoData m_TrialInfoData;
    private SpawnPeripheral m_SpawnPeriperal;
    private FlickerManager m_FlickerManager;
    private TrialNumber m_TrialNumber;
    private AdjustCoherence m_AdjustCoherence;

    // Gameobject references
    private List<GameObject> occluderRef = new List<GameObject>();
    private GameObject peripheralDisplayRef;
    private GameObject fixationObjectRef;
    private GameObject trialCounterRef;
    private GameObject inputGameobjectRef;
    //private GameObject feedbackGameobjectRef;
    private GameObject instructionsRef;
    private GameObject coherenceManagerRef;

    // UI references 
    public InputField inputRef;
    public Text feedbackRef;

    // Temporary data storage list
    private string peripheralDirection;
    public int totalNum;
    private int targetCount;
    private List<float> targetApperanceTime = new List<float>();
    private int resp;
    private int numCorrect = 0;
    #endregion

    void Start()
    {
        // Setup script references
        m_ExpSetup = this.GetComponent<ExpSetup>();
        m_ExpCue = this.GetComponent<ExpCue>();
        m_ExpPeripheral = this.GetComponent<ExpPeripheral>();
        m_ExpTrial = this.GetComponent<ExpTrial>();

        m_TrialInfoData = this.GetComponent<TrialInfoData>();
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

        //// Setup feedback references
        //feedbackGameobjectRef = GameObject.Find("Feedback");        
        //feedbackGameobjectRef.SetActive(false);                     // hide feedback object

        // Setup instruction references
        instructionsRef = GameObject.Find("Instructions");

        // Setup coherence manager references
        coherenceManagerRef = GameObject.Find("CoherenceManager");
        if (coherenceManagerRef != null)
            m_AdjustCoherence = coherenceManagerRef.GetComponent<AdjustCoherence>();

        // Adjust coherence level for current subject using calibration data
        if (adjustCoherence)
            totalNum = m_AdjustCoherence.coherenceNum;
        else if (!adjustCoherence && coherenceManagerRef != null)   // **hard coded to grab last 10 coherence numbers**
        {
            int lastIndx = m_AdjustCoherence.numRecord.Count - 1;
            int avgNum = 0;
            for (int i = lastIndx; i > lastIndx - 10; i--)
            {
                avgNum = m_AdjustCoherence.numRecord[i] + avgNum;
            }
            avgNum = avgNum / 10;

            totalNum = avgNum;
        }
        else
        {
            totalNum = 50;
        }
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
            m_ExpCue.ShowCue();                             // spawn cue for trial
            fixationObjectRef.SetActive(false);             // hide fixation cross

            // activate occluders as peripheral sets up
            for (int i = 0; i < occluderRef.Count; i++)
            {
                occluderRef[i].SetActive(true);
            }

            m_FlickerManager.SetFlickerFreq();                          // set peripheral frequencies for trial
            peripheralDirection = m_ExpPeripheral.SetupPeripheral();    // set peripheral apperance as right or left
            m_FlickerManager.StartAllFlicker();                         // start flickering elements

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
        targetBufferTimer += Time.deltaTime;
        targetShowTimer += Time.deltaTime;

        // Reset LSL markers
        trialStartMarker = 0;
        trialEndMarker = 0;
        targetStartMarker = 0;
        targetEndMarker = 0;

        // Start the central task
        if (!e_trialCall)
        {
            // deactivate occluders as peripheral sets up
            for (int i = 0; i < occluderRef.Count; i++)
            {
                occluderRef[i].SetActive(false);
            }


            m_ExpTrial.SpawnShapes(totalNum);                                           // spawn center shapes

            trialStartMarker = 1;                                               // set LSL trialStartMarker as 1
            e_trialCall = true;
        }

        DestroyExtra("DistractorClone", totalNum);                   // constantly check the number of items are constant

        // If target hidden and buffer of 500 ms passses
        if (!e_targetCall && targetBufferTimer > targetBuffer * Time.timeScale)
        {
            int n = Random.Range(0, 25);                                        // continue randomly selecting numbers until target shown

            // If target to appear
            if (n == 0)
            {
                targetApperanceTime.Add(generalTimer);
                bool finishedShowMethod = m_ExpTrial.ShowTarget();              // destroy and replace distractors with targets

                // Make sure ShowTarget method finished running
                if (finishedShowMethod)
                {
                    targetCount++;                                              // increase target count

                    targetStartMarker = 1;                                      // set LSL targetStartMarker as 1
                    targetShowTimer = 0;                                        // reset targetShowTimer to 0
                    e_targetCall = true;                                        // signal target not hidden (target shown)
                }
            }
        }

        // If target time has reached max time
        if (e_targetCall && targetShowTimer > targetTime)
        {
            bool finishedHideMethod = m_ExpTrial.HideTarget();

            // Make sure HideTarget method finished running
            if (finishedHideMethod)
            {
                targetEndMarker = 1;                                            // set LSL targetEndMarker as 1
                targetBufferTimer = 0;                                          // reset targetBufferTimer to 0
                e_targetCall = false;                                           // signal target hidden
            }
        }

        // End trial when trial time reaches max
        if (e_trialCall && generalTimer > trialTime)
        {
            m_FlickerManager.StopAllFlicker();                                  // stop flickering elements

            trialEndMarker = 1;                                                 // set LSL trialEndMarker to 1
            generalTimer = 0;                                                   // reset the general timer
            searchDone = true;
            Debug.Log("searchphase done");
        }
    }

    void ResponsePhase()
    {
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
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (inputRef.text == "")
            {
                Debug.Log("input is empty");
            }
            else if (inputRef.text != "")
            {
                resp = int.Parse(inputRef.text);                            // convert string input to int

                // store the trial's info into the trialinfo csv
                bool writeDataDone = m_TrialInfoData.WriteData(peripheralDirection, totalNum,
                    targetCount, resp, targetApperanceTime);

                // Make sure data written before moving on
                if (writeDataDone)
                {
                    // add number of correct trials
                    numCorrect = numCorrect + m_TrialInfoData.CorrectResponseCalc(targetCount, resp);
                    Debug.Log("Num Trials Correct = " + numCorrect);

                    inputRef.DeactivateInputField();                            // deactivate text input field
                    inputGameobjectRef.SetActive(false);                        // hide text input field

                    if (adjustCoherence)
                    {
                        totalNum = m_AdjustCoherence.AdjustTargetNum(targetCount, resp);
                        //feedbackGameobjectRef.SetActive(true);                      // activate feedback text
                        //feedbackRef.text = targetCount.ToString();                  // show targetcount as string input
                    }

                    trialNumber++;                                                              // increment trial number by one

                    if (trialNumber != 64 || trialNumber != 128 || trialNumber != 192)       // **hard coded numbers**
                    {
                        trialCounterRef.SetActive(true);                                        // show trial counter
                        m_TrialNumber.UpdateTrialNumber(trialNumber, m_ExpSetup.totalTrials);   // update trial number
                    }

                    e_feedbackCall = true;
                }
            }
        }

        // Wait to move on from feedback
        if (e_feedbackCall && Input.GetKeyDown(KeyCode.Space))
        {
            //if (adjustCoherence)
            //feedbackGameobjectRef.SetActive(false);

            trialCounterRef.SetActive(false);               // hide trial counter

            responseDone = true;
        }
    }

    void TrialUpdate()
    {
        // Breaks between trial blocks
        if (trialNumber == 64 || trialNumber == 128 || trialNumber == 192)               // **hard coded numbers**
            recordingSetupDone = false;

        // Close the experiment out on the last trial
        if (trialNumber == m_ExpSetup.totalTrials)
        {
            // Save the written data to their respective csv files
            m_TrialInfoData.SaveData();

            // Calculate correct percentage in log output
            m_TrialInfoData.WriteSavePercent(numCorrect, m_ExpSetup.totalTrials);

            // Continue to next scene
            SceneManager.LoadScene(nextSceneNum);
        }

        // Close the experiment out when Esc key hit
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Save the written data to their respective csv files
            m_TrialInfoData.SaveData();

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
        e_targetCall = false;
        e_trialCall = false;
        e_respCall = false;
        e_feedbackCall = false;
        e_flickStartCall = false;

        // Reset changing variables to 0 for the next trial
        generalTimer = 0;
        targetBufferTimer = 0;
        targetCount = 0;

        // Clear lists
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
            timeBetweenPeripheralSpawn = Random.Range(0.05f, 0.1f); // **hard coded time range for now**
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
    #endregion
}


