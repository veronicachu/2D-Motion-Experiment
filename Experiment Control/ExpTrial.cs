using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpTrial : MonoBehaviour
{
    // boundary of spawn area (2D)
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;

    public int targetNum;                   // number of target items
    public float rampStep;                  // ramp up/down time spacing

    public GameObject rightMotion;          // right motion object
    public GameObject leftMotion;           // left motion object
    public GameObject distractorMotion;     // distractor motion object

    // script references
    private ExpCue m_ExpCue;

    void Start() {
        m_ExpCue = this.GetComponent<ExpCue>();
    }

    public List<float> TargetTimes(int targetnum)
    {
        List<float> targTime = new List<float>();

        if (targetnum == 1)
        {
            float temp1 = Random.Range(1.000f, 9.000f);
            targTime.Add(temp1);
        }

        if (targetnum == 2)
        {
            float temp1 = Random.Range(1.000f, 4.500f);
            float temp2 = Random.Range(5.500f, 9.500f);
            targTime.Add(temp1);
            targTime.Add(temp2);
        }

        if (targetnum == 3)
        {
            float temp1 = Random.Range(1.000f, 3.000f);
            float temp2 = Random.Range(4.000f, 6.000f);
            float temp3 = Random.Range(7.000f, 9.000f);
            targTime.Add(temp1);
            targTime.Add(temp2);
            targTime.Add(temp3);
        }

        return targTime;
    }

    public void SpawnShapes(int totalNum)
    {
        // instantiate distractor object (random motion)
        for (int i = 0; i < totalNum; i++)
        {
            SpawnObject(distractorMotion);
        }
    }

    public void SpawnObject(GameObject objecttospawn)
    {
        // instantiate object
        GameObject newObject = Instantiate(objecttospawn);

        // if a distractor type object, set random direction
        string objectName = objecttospawn.name;
        if (objectName.Substring(0, 10) == "Distractor")
        {
            newObject.GetComponent<DistractorMotion>().enabled = true;      // make sure motion script enabled
            newObject.GetComponent<DistractorMotion>().SetDirection();      // set random motion direction
            newObject.tag = "DistractorClone";                              // tag for easy deletion later
        }
        else
        {
            newObject.GetComponent<TargetMotion>().enabled = true;          // make sure motion script enabled
            newObject.tag = "TargetClone";                                  // tag for easy deletion later
        }

        // set start location at random location within range
        float randX = Random.Range(xMin + 0.2f, xMax - 0.2f);
        float randY = Random.Range(yMin + 0.2f, yMax - 0.2f);
        newObject.transform.position = new Vector3(randX, randY, 6.8f);

        // make sure destroy script enabled
        newObject.GetComponent<DestroyReplace>().enabled = true;
    }

    public bool ShowTarget()
    {
        bool finishedRunning = false;

        //// get current trial's target
        //bool rightDirection = m_ExpCue.activeTarget.GetComponent<TargetMotion>().moveRight;

        //// collect all distractor objects
        //List<GameObject> distractors = new List<GameObject>();
        //distractors.AddRange(GameObject.FindGameObjectsWithTag("DistractorClone"));

        //// delete and replace subset of distractors with target object
        //for (int i = 0; i < targetNum; i++)
        //{
        //    // destroy random distractor object and remove from list
        //    int n = Random.Range(0, distractors.Count);
        //    Destroy(distractors[n]);
        //    distractors.RemoveAt(n);

        //    // if target rightward motion
        //    if (rightDirection)
        //    {
        //        SpawnObject(rightMotion);
        //    }
        //    // if target leftward motion
        //    else if (!rightDirection)
        //    {
        //        SpawnObject(leftMotion);
        //    }
        //}
        StartCoroutine(WaittoSpawn());

        finishedRunning = true;
        return finishedRunning;
    }

    private IEnumerator WaittoSpawn()
    {
        WaitForSeconds wait = new WaitForSeconds(rampStep);

        // get current trial's target
        bool rightDirection = m_ExpCue.activeTarget.GetComponent<TargetMotion>().moveRight;
        
        // delete and replace subset of distractors with target object
        bool done = false;
        for (int i = 0; i < targetNum; i++)
        {
            if (!done)
            {
                // collect all distractor objects
                List<GameObject> distractors = new List<GameObject>();
                distractors.AddRange(GameObject.FindGameObjectsWithTag("DistractorClone"));

                // destroy random distractor object and remove from list
                int n = Random.Range(0, distractors.Count);
                Destroy(distractors[n]);
                distractors.RemoveAt(n);

                // if target rightward motion
                if (rightDirection)
                {
                    SpawnObject(rightMotion);
                }
                // if target leftward motion
                else if (!rightDirection)
                {
                    SpawnObject(leftMotion);
                }

                done = true;
            }
            
            if (done)
            {
                yield return wait;
                done = false;
            }
        }
    }

    public bool HideTarget()
    {
        bool finishedRunning = false;

        //// collect all target objects
        //List<GameObject> targets = new List<GameObject>();
        //targets.AddRange(GameObject.FindGameObjectsWithTag("TargetClone"));

        //// delete and replace targets with distractors
        //for (int i = 0; i < targetNum; i++)
        //{
        //    // destroy target object
        //    Destroy(targets[i]);

        //    // instantiate distractor object
        //    SpawnObject(distractorMotion);
        //}
        StartCoroutine(WaittoDespawn());

        finishedRunning = true;
        return finishedRunning;
    }

    private IEnumerator WaittoDespawn()
    {
        WaitForSeconds wait = new WaitForSeconds(rampStep);

        // collect all target objects
        List<GameObject> targets = new List<GameObject>();
        targets.AddRange(GameObject.FindGameObjectsWithTag("TargetClone"));

        // delete and replace targets with distractors
        bool done = false;
        for (int i = 0; i < targetNum; i++)
        {
            if (!done)
            {
                // destroy target object
                Destroy(targets[i]);

                // instantiate distractor object
                SpawnObject(distractorMotion);

                done = true;
            }
            
            if (done)
            {
                yield return wait;
                done = false;
            }
        }
    }
}
