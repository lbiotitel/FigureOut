using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoticeRegionName : MonoBehaviour
{
    [SerializeField] private BoxCollider colider;

    [SerializeField] private TextMeshProUGUI regionText;
    [SerializeField] private string regionName;

    [SerializeField] private Animator anim;

    private void Awake()
    {
        colider = gameObject.GetComponent<BoxCollider>();

        switch(gameObject.name)
        {
            case "Region1":
                regionName = "부량아들의 은신처";
                break;

            case "Region2":
                regionName = "우울한 무기 창고";
                break;

            case "Region3":
                regionName = "호화로웠던 마을";
                break;

            case "Region4":
                regionName = "황금향";
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            regionText.text = regionName;
            anim.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            anim.gameObject.SetActive(false);
        }
    }

}
