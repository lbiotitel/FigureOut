using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour,IPoolingObject
{
    [SerializeField] private float yHeight;
    [SerializeField] private float xRange;

    [SerializeField] private float animSpeed;
    [SerializeField] private float animDuration;

    private TMP_Text damageText;

    public GameObject OriginPrefab { get; set; }

    private void Awake()
    {
        damageText = GetComponentInChildren<TMP_Text>();
    }

    public void Init(float damage, EHitType hitType)
    {
        damageText.text = damage.ToString();
        SetColor(hitType);
        DoAnimation();
    }



    private void SetColor(EHitType hitType)
    {
        switch (hitType)
        {
            case EHitType.Head:
                damageText.color = Color.red;
                break;
            case EHitType.Body:
                damageText.color = Color.white;
                break;
        }
    }


    private void DoAnimation()
    { 
        float randX = Random.Range(-xRange, xRange);
        transform.DOJump(transform.position + new Vector3(randX, yHeight, 0), 1, 1, animDuration)
            .OnComplete(() => { ReturnObject(); });
        damageText.DOFade(0, animSpeed);
    }

    public void ResetObject()
    {
        transform.DOKill();
    }

    public void ReturnObject()
    {
        ResetObject();
        Managers.Object.PoolingSystem.ReturnObject(gameObject);
    }
}