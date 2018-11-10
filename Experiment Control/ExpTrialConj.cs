using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpTrialConj : MonoBehaviour
{
    // boundary of spawn area (2D)
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;

    public int targetNum;                                               // number of target direction items
    public List<GameObject> distractorObjects = new List<GameObject>(); // distractor objects (not target direction)
    public List<GameObject> rightObjects = new List<GameObject>();      // list of right motion objects
    public List<GameObject> leftObjects = new List<GameObject>();       // list of left motion objects

    // script references
    private ExpCueConj m_ExpCueConj;

    void Start()
    {
        m_ExpCueConj = this.GetComponent<ExpCueConj>();
    }

    public List<float> TargetTimes(int targetnum)
    {
        List<float> targTime = new List<float>();

        if (targetnum == 1)
        {
            float temp1 = Random.Range(0.500f, 7.000f);
            targTime.Add(temp1);
        }

        if (targetnum == 2)
        {
            float temp1 = Random.Range(0.500f, 2.500f);
            float temp2 = Random.Range(5.000f, 7.000f);
            targTime.Add(temp1);
            targTime.Add(temp2);
        }

        if (targetnum == 3)
        {
            float temp1 = Random.Range(0.500f, 1.000f);
            float temp2 = Random.Range(3.500f, 4.000f);
            float temp3 = Random.Range(6.500f, 7.000f);
            targTime.Add(temp1);
            targTime.Add(temp2);
            targTime.Add(temp3);
        }

        return targTime;
    }

    public void SpawnShapes(int totalNum)
    {
        // instantiate distractor objects (expects only 2 items in list)
        for (int i = 0; i < totalNum / 2; i++)
        {
            SpawnObject(distractorObjects[0]);
            SpawnObject(distractorObjects[1]);
        }

        // get target
        GameObject targetObject = m_ExpCueConj.activeTarget;

        // instantiate object with same target direction (not exact target)
        bool rightDirection = m_ExpCueConj.activeTarget.GetComponent<TargetMotion>().moveRight;
        int distnum;
        if (rightDirection)
        {
            for (int i = 0; i < rightObjects.Count; i++)
            {
                if (rightObjects[i] != targetObject)
                {
                    distnum = i;
                    for (int j = 0; j < targetNum; j++)
                    {
                        SpawnObject(rightObjects[distnum]);
                    }
                }
            }
        }
        else if (!rightDirection)
        {
            for (int i = 0; i < leftObjects.Count; i++)
            {
                if (leftObjects[i] != targetObject)
                {
                    distnum = i;
                    for (int j = 0; j < targetNum; j++)
                    {
                        SpawnObject(leftObjects[distnum]);
                    }
                }
            }
        }
    }

    public void SpawnObject(GameObject objecttospawn)
    {
        // get target
        GameObject targetObject = m_ExpCueConj.activeTarget;

        // instantiate object
        GameObject newObject = Instantiate(objecttospawn);

        // tag targets and distractors
        string objectName = objecttospawn.name;
        if (objectName.Substring(0, 15) == targetObject.name.Substring(0, 15))
        {
            newObject.GetComponent<TargetMotion>().enabled = true;          // make sure motion script enabled
            newObject.tag = "TargetClone";                                  // tag for easy deletion later
        }
        else if (objectName.Substring(0, 10) == "Distractor")
        {
            newObject.GetComponent<DistractorMotionConj>().enabled = true;      // make sure motion script enabled
            newObject.GetComponent<DistractorMotionConj>().SetDirection();      // set random motion direction
            newObject.tag = "DistractorClone";                              // tag for easy deletion later
        }
        else
        {
            newObject.GetComponent<TargetMotion>().enabled = true;
            newObject.tag = "DistractorClone";                              // tag for easy deletion later
        }

        // set start location at random location within range
        float randX = Random.Range(xMin + 0.2f, xMax - 0.2f);
        float randY = Random.Range(yMin + 0.2f, yMax - 0.2f);
        newObject.transform.position = new Vector3(randX, randY, 6.8f);

        // make sure destroy script enabled
        newObject.GetComponent<DestroyReplaceConj>().enabled = true;
    }

    public int ShowTarget(int totalNum)
    {
        // get target
        GameObject targetObject = m_ExpCueConj.activeTarget;

        // instantiate target
        bool rightDirection = m_ExpCueConj.activeTarget.GetComponent<TargetMotion>().moveRight;
        int distnum;

        if (rightDirection)
        {
            for (int i = 0; i < rightObjects.Count; i++)
            {
                if (rightObjects[i] == targetObject)
                {
                    distnum = i;
                    SpawnObject(rightObjects[distnum]);
                }
            }
        }
        else if (!rightDirection)
        {
            for (int i = 0; i < leftObjects.Count; i++)
            {
                if (leftObjects[i] == targetObject)
                {
                    distnum = i;
                    SpawnObject(leftObjects[distnum]);
                }
            }
        }

        // collect all distractor objects
        List<GameObject> distractors = new List<GameObject>();
        distractors.AddRange(GameObject.FindGameObjectsWithTag("DistractorClone"));

        // destroy random distractor object and remove from list
        int n = Random.Range(0, distractors.Count);
        Destroy(distractors[n]);
        distractors.RemoveAt(n);

        // reduce total number of target direction distractors to account for added target
        totalNum = totalNum - 1;
        return totalNum;
    }

    public int HideTarget(int totalNum)
    {
        // find target object
        List<GameObject> targets = new List<GameObject>();
        targets.AddRange(GameObject.FindGameObjectsWithTag("TargetClone"));

        // destroy target object
        for (int i = 0; i < targets.Count; i++)
        {
            Destroy(targets[i]);
        }

        // get target
        GameObject targetObject = m_ExpCueConj.activeTarget;

        // instantiate object with same target direction (not exact target)
        bool rightDirection = m_ExpCueConj.activeTarget.GetComponent<TargetMotion>().moveRight;
        int distnum;
        if (rightDirection)
        {
            for (int i = 0; i < rightObjects.Count; i++)
            {
                if (rightObjects[i] != targetObject)
                {
                    distnum = i;
                    SpawnObject(rightObjects[distnum]);
                }
            }
        }
        else if (!rightDirection)
        {
            for (int i = 0; i < leftObjects.Count; i++)
            {
                if (leftObjects[i] != targetObject)
                {
                    distnum = i;
                    SpawnObject(leftObjects[distnum]);
                }
            }
        }

        // increase total number of target direction distractors to account for removed target
        totalNum = totalNum + 1;
        return totalNum;
    }
}
