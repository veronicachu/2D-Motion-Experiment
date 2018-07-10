using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FovealFlicker : MonoBehaviour {

    public float timeBetweenPeripheralSpawn;
    public bool bothSideSpawn;

    private float peripheralTimer;

    private SpawnPeripheral m_SpawnPeriperal;
    private GameObject peripheralDisplayRef;

    private FlickerManager m_FlickerManager;

    void Awake () {
        peripheralDisplayRef = GameObject.Find("Peripheral Display");
        m_SpawnPeriperal = peripheralDisplayRef.GetComponent<SpawnPeripheral>();
        m_FlickerManager = this.GetComponent<FlickerManager>();
        
    }
	
	void Start () {
        m_FlickerManager.StartAllFlicker();

    }

    private void Update()
    {
        StartPeripheral();                                      // keep peripheral dots spawning and moving
    }

    void StartPeripheral()
    {
        peripheralTimer += Time.deltaTime;

        if (peripheralTimer > timeBetweenPeripheralSpawn && bothSideSpawn)
        {
            m_SpawnPeriperal.SpawnFromBothColumns();
            peripheralTimer = 0;
        }

        if (peripheralTimer > timeBetweenPeripheralSpawn && !bothSideSpawn)
        {
            m_SpawnPeriperal.SpawnLeftwardMotion();
            peripheralTimer = 0;
        }
    }
}
