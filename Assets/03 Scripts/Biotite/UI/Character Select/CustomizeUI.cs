using UnityEngine;

public class CustomizeUI : MonoBehaviour
{
    [SerializeField] private GameObject[] customizeUIObject;


    private void Start()
    {
        HideAllUI();
        ShowUI(0);
    }


    public void ShowUI(int index)
    {
        if (index < 0 || customizeUIObject.Length <= index) return;
        HideAllUI();
        customizeUIObject[index].SetActive(true);
    }


    private void HideAllUI()
    {
        foreach (var ui in customizeUIObject)
        {
            ui.SetActive(false);
        }
    }
}
