/// <summary>
/// This code calculates the FPS for benchmarking/performance tracking purposes
///</summary>

using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour {

    public int Granularity = 5; // how many frames to wait until you re-calculate the FPS
    public double fps;          // calculated fps

    private List<double> times = new List<double>();
    private int counter = 5;                            // set initially to same number as Granularity
    
    public void Update()
    {
        // When counter down to 0, call the CalcFPS method
        if (counter <= 0)
        {
            CalcFPS();
            counter = Granularity;  // set counter back to original value
        }

        times.Add(Time.deltaTime);  // create list of times to use when measuring fps
        counter--;
    }

    // Method calculates FPS by taking the average time
    public void CalcFPS()
    {
        double sum = 0;
        foreach (double F in times)
        {
            sum += F;   // add the times
        }

        double average = sum / times.Count;     // calculate the average of the added times
        fps = 1 / average;                      // take the reciprocal of the average (frequency)

        // update a GUIText or something
    }
}
