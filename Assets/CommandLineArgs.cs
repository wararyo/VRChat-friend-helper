using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR; //Steam VR

public class CommandLineArgs : MonoBehaviour
{
    public GameObject OpenVrManager;
    public OpenVRAutoLaunchManager autoLaunch;

    void Start()
    {
        string[] args = Environment.GetCommandLineArgs();
        foreach(string arg in args) {
            switch(arg)
            {
                case "--no-vr":
                    Debug.Log("Desktop mode");
                    OpenVrManager.SetActive(false);
                    break;

                case "--uninstall":
                    OpenVrManager.SetActive(false);
                    //OpenVRの初期化
                    var openVRError = EVRInitError.None;
                    OpenVR.Init(ref openVRError, EVRApplicationType.VRApplication_Overlay);
                    if (openVRError != EVRInitError.None)
                    {
                        Debug.LogError("Failed to initialize OpenVR: " + openVRError.ToString());
                        return;
                    }
                    autoLaunch.Uninstall();
                    break;
            }
        }
    }
}
