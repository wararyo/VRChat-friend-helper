using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Valve.VR;

// 2つのコントローラーの距離を近づけた時にオーバーレイを表示する
[RequireComponent(typeof(EasyOpenVROverlayForUnity))]
public class ControllerGesture : MonoBehaviour
{
    public string actionManifestFileName = "actions.json";

    private EasyOpenVROverlayForUnity _overlay = null;
    private EasyOpenVROverlayForUnity overlay { get { if(_overlay == null) _overlay = GetComponent<EasyOpenVROverlayForUnity>(); return _overlay; } }

    ulong actionHandle = 0, actionSetHandle = 0;
    const string ACTION_SET_MAIN = "/actions/main";

    void Start()
    {
        overlay.show = false;

        // OpenVR 入力の初期化
#if UNITY_EDITOR
        string actionManifestPath = Path.GetFullPath(Path.Combine("Assets/", actionManifestFileName));
#elif UNITY_STANDALONE_WIN
        string actionManifestPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'), actionManifestFileName);
#endif
        Debug.Log("Action manifest path: " + actionManifestPath);
        EVRInputError error = OpenVR.Input.SetActionManifestPath(actionManifestPath);
        if (error != EVRInputError.None)
        {
            Debug.LogError(error);
            return;
        }
        error = OpenVR.Input.GetActionHandle("/actions/main/in/ShowOverlay", ref actionHandle);
        if (error != EVRInputError.None)
        {
            Debug.LogError(error);
            return;
        }
        error = OpenVR.Input.GetActionSetHandle(ACTION_SET_MAIN, ref actionSetHandle);
        if (error != EVRInputError.None)
        {
            Debug.LogError(error);
            return;
        }
    }

    void Update()
    {
        uint leftHandId = overlay.openvr.GetTrackedDeviceIndexForControllerRole(Valve.VR.ETrackedControllerRole.LeftHand);
        uint rightHandId = overlay.openvr.GetTrackedDeviceIndexForControllerRole(Valve.VR.ETrackedControllerRole.RightHand);

        // ボタンを押しているかどうか
        VRActiveActionSet_t[] actionSet = { new VRActiveActionSet_t { ulActionSet = actionSetHandle } };
        EVRInputError error = OpenVR.Input.UpdateActionState(actionSet, (uint)Marshal.SizeOf(typeof(VRActiveActionSet_t)));
        if (error != EVRInputError.None)
        {
            Debug.LogError(error);
            return;
        }
        InputDigitalActionData_t inputData = new InputDigitalActionData_t { };
        error = OpenVR.Input.GetDigitalActionData(actionHandle, ref inputData, (uint)Marshal.SizeOf(typeof(InputDigitalActionData_t)), 0);
        if (error != EVRInputError.None)
        {
            Debug.LogError(error);
            return;
        }
#if UNITY_EDITOR
        overlay.show = true;
#elif UNITY_STANDALONE_WIN
        overlay.show = inputData.bState;
#endif
        if (!overlay.show) return;

        // コントローラー位置の取得
        TrackedDevicePose_t[] allDevicePose = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
        overlay.openvr.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, 0f, allDevicePose);

        Vector3 leftHandPos = GetPositionFromPose(allDevicePose[leftHandId].mDeviceToAbsoluteTracking);
        Vector3 rightHandPos = GetPositionFromPose(allDevicePose[rightHandId].mDeviceToAbsoluteTracking);

        // コントローラー同士の距離を計算し、近かったらオーバーレイを表示
        float dist = Mathf.Clamp01(1.1f - Vector3.Distance(leftHandPos, rightHandPos)*3);
        overlay.alpha = dist;
    }

    Vector3 GetPositionFromPose(HmdMatrix34_t pose)
    {
        var m = Matrix4x4.identity;

        m[0, 0] = pose.m0;
        m[0, 1] = pose.m1;
        m[0, 2] = -pose.m2;
        m[0, 3] = pose.m3;

        m[1, 0] = pose.m4;
        m[1, 1] = pose.m5;
        m[1, 2] = -pose.m6;
        m[1, 3] = pose.m7;

        m[2, 0] = -pose.m8;
        m[2, 1] = -pose.m9;
        m[2, 2] = pose.m10;
        m[2, 3] = -pose.m11;

        var x = m.m03;
        var y = m.m13;
        var z = m.m23;

        return new Vector3(x, y, z);
    }
}
