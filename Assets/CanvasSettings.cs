using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasSettings : MonoBehaviour
{
    const float BASE_WIDTH = 640; // OpenVRのWidthが1になるようなCanvasの横解像度

    public enum CanvasType
    {
        Vr,
        Desktop
    }

    public CanvasType canvasType;
    public RenderTexture renderTexture;
    public EasyOpenVROverlayForUnity openVr;
    public Camera canvasCamera;
    public Text text;

    CanvasScaler canvasScaler { get { return GetComponent<CanvasScaler>(); } }

    public void onSettingsLoaded()
    {
        // テキストサイズを変更
        text.fontSize = SettingsProvider.settings.vrFontSize;
        if (canvasType == CanvasType.Vr)
        {
            // ウィンドウサイズを変更
            SettingsProvider.RectSize windowSize = SettingsProvider.settings.vrWindowSize;
            renderTexture.width = windowSize.width;
            renderTexture.height = windowSize.height;
            canvasCamera.targetTexture = null;
            canvasCamera.targetTexture = renderTexture;
            canvasScaler.referenceResolution = (Vector2)windowSize;
            openVr.width = windowSize.width / BASE_WIDTH;
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
