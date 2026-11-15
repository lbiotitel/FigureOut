using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FogRenderConfig", menuName = "FogOfWar/Render Config")]
public class FogRenderConfig : ScriptableObject
{
    [SerializeField] private int resolution = 512;
    [SerializeField] private FilterMode filterMode = FilterMode.Bilinear;
    [SerializeField] private float cameraHeight = 40f;
    [SerializeField] private float orthographicSize = 125f;
    [SerializeField] private Color fogColor = new Color(0, 0, 0, 0.5f);

    [Range(0.1f, 5f)]
    [SerializeField] private float meshResloution = 2f;
    [Range(1, 10)]
    [SerializeField] private int edgeResolveIterations = 4;
    [SerializeField] private float edgeDstThreshold = 0.5f;

    [SerializeField] private LayerMask visionLayer;


    public int Resolution => resolution;
    public FilterMode FilterMode => filterMode;
    public float CameraHeight => cameraHeight;
    public float OrthographicSize => orthographicSize;
    public Color FogColor => fogColor;
    public float MeshResloution => meshResloution;
    public int EdgeResolveIterations => edgeResolveIterations;
    public float EdgeDstThreshold => edgeDstThreshold;
    public LayerMask VisionLayer => visionLayer;
}

