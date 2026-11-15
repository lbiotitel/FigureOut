using System;
using UnityEngine;



public class PickUpController : MonoBehaviour
{
    [SerializeField] private float magneticRange;

    public Action<ItemData,int> OnItemPickUp;

  
    private void Update()
    {
        CheckItemInRange();
    }

    private void CheckItemInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, magneticRange);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(KeyDefines.TagKey.DROP_ITEM_TAG))
            {
                if (collider.TryGetComponent<DropItem>(out DropItem dropItem))
                {
                    if (!dropItem.CanPickedUp ||  dropItem.IsPickedUp) return;
                    dropItem.OnPickUpAnimFinish += PickUp;
                    dropItem.PickUp(transform);
                    Logger.Log($"지금 충돌 된 아이템 {dropItem.name}");
                }
            }
        }
    }

    private void PickUp(DropItem dropItem)
    {
        OnItemPickUp?.Invoke(dropItem.ItemData,dropItem.Amount);
        dropItem.OnPickUpAnimFinish -= PickUp;
        Destroy(dropItem.gameObject);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, magneticRange);
    }

}
