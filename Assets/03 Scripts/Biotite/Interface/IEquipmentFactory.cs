using UnityEngine;

public interface IEquipmentFactory 
{
    public void Init(Unit ownUnit);
    public T Create<T>(ItemData itemData);
}
