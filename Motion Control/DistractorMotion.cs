using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistractorMotion : MonoBehaviour {

    public float velocity;

    public Vector3 direction;
    public float magnitude;
    
	void Update () {
        
        transform.Translate(direction * Time.deltaTime * velocity * 1/magnitude);
    }

    public Vector3 SetDirection ()
    {
        // set random movement vector
        float randX = Random.Range(-1f, 1f);
        float randY = Random.Range(-1f, 1f);
        direction = new Vector3(randX, randY, 0f);

        // calculate magnitude of motion vector
        magnitude = Mathf.Pow(randX,2f) + Mathf.Pow(randY,2);   // x^2 + y^2
        magnitude = Mathf.Sqrt(magnitude);                      // sqrt(x^2 + y^2)

        return direction;
    }
}
