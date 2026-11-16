using TMPro;
using UnityEngine;
using UnityEngine.UI;



//진화에 필요한 재료에 대한 노드 ui 입니다.
public class ResourceNodeUI : MonoBehaviour
{
    [SerializeField] private Image materialImage;
    private TMP_Text countText;

    public void Init()
    {
        countText = GetComponentInChildren<TMP_Text>();
        HideUI();
    }

    public void SetData(Sprite materialSprite, int count,int requiredCount)
    {
        materialImage.sprite = materialSprite;
        countText.text = $"{count}/{requiredCount}";
        SetColorByCount(count,requiredCount);
    }

    private void SetColorByCount(int count, int requiredCount)
    { 
        if(count < requiredCount)
        {
            countText.color = Color.red;
        }
        else
        {
            countText.color = Color.green;
        }
    
    }

    public void ShowUI() => gameObject.SetActive(true);
    public void HideUI() => gameObject.SetActive(false);    
}
