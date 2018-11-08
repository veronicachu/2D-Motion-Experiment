using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistractorMotion : MonoBehaviour {

    public float velocity;

    public Vector3 direction;
    public float magnitude;

    public GameObject ExpMangerObject;
    private ExpCue m_expCue;
    
    void Update () {
        
        transform.Translate(direction * Time.deltaTime * velocity * 1/magnitude);
    }

    public Vector3 SetDirection()
    {
        ExpMangerObject = GameObject.Find("ExperimentManager");
        m_expCue = ExpMangerObject.GetComponent<ExpCue>();

        // get target direction
        bool rightDirection = m_expCue.activeTarget.GetComponent<TargetMotion>().moveRight;

        // make list of possible x and y directions depending on target
        List<float> xDir = new List<float>();
        List<float> yDir = new List<float>();
        if (rightDirection)
        {
            xDir.Add(-1);
            xDir.Add(Mathf.Sqrt(2) / -2);

            yDir.Add(0);
            yDir.Add(Mathf.Sqrt(2) / 2);
            yDir.Add(Mathf.Sqrt(2) / -2);

            // maybe
            xDir.Add(Mathf.Sqrt(2) / 2);
            yDir.Add(Mathf.Sqrt(2) / 2);
            yDir.Add(Mathf.Sqrt(2) / -2);
        }
        else if (!rightDirection)
        {
            xDir.Add(1);
            xDir.Add(Mathf.Sqrt(2) / 2);

            yDir.Add(0);
            yDir.Add(Mathf.Sqrt(2) / 2);
            yDir.Add(Mathf.Sqrt(2) / -2);

            // maybe
            xDir.Add(Mathf.Sqrt(2) / -2);
            yDir.Add(Mathf.Sqrt(2) / 2);
            yDir.Add(Mathf.Sqrt(2) / -2);
        }

        // randomly select x and y directions
        int xIndex = Random.Range(0, xDir.Count);
        int yIndex = Random.Range(0, yDir.Count);

        float randX = xDir[xIndex];
        float randY = yDir[yIndex];

        //float randX = Random.Range(-1f, 1f);
        //float randY = Random.Range(-1f, 1f);
        direction = new Vector3(randX, randY, 0f);

        // calculate magnitude of motion vector
        magnitude = Mathf.Pow(randX,2f) + Mathf.Pow(randY,2);   // x^2 + y^2
        magnitude = Mathf.Sqrt(magnitude);                      // sqrt(x^2 + y^2)

        return direction;
    }
}
