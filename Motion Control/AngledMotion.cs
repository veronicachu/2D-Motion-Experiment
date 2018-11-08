using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngledMotion : MonoBehaviour {

    public bool moveUp;
    public float velocity;

    void Update()
    {
        if (moveUp)
        {
            transform.Translate(Vector3.up * Time.deltaTime * velocity);
        }

        if (!moveUp)
        {
            transform.Translate(Vector3.down * Time.deltaTime * velocity);
        }
    }
}
