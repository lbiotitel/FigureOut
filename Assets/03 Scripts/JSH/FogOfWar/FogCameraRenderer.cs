using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FogCameraRenderer : MonoBehaviour
{
    [SerializeField] private FogRenderConfig config;

    [SerializeField] private Vector3 mapCenter = Vector3.zero;
    private Camera fogCam;

    private void Awake()
    {
        fogCam = GetComponent<Camera>();

        fogCam.orthographic = true;
        fogCam.orthographicSize = config.OrthographicSize;
        fogCam.nearClipPlane = 0.1f;
        fogCam.farClipPlane = 200f;
        fogCam.cullingMask = config.VisionLayer;
        fogCam.clearFlags = CameraClearFlags.SolidColor;
        fogCam.backgroundColor = Color.black;

        var urpData = fogCam.GetUniversalAdditionalCameraData();
        urpData.renderType = CameraRenderType.Base;
        urpData.renderShadows = false;
        urpData.requiresColorTexture = false;
        urpData.requiresDepthTexture = false;

        transform.position = new Vector3(mapCenter.x, mapCenter.y + config.CameraHeight, mapCenter.z);
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}

