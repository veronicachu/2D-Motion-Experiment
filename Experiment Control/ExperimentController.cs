using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExperimentController : MonoBehaviour {

    // Changeable variables
    public float cueTime;
    public float fixationTime;
    public float trialTime;
    public float targetTime;
    public float timeBetweenPeripheralSpawn;
    public int nextSceneNum;

    // Unchangeable variables
    private float generalTimer;
    private float targetBufferTimer;
    private float targetShowTimer;
    private float peripheralTimer;
    public int trialNumber;

    // Trial progression flag variables
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
    bool e_flickStartCall = false;

    // Experiment script references
    private ExpSetup m_ExpSetup;
    private ExpCue m_ExpCue;
    private ExpTrial m_ExpTrial;
    private TrialInfoData m_TrialInfoData;
    private SpawnPeripheral m_SpawnPeriperal;
    private FlickerManager m_FlickerManager;

    // Gameobject references
    public GameObject inputGameobjectRef;
    public InputField inputRef;
    private GameObject peripheralOccluderRef;
    private GameObject peripheralDisplayRef;
    private GameObject fixationObjectRef;

    // Temporary data storage list
    private int targetCount;
    private int resp;
    
	void Start () {
        // Setup script references
        m_ExpSetup = this.GetComponent<ExpSetup>();
        m_ExpCue = this.GetComponent<ExpCue>();
        m_ExpTrial = this.GetComponent<ExpTrial>();

        m_TrialInfoData = this.GetComponent<TrialInfoData>();
        m_FlickerManager = this.GetComponent<FlickerManager>();

        // Setup peripheral display references
        peripheralOccluderRef = GameObject.Find("Occluder");
        peripheralDisplayRef = GameObject.Find("Peripheral Display");
        m_SpawnPeriperal = peripheralDisplayRef.GetComponent<SpawnPeripheral>();
        
        // Setup miscellaneous gameobject references
        fixationObjectRef = GameObject.Find("FixationCross");
        inputGameobjectRef.SetActive(false);    // hide text input field
    }
	
	void Update () {
        if (!searchDone)
            StartPeripheral();                  // Keep peripheral dots spawning and moving

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

    void CuePhase() {
        // Display the trial's cue
        if (!e_cueCall)
        {
            m_ExpCue.ShowCue();                     // spawn cue for trial
            fixationObjectRef.SetActive(false);     // hide fixation cross
            peripheralOccluderRef.SetActive(true);  // activate occluder as peripheral sets up

            m_FlickerManager.SetFlickerFreq();      // set peripheral frequencies for trial
            m_FlickerManager.StartAllFlicker();     // start flickering elements

            e_cueCall = true;
        }

        // Keep track of time for when minimum cue phase reached
        generalTimer += Time.deltaTime;
        if (generalTimer >= cueTime && Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            cueDone = true;
            generalTimer = 0f;
            Debug.Log("cuephase done");
        }
    }

    void FixationPhase() {
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

    void SearchPhase() {
        generalTimer += Time.deltaTime;
        targetBufferTimer += Time.deltaTime;
        targetShowTimer += Time.deltaTime;
        
        // Start the central task
        if (!e_trialCall)
        {
            peripheralOccluderRef.SetActive(false);     // hide occluder to do task
            m_ExpTrial.SpawnShapes();                   // spawn center shapes

            e_trialCall = true;
        }

        // If target hidden and buffer of 500 ms passses
        if (!e_targetCall && targetBufferTimer > 1.0f)
        {
            int n = Random.Range(0, 20);                // continue randomly selecting numbers until target shown

            // If target to appear
            if (n == 0)
            {
                bool finishedShowMethod = m_ExpTrial.ShowTarget();                // destroy and replace distractors with targets

                // make sure ShowTarget method finished running
                if(finishedShowMethod)
                {
                    targetCount++;                          // increase target count

                    targetShowTimer = 0;                    // reset targetShowTimer to 0
                    e_targetCall = true;                    // signal target not hidden (target shown)
                    Debug.Log("targets shown");
                }
            }
        }

        // If target time has reached max time
        if (e_targetCall && targetShowTimer > targetTime)
        {
            bool finishedHideMethod = m_ExpTrial.HideTarget();

            // make sure HideTarget method finished running
            if(finishedHideMethod)
            {
                targetBufferTimer = 0;                      // reset targetBufferTimer to 0
                e_targetCall = false;                       // signal target hidden
            }
        }

        // End trial when trial time reaches max
        if (e_trialCall && generalTimer > trialTime)
        {
            m_FlickerManager.StopAllFlicker();          // stop flickering elements

            generalTimer = 0;                           // reset the general timer
            searchDone = true;
            Debug.Log("searchphase done");
        }
    }

    void ResponsePhase() {
        if (!e_respCall)
        {
            DestroyClones("DistractorClone");           // destroy central objects (distractors)
            DestroyClones("TargetClone");               // destroy central objects (targets)
            DestroyClones("FlickerClone");              // destroy peripheral objects
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
                resp = int.Parse(inputRef.text);        // convert string input to int
                m_TrialInfoData.WriteData(targetCount, resp);        // store the trial's target and flicker info into the trialinfo csv

                inputRef.DeactivateInputField();        // deactivate text input field
                inputGameobjectRef.SetActive(false);    // hide text input field
                Debug.Log("input is " + resp);

                responseDone = true;
            }
        }
    }

    void TrialUpdate () {
        trialNumber++;                                  // increment trial number by one

        // Close the experiment out on the last trial
        if (trialNumber == m_ExpSetup.totalTrials)
        {
            // Save the written data to their respective csv files
            m_TrialInfoData.SaveData();

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
        e_flickStartCall = false;

        // Reset changing variables to 0 for the next trial
        generalTimer = 0;
        targetBufferTimer = 0;
        targetCount = 0;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Appendix Methods
    //////////////////////////////////////////////////////////////////////////////////////////////////
    void StartPeripheral()
    {
        peripheralTimer += Time.deltaTime;

        if (peripheralTimer > timeBetweenPeripheralSpawn)
        {
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
}
