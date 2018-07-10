using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMotion : MonoBehaviour {

    public bool moveRight;
    public float velocity;
    
    void Update () {
        if (moveRight)
        {
            transform.Translate(Vector3.right * Time.deltaTime * velocity);
        }
        
        if (!moveRight)
        {
            transform.Translate(Vector3.left * Time.deltaTime * velocity);
        }
	}
}
