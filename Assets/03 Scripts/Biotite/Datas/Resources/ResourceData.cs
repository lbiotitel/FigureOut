using UnityEngine;



//제료 데이터를 정의합니다.
[CreateAssetMenu()]
public class ResourceData : ItemData
{
    private void Awake()
    {
        itemType = EItemType.Resource;
    }
}
