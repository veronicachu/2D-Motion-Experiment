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
    private TrialNumber m_TrialNumber;

    // Gameobject references
    public GameObject trialCounterRef;
    public GameObject inputGameobjectRef;
    public InputField inputRef;
    private List<GameObject> occluderRef = new List<GameObject>();
    private GameObject peripheralDisplayRef;
    private GameObject fixationObjectRef;
    private GameObject rightFlicker;
    private GameObject leftFlicker;

    // Temporary data storage list
    private string peripheralDirection;
    private int targetCount;
    private List<float> targetApperanceTime = new List<float>();
    private int resp;
    
	void Start () {
        // Setup script references
        m_ExpSetup = this.GetComponent<ExpSetup>();
        m_ExpCue = this.GetComponent<ExpCue>();
        m_ExpTrial = this.GetComponent<ExpTrial>();

        m_TrialInfoData = this.GetComponent<TrialInfoData>();
        m_FlickerManager = this.GetComponent<FlickerManager>();

        // Setup peripheral display references
        occluderRef.AddRange(GameObject.FindGameObjectsWithTag("Occluder"));
        peripheralDisplayRef = GameObject.Find("Peripheral Display");
        m_SpawnPeriperal = peripheralDisplayRef.GetComponent<SpawnPeripheral>();
        
        // Setup miscellaneous gameobject references
        fixationObjectRef = GameObject.Find("FixationCross");
        m_TrialNumber = trialCounterRef.GetComponent<TrialNumber>();
        trialCounterRef.SetActive(false);                           // hide trial counter
        inputGameobjectRef.SetActive(false);                        // hide text input field

        // Find flicker objects
        rightFlicker = GameObject.Find("RightMotion_Flicker");
        leftFlicker = GameObject.Find("LeftMotion_Flicker");
    }
	
	void Update () {
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

    void CuePhase() {
        // Display the trial's cue
        if (!e_cueCall)
        {
            m_ExpCue.ShowCue();                             // spawn cue for trial
            fixationObjectRef.SetActive(false);             // hide fixation cross

            // activate occluders as peripheral sets up
            for (int i = 0; i < occluderRef.Count; i++)
            {
                occluderRef[i].SetActive(true);             
            }

            m_FlickerManager.SetFlickerFreq();              // set peripheral frequencies for trial
            m_FlickerManager.StartAllFlicker();             // start flickering elements

            peripheralDirection = SetupPeripheral();        // set peripheral apperance as right or left

            trialCounterRef.SetActive(true);                // show trial counter
            m_TrialNumber.UpdateTrialNumber(trialNumber);   // update trial number

            e_cueCall = true;
        }
        
        // Keep track of time for when minimum cue phase reached
        generalTimer += Time.deltaTime;
        if (generalTimer >= cueTime && Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            trialCounterRef.SetActive(false);               // hide trial counter

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
            // deactivate occluders as peripheral sets up
            for (int i = 0; i < occluderRef.Count; i++)
            {
                occluderRef[i].SetActive(false);
            }

            m_ExpTrial.SpawnShapes();                                           // spawn center shapes

            e_trialCall = true;
        }

        DestroyExtra("DistractorClone", m_ExpTrial.totalNum);                   // constantly check the number of items are constant

        // If target hidden and buffer of 500 ms passses
        if (!e_targetCall && targetBufferTimer > 1.5f)
        {
            int n = Random.Range(0, 100);                                        // continue randomly selecting numbers until target shown

            // If target to appear
            if (n == 0)
            {
                targetApperanceTime.Add(generalTimer);
                bool finishedShowMethod = m_ExpTrial.ShowTarget();              // destroy and replace distractors with targets

                // Make sure ShowTarget method finished running
                if(finishedShowMethod)
                {
                    targetCount++;                                              // increase target count

                    targetShowTimer = 0;                                        // reset targetShowTimer to 0
                    e_targetCall = true;                                        // signal target not hidden (target shown)
                    Debug.Log("targets shown");
                }
            }
        }

        // If target time has reached max time
        if (e_targetCall && targetShowTimer > targetTime)
        {
            bool finishedHideMethod = m_ExpTrial.HideTarget();

            // Make sure HideTarget method finished running
            if(finishedHideMethod)
            {
                targetBufferTimer = 0;                                          // reset targetBufferTimer to 0
                e_targetCall = false;                                           // signal target hidden
            }
        }

        // End trial when trial time reaches max
        if (e_trialCall && generalTimer > trialTime)
        {
            m_FlickerManager.StopAllFlicker();                                  // stop flickering elements

            generalTimer = 0;                                                   // reset the general timer
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
                resp = int.Parse(inputRef.text);                                        // convert string input to int

                // store the trial's info into the trialinfo csv
                m_TrialInfoData.WriteData(peripheralDirection, 
                    targetCount, resp, targetApperanceTime);                            

                inputRef.DeactivateInputField();                                        // deactivate text input field
                inputGameobjectRef.SetActive(false);                                    // hide text input field
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

        // Clear lists
        targetApperanceTime.Clear();
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    // Appendix Methods
    //////////////////////////////////////////////////////////////////////////////////////////////////
    void StartPeripheral(string direction)
    {
        peripheralTimer += Time.deltaTime;

        if (peripheralTimer > timeBetweenPeripheralSpawn)
        {
            if (direction == "Right")
                m_SpawnPeriperal.SpawnFromRightColumns();
            if (direction == "Left")
                m_SpawnPeriperal.SpawnFromLeftColumns();
            if (direction == "Null")
                m_SpawnPeriperal.SpawnFromBothColumns();
            peripheralTimer = 0;
        }
    }

    string SetupPeripheral()
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
            if (rightFreq == 12.5f)
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
            else if (rightFreq == 18.75f)
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
            if (leftFreq == 12.5f)
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
            else if (leftFreq == 18.75f)
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

        return peripheralSetting;
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
        Debug.Log(listClones.Count);

        if (listClones.Count > maxNum)
            Destroy(listClones[0]);
    }
}
