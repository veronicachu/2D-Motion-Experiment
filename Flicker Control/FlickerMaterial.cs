using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerMaterial : MonoBehaviour
{

    // Setup variables for controlling flicker color and frequency
    public float Frequency;
    public Texture m_MainTexture, m_SecondTexture;
    Renderer m_Renderer;
    int textureCounter = 0;

    private void Start()
    {
        m_Renderer = GetComponent<Renderer>();
    }

    // This method starts the flicker at the given frequency rate
    public void StartFlicker()
    {
        float freq = (1.0f / (Frequency * 2f));
        InvokeRepeating("CycleColors", freq, freq);
    }

    // This method stops the flicker
    public void StopFlicker()
    {
        CancelInvoke("CycleColors");
        m_Renderer.material.SetTexture("_MainTex", m_MainTexture);
    }

    // This controls cycling between the two colors
    void CycleColors()
    {
        Texture[] textures = new Texture[2];
        textures[0] = m_MainTexture;
        textures[1] = m_SecondTexture;
        textureCounter = ++textureCounter % textures.Length;
        m_Renderer.material.SetTexture("_MainTex", textures[textureCounter]);
    }
}
