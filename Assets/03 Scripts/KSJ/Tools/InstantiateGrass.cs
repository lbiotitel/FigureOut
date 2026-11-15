using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateGrass : MonoBehaviour
{
    [Header("Grass")]
    [SerializeField] private GameObject objGrass;
    [Range(0, 180)]
    [SerializeField] private int grassRotationY;
    [SerializeField] private Vector3 grassScale;
    [SerializeField] private int grassAmount = 50;

    [Header("Area")]
    // 잔디 위 부모(계층 구조)
    [SerializeField] private Transform plantParent;
    // 박스 콜라이더로 생성 범위 지정
    [SerializeField] private BoxCollider plantBoundary;

#if UNITY_EDITOR
    [ContextMenu("잔디 심기")]
    public void PlantGrass()
    {
        //float rangeX = plantBoundary.bounds.size.x;
        //float rangeZ = plantBoundary.bounds.size.z;

        for (int i = 0; i < grassAmount; i++)
        {
            float posX = Random.Range(plantBoundary.bounds.min.x, plantBoundary.bounds.max.x);
            float posZ = Random.Range(plantBoundary.bounds.min.z, plantBoundary.bounds.max.z);
            //float posX = Random.Range(plantBoundary.center.x + ((rangeX / 2) * -1), plantBoundary.center.x + (rangeX / 2));
            //float posZ = Random.Range(plantBoundary.center.z + ((rangeZ / 2) * -1), plantBoundary.center.z + (rangeZ / 2));

            Vector3 grassPosition = new Vector3(posX, 0f, posZ);

            grassRotationY = Random.Range(0, 181);
            Vector3 grassRotation = new Vector3(0, grassRotationY, 0);

            GameObject clone = Instantiate(objGrass, grassPosition, Quaternion.Euler(grassRotation), plantParent);
        }

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
    }
#endif
}
