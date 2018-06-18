using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpTrial : MonoBehaviour
{
    // boundary of spawn area (2D)
    //[HideInInspector]
    public float xMin;
    //[HideInInspector]
    public float xMax;
    //[HideInInspector]
    public float yMin;
    //[HideInInspector]
    public float yMax;

    public int targetNum;                   // number of target items
    public int totalNum;                    // number of distractor items

    public GameObject rightMotion;          // right motion object
    public GameObject leftMotion;           // left motion object
    public GameObject distractorMotion;     // distractor motion object

    // script references
    private ExpCue m_ExpCue;

    void Start()
    {
        m_ExpCue = this.GetComponent<ExpCue>();
    }

    public void SpawnShapes()
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

        // get current trial's target
        bool rightDirection = m_ExpCue.activeTarget.GetComponent<TargetMotion>().moveRight;

        // collect all distractor objects
        List<GameObject> distractors = new List<GameObject>();
        distractors.AddRange(GameObject.FindGameObjectsWithTag("DistractorClone"));

        // delete and replace subset of distractors with target object
        for (int i = 0; i < targetNum; i++)
        {
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
        }

        finishedRunning = true;
        return finishedRunning;
    }

    public bool HideTarget()
    {
        bool finishedRunning = false;

        // collect all target objects
        List<GameObject> targets = new List<GameObject>();
        targets.AddRange(GameObject.FindGameObjectsWithTag("TargetClone"));

        // delete and replace targets with distractors
        for (int i = 0; i < targetNum; i++)
        {
            // destroy target object
            Destroy(targets[i]);

            // instantiate distractor object
            SpawnObject(distractorMotion);
        }

        finishedRunning = true;
        return finishedRunning;
    }
}
