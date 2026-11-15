using Biotite;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Transform classButtonTransform;
    [SerializeField] private TMP_Text classNameText;
    [SerializeField] private Button gameStartButton;
    [SerializeField] private HelecopterAnim helecopterAnim;

    private CustomizePlayer player;
    private List<ClassButtonUI> classButtons = new List<ClassButtonUI>();

    private CanvasGroup canvasGroup;
    private EClassType currentClassType;
    private bool gameStart = false;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        gameStartButton.onClick.AddListener(GameStart);
        InitClassButtonUI();
        HandleClickedClassButton(EClassType.Farmer);
        helecopterAnim.OnAnimFinished += () =>
        {
            Managers.Scene
            .NewTransition()
            .Unload(EScene.CharacterSelect.ToString())
            .UnLoadResources()
            .LoadResources(EScene.Game.ToString())
            .Load(EScene.Game.ToString())
            .WithOverlay()
            .Perform();
        };
        player = FindAnyObjectByType<CustomizePlayer>();
    }


    private void InitClassButtonUI()
    {
        for (int i = 0; i < classButtonTransform.childCount; i++)
        {
            ClassButtonUI classButton = classButtonTransform.GetChild(i).GetComponent<ClassButtonUI>();
            if (classButton != null)
            {
                classButtons.Add(classButton);
                classButton.OnClicked += HandleClickedClassButton;
            }
        }
    }

    private void GameStart()
    {
        if (gameStart) return;
        gameStart = true;

        CharaterSelectData.selectedClassType = currentClassType;
        CharaterSelectData.selectedColor = player.GetColor();
        CharaterSelectData.selectedFaceMat = player.GetFaceMaterial();

        Logger.Log("게임 시작");
        helecopterAnim.MoveWayPoint();
        canvasGroup.interactable = false;
        canvasGroup.DOFade(0f, 1f);
    }



    private void HandleClickedClassButton(EClassType classType)
    {
        currentClassType = classType;
        classNameText.text = Utils.ClassTypeToKor.GetKor(classType);
        Logger.Log($"{classType} 선택");
    }

}
