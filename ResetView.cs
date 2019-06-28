/// <summary>
/// Code to enable user to reset camera in VR by hitting 'r' key
/// </summary>

using UnityEngine;

public class ResetView : MonoBehaviour {
    
	void Update ()
    {
        // Resets the orientation of the Rift
        if (Input.GetKeyDown("r"))
        {
            UnityEngine.XR.InputTracking.Recenter();
        }
    }
}
