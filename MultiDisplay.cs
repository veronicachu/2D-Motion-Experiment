/// <summary>
/// Allow for the program to run on two or more monitors
/// 
/// Useful when have two cameras in the scene that show different things 
/// happening simultaneously
/// </summary>

using UnityEngine;

public class MultiDisplay : MonoBehaviour {
    
	void Start () {
        // Display.displays[0] is the primary, default display and is always ON.
        // Check if additional displays are available and activate each.
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
        if (Display.displays.Length > 2)
            Display.displays[2].Activate();

        Debug.Log("displays connected: " + Display.displays.Length);
    }
}
