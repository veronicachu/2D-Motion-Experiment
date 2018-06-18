using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlappedFlicker : MonoBehaviour {
    
    private FlickerManager m_FlickerManager;

    void Awake()
    {
        m_FlickerManager = this.GetComponent<FlickerManager>();

    }

    void Start()
    {
        m_FlickerManager.StartAllFlicker();

    }
}
