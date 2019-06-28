/// <summary>
/// This code destroys the gameobject it is placed on when it either reaches 
/// a specified boundary's borders or exceeds a specified lifespan, and replaces 
/// a copy of the destroyed gameobject at a random location within the boundary 
/// space
/// 
/// Place this code on a prefab object that will be called by another script 
/// (e.g. ExpTrial.m)
/// </summary>

using UnityEngine;

public class DestroyReplace : MonoBehaviour {

    public GameObject prefabType;       // input the gameobject this script is on
    public float lifespanTime;          // input max lifespan time (in sec)

    private Vector3 objectPosition;
    private float timer = 0;

    // script references
    private GameObject experimentManagerRef;
    private ExpTrial m_ExpTrial;
    
	void Start ()
    {
        experimentManagerRef = GameObject.Find("ExperimentManager");    // access the ExperimentManager gameobject
        m_ExpTrial = experimentManagerRef.GetComponent<ExpTrial>();     // access the ExpTrial script on ExperimentManager
	}
	
	void Update ()
    {
        objectPosition = transform.position;        // get object's current position
        timer += Time.deltaTime;                    // keep timer counting

        // get specified boundaries from ExpTrial script
        float xMin = m_ExpTrial.xMin;
        float xMax = m_ExpTrial.xMax;
        float yMin = m_ExpTrial.yMin;
        float yMax = m_ExpTrial.yMax;

        // check if object location within specified boundaries
        if (objectPosition.x < xMin || objectPosition.x > xMax)
        {
            Destroy(this.gameObject);               // destroy this object
            m_ExpTrial.SpawnObject(prefabType);     // spawn another of this object
        }
        if (objectPosition.y < yMin || objectPosition.y > yMax)
        {
            Destroy(this.gameObject);               // destroy this object
            m_ExpTrial.SpawnObject(prefabType);     // spawn another of this object
        }

        // check if object lifespan within max time
        if (timer > lifespanTime)
        {
            Destroy(this.gameObject);               // destroy this object
            m_ExpTrial.SpawnObject(prefabType);     // spawn another of this object
            timer = 0;                              // reset timer
        }
	}
}
