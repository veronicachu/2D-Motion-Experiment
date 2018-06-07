using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeWaypoints : MonoBehaviour
{
    public int nRow;                    // number of waypoints long
    public float ySpace;
    public GameObject waypointHolder;   // object script is placed on
    private GameObject waypoint;

    private float xCoord;
    private float yCoord;
    private float zCoord;
    
    void Awake ()
    //public void GenerateWaypoints()
    {
        for (int i = 0; i < nRow; i++)
        {
            waypoint = new GameObject("Waypoint" + i);
            waypoint.transform.parent = waypointHolder.transform;

            xCoord = waypointHolder.transform.position.x;
            yCoord = waypointHolder.transform.position.y - i*ySpace;
            zCoord = waypointHolder.transform.position.z;

            waypoint.transform.position = new Vector3(xCoord, yCoord, zCoord);
            waypoint.tag = waypointHolder.name;
        }
    }

    public void RemoveWaypoints()
    {
        // Find all the cube clones and destroy them
        List<GameObject> arrayObjects = new List<GameObject>();
        arrayObjects.AddRange(GameObject.FindGameObjectsWithTag("NearWaypoints"));
        arrayObjects.AddRange(GameObject.FindGameObjectsWithTag("FarWaypoints"));
        for (int i = 0; i < arrayObjects.Count; i++)
        {
            Destroy(arrayObjects[i]);
        }
    }
}
