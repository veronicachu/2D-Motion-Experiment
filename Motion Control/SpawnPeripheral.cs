using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPeripheral : MonoBehaviour {

    public GameObject rightObject;
    public GameObject leftObject;

    List<GameObject> leftWaypoints = new List<GameObject>();
    List<GameObject> rightWaypoints = new List<GameObject>();

    private void Start()
    {
        leftWaypoints.AddRange(GameObject.FindGameObjectsWithTag("LeftWaypoint"));
        rightWaypoints.AddRange(GameObject.FindGameObjectsWithTag("RightWaypoint"));
    }

    public void SpawnFromBothColumns()
    {
        // rightward motion from left side
        for (int i = 0; i < leftWaypoints.Count; i++)
        {
            Vector3 waypointPosition = leftWaypoints[i].transform.position;
            GameObject newObject = Instantiate(leftObject, waypointPosition, leftObject.transform.rotation);
            newObject.tag = "FlickerClone";
            newObject.transform.parent = gameObject.transform;
        }

        // leftward from right side
        for (int i = 0; i < rightWaypoints.Count; i++)
        {
            Vector3 waypointPosition = rightWaypoints[i].transform.position;
            GameObject newObject = Instantiate(rightObject, waypointPosition, rightObject.transform.rotation);
            newObject.tag = "FlickerClone";
            newObject.transform.parent = gameObject.transform;
        }
    }
}
