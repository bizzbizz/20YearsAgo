using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonGame : MonoBehaviour
{
    public Camera cam;
    public Player player;
    public BloodEffect blood;
    public static SingletonGame Instance { get; private set; }

    public void SetBloodiness(float value)
    {
        if (value < 0.01f)
            blood.HoleSmooth = 0;
        else
            blood.HoleSmooth = .3f + value * .55f; //(Mathf.Log(value + 1) / Mathf.Log(1.2f));
    }
    public void SetHaze(bool value)
    {
        VisualEffects.Haze = value;
    }
    void Awake()
    {
        Instance = this;

        //fx_blood = cam.GetComponent<CameraFilterPack_Vision_Blood>();
        //fx_blood.HoleSize = 1;
        //fx_blood.HoleSmooth = 0;

        //fx_glitch = cam.GetComponent<CameraFilterPack_TV_Artefact>();
        //fx_glitch.enabled = false;
    }
    void Update()
    {
    }

}
