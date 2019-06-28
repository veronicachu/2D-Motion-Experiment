/// <summary>
/// Load a scene using the scene's name when 'Enter' key hit
/// </summary>

using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveOnBreak : MonoBehaviour {
    
	void Update () {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("TaskMonitor");
        }
    }
}
