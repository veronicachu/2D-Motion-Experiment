using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyReplace : MonoBehaviour {

    public GameObject prefabType;
    public float lifespanTime;

    Vector3 objectPosition;
    float timer = 0;

    // script references
    private GameObject experimentManagerRef;
    private ExpTrial m_ExpTrial;
    
	void Start () {
        experimentManagerRef = GameObject.Find("ExperimentManager");
        m_ExpTrial = experimentManagerRef.GetComponent<ExpTrial>();
	}
	
	void Update () {
        objectPosition = transform.position;        // get object's current position
        timer += Time.deltaTime;                    // keep timer counting

        // get boundaries
        float xMin = m_ExpTrial.xMin;
        float xMax = m_ExpTrial.xMax;
        float yMin = m_ExpTrial.yMin;
        float yMax = m_ExpTrial.yMax;

        // check if object location within boundaries
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

        // check if object lifespan within time
        if (timer > lifespanTime)
        {
            Destroy(this.gameObject);               // destroy this object
            m_ExpTrial.SpawnObject(prefabType);     // spawn another of this object
            timer = 0;                              // reset timer
        }
	}
}
