using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterFaceNodeUI : MonoBehaviour
{
    [SerializeField] private Image faceIcon;
    private Button button;
    int index;

    public event Action<int> OnClicked;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Init(Sprite faceImage, int index)
    {
        this.index = index;
        faceIcon.sprite = faceImage;
        button.onClick.AddListener(() => {
            OnClicked?.Invoke(index);
        });
    }


}
