///<summary>
/// This code is placed on a gameobject with a collider component
/// 
/// When another gameobject with a collider component hits this 
/// gameobject, then destroy the other gameobject
/// </summary>
using UnityEngine;

public class DestroyObject : MonoBehaviour {
    
    // OnTriggerEnter initiates once the collider is first hit, 
    // as opposed to OnTriggerExit
    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}
