using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPeripheral : MonoBehaviour {

    public GameObject leftwardObject;
    public GameObject rightwardObject;

    public int rightNRows;
    public int leftNRows;

    public float lowestY;
    public float highestY;

    private GameObject leftWaypoint;
    private GameObject rightWaypoint;

    private float leftX;
    private float leftZ;
    private float rightX;
    private float rightZ;

    private void Start()
    {
        leftWaypoint = GameObject.Find("LeftWaypoint");
        rightWaypoint = GameObject.Find("RightWaypoint");

        leftX = leftWaypoint.transform.position.x;
        leftZ = leftWaypoint.transform.position.z;

        rightX = rightWaypoint.transform.position.x;
        rightZ = rightWaypoint.transform.position.z;
    }

    public void SpawnLeftwardMotion()
    {
        // leftward from right side
        for (int i = 0; i < rightNRows; i++)
        {
            float rightY = Random.Range(lowestY, highestY);

            GameObject newObject = Instantiate(leftwardObject, new Vector3(rightX, rightY, rightZ), leftwardObject.transform.rotation);
            newObject.tag = "FlickerClone";
            newObject.transform.parent = gameObject.transform;
        }
    }

    public void SpawnRightwardMotion()
    {
        // rightward motion from left side
        for (int i = 0; i < leftNRows; i++)
        {
            float leftY = Random.Range(lowestY, highestY);

            GameObject newObject = Instantiate(rightwardObject, new Vector3(leftX, leftY, leftZ), rightwardObject.transform.rotation);
            newObject.tag = "FlickerClone";
            newObject.transform.parent = gameObject.transform;
        }
    }

    public void SpawnFromBothColumns()
    {
        // rightward motion from left side
        for (int i = 0; i < leftNRows; i++)
        {
            float leftY = Random.Range(lowestY, highestY);

            GameObject newObject = Instantiate(rightwardObject, new Vector3(leftX,leftY,leftZ), rightwardObject.transform.rotation);
            newObject.tag = "FlickerClone";
            newObject.transform.parent = gameObject.transform;
        }

        // leftward from right side
        for (int i = 0; i < rightNRows; i++)
        {
            float rightY = Random.Range(lowestY, highestY);

            GameObject newObject = Instantiate(leftwardObject, new Vector3(rightX,rightY,rightZ), leftwardObject.transform.rotation);
            newObject.tag = "FlickerClone";
            newObject.transform.parent = gameObject.transform;
        }
    }
}
