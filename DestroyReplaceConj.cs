/// <summary>
/// This code destroys the gameobject it is placed on when it either reaches 
/// a specified boundary's borders or exceeds a specified lifespan, and replaces 
/// a copy of the destroyed gameobject at a random location within the boundary 
/// space
/// 
/// /// Place this code on a prefab object that will be called by another script 
/// (e.g. ExpTrialConj.m)
/// </summary>

using UnityEngine;

public class DestroyReplaceConj : MonoBehaviour
{
    public GameObject prefabType;       // input the gameobject this script is on
    public float lifespanTime;          // input max lifespan time (in sec)

    private Vector3 objectPosition;
    private float timer = 0;

    // script references
    private GameObject experimentManagerRef;
    private ExpTrialConj m_ExpTrialConj;

    void Start()
    {
        experimentManagerRef = GameObject.Find("ExperimentManager");        // access the ExperimentManager gameobject
        m_ExpTrialConj = experimentManagerRef.GetComponent<ExpTrialConj>(); // access the ExpTrialConj script on ExperimentManager
    }

    void Update()
    {
        objectPosition = transform.position;        // get object's current position
        timer += Time.deltaTime;                    // keep timer counting

        // get specified boundaries from ExpTrialConj script
        float xMin = m_ExpTrialConj.xMin;
        float xMax = m_ExpTrialConj.xMax;
        float yMin = m_ExpTrialConj.yMin;
        float yMax = m_ExpTrialConj.yMax;

        // check if object location within specified boundaries
        if (objectPosition.x < xMin || objectPosition.x > xMax)
        {
            Destroy(this.gameObject);                   // destroy this object
            m_ExpTrialConj.SpawnObject(prefabType);     // spawn another of this object
        }
        if (objectPosition.y < yMin || objectPosition.y > yMax)
        {
            Destroy(this.gameObject);                   // destroy this object
            m_ExpTrialConj.SpawnObject(prefabType);     // spawn another of this object
        }

        // check if object lifespan within max time
        if (timer > lifespanTime)
        {
            Destroy(this.gameObject);                   // destroy this object
            m_ExpTrialConj.SpawnObject(prefabType);     // spawn another of this object
            timer = 0;                                  // reset timer
        }
    }
}
