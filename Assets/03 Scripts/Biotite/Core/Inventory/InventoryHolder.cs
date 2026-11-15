using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class InventoryHolder : MonoBehaviour
{
    [SerializeField] private int inventorySize;
    [SerializeField] protected InventorySystem inventorySys;


    public InventorySystem InventorySys => inventorySys;

    private void Awake()
    {
        inventorySys = new InventorySystem(inventorySize);
    }

}
