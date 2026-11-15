using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorNodeUI : MonoBehaviour
{
    public event Action<Color> OnClicked;
    private Button button;
    private Image color;
    private void Awake()
    {
        color = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            OnClicked?.Invoke(color.color);
        });
    }


}
