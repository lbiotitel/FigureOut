using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameOverData
{ 
    public string killerName;
    public List<Sprite> lossItemImages;
}

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TMP_Text whoKilledText;

    [SerializeField] private GameObject lossItemPrefab;
    [SerializeField] private Transform lossItemContent;

    [SerializeField] private Button regameButton;

    private List<GameObject> lossItemObjects = new List<GameObject>();
    private Animator animator;


    private void Start()
    {
        regameButton.onClick.AddListener(() => Managers.Game.ResetGame());
        animator = GetComponent<Animator>();
        HideUI();
    }


    public void ShowUI(GameOverData gameOverData)
    {
        whoKilledText.text = $"{gameOverData.killerName}에게 죽었습니다.";
        DrawLossItemImages(gameOverData.lossItemImages);
        gameObject.SetActive(true);
        animator.SetTrigger("Show");
    }

    private void DrawLossItemImages(List<Sprite> lossItemImages)
    {
        ClearLossItemImages();
        for (int i = 0; i < lossItemImages.Count; i++)
        {
            GameObject itemImage = Instantiate(lossItemPrefab, lossItemContent);
            itemImage.GetComponent<Image>().sprite = lossItemImages[i];
            itemImage.GetComponent<Image>().preserveAspect = true;
            itemImage.SetActive(true);
            lossItemObjects.Add(itemImage);
        }
    }


    private void ClearLossItemImages()
    { 
        foreach(var obj in lossItemObjects)
        {
            Destroy(obj);
        }
        lossItemObjects.Clear();
    }


    public void HideUI() => gameObject.SetActive(false);
}
