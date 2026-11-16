using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;

public class CharacterFaceUI : MonoBehaviour
{
    [SerializeField] private CharacterFaceNodeUI faceNodeUIPrefab;
    [SerializeField] private Transform gridLayout;

    private PlayerFaceData playerFaceData;
    List<CharacterFaceNodeUI> faceNodeUIs = new List<CharacterFaceNodeUI>();

    private CustomizePlayer player;


    private void Start()
    {
        player = FindObjectOfType<CustomizePlayer>();
        Init();
    }


    private void Init()
    {
        playerFaceData = Managers.Data.GetData<PlayerFaceData>("Face Datas");

        int index = 0;
        foreach (var face in playerFaceData.FaceInfos)
        {
            CharacterFaceNodeUI slot = Instantiate(faceNodeUIPrefab, gridLayout);
            slot.Init(face.faceIcon, index++);
            slot.OnClicked += HandleFaceClicked;
            faceNodeUIs.Add(slot);
        }

        HandleFaceClicked(0);
    }

    private void OnDestroy()
    {
        foreach (var node in faceNodeUIs)
        {
            node.OnClicked -= HandleFaceClicked;
        }
    }

    private void HandleFaceClicked(int index)
    {
        Logger.Log($"{index} 얼굴 선택");
        PlayerFaceInfo selectedFace = playerFaceData.FaceInfos[index];
        player.ChangeFace(selectedFace.faceMaterial);
    }
}
