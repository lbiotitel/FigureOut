using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu()]
public class PlayerFaceData : ScriptableObject
{

    [SerializeField] private List<PlayerFaceInfo> faceInfos;

    public List<PlayerFaceInfo> FaceInfos => faceInfos;
}

[Serializable]
public struct PlayerFaceInfo
{
    public Sprite faceIcon;
    public Material faceMaterial;
}
