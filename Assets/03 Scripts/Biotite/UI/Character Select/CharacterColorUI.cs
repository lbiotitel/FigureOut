using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterColorUI : MonoBehaviour
{
    private List<CharacterColorNodeUI> characterColorNodeUIs = new List<CharacterColorNodeUI>();
    private CustomizePlayer player;

    private void Awake()
    {
        player = FindObjectOfType<CustomizePlayer>();
        Init();
    }

    private void Init()
    {
        characterColorNodeUIs.AddRange(GetComponentsInChildren<CharacterColorNodeUI>());

        foreach (var node in characterColorNodeUIs)
        {
            node.OnClicked += HandleClickedColor;
        }

        HandleClickedColor(Color.red);
    }

    private void OnDestroy()
    {
        foreach (var node in characterColorNodeUIs)
        {
            node.OnClicked -= HandleClickedColor;
        }
    }

    private void HandleClickedColor(Color color)
    {
        player.ChangeColor(color);
    }


}
