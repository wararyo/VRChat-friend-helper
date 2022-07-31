using UnityEngine;
using Valve.VR; //Steam VR
using System.IO;
using System;

public class OpenVRAutoLaunchManager : MonoBehaviour
{
    public string appKey = "wararyo.vrc-friend-helper";
    const string manifestFileName = "manifest.vrmanifest";

    void Start()
    {
#if UNITY_EDITOR
        string manifestPath = Path.GetFullPath(Path.Combine("Assets/", manifestFileName));
        RegisterOpenVrApplication(appKey, manifestPath);
#elif UNITY_STANDALONE_WIN
        string manifestPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'), manifestFileName);
        RegisterOpenVrApplication(appKey, manifestPath);
#endif
    }

    static EVRApplicationError RegisterOpenVrApplication(string appKey, string manifestPath)
    {
        Debug.Log("Manifest File Path: " + manifestPath);
        EVRApplicationError manifest_error = OpenVR.Applications.AddApplicationManifest(manifestPath, false);
        if (manifest_error != EVRApplicationError.None)
        {
            Debug.LogError("Could not add application manifest: " + OpenVR.Applications.GetApplicationsErrorNameFromEnum(manifest_error));
            return manifest_error;
        }
        Debug.Log("Successfully installed manifest with no errors: " + OpenVR.Applications.GetApplicationsErrorNameFromEnum(manifest_error));
        Debug.Log("Is Application installed? : " + OpenVR.Applications.IsApplicationInstalled(appKey));

        Debug.Log("Enabling overlay autostart...");
        EVRApplicationError autostart_error = OpenVR.Applications.SetApplicationAutoLaunch(appKey, true);
        if (autostart_error != EVRApplicationError.None)
        {
            Debug.LogError("Could not enable autostart: " + OpenVR.Applications.GetApplicationsErrorNameFromEnum(autostart_error));
            return autostart_error;
        }
        return EVRApplicationError.None;
    }

    public void Uninstall()
    {
        Debug.Log("Uninstalling...");
#if UNITY_EDITOR
        string manifestPath = Path.GetFullPath(Path.Combine("Assets/", manifestFileName));
#elif UNITY_STANDALONE_WIN
        string manifestPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'), manifestFileName);
#endif
        EVRApplicationError manifest_error = OpenVR.Applications.RemoveApplicationManifest(manifestPath);
        if (manifest_error != EVRApplicationError.None)
        {
            Debug.LogError("Could not remove application manifest: " + OpenVR.Applications.GetApplicationsErrorNameFromEnum(manifest_error));
            return;
        }
        Debug.Log("Successfully unregistered from OpenVR.");
    }
}
