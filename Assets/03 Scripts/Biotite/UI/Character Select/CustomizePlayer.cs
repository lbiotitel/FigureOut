using UnityEngine;

public class CustomizePlayer : MonoBehaviour
{
    [SerializeField] private MeshRenderer faceRenderer;
    [SerializeField] private MeshRenderer bodyRenderer;

    //머티리얼 인스턴스를 따로 만들지 않으면서도 해당 객체의 특정 속성(색상 등)만 다르게 그릴 수 있게 해줍니다.
    private MaterialPropertyBlock propBlock; 

    void Awake()
    {
        propBlock = new MaterialPropertyBlock();
    }

    public void ChangeColor(Color color)
    {
        bodyRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", color); 
        bodyRenderer.SetPropertyBlock(propBlock);
    }

    public void ChangeFace(Material faceIMat)
    {
        faceRenderer.sharedMaterial = faceIMat;
    }

    public Color GetColor()
    {
        bodyRenderer.GetPropertyBlock(propBlock);
        return propBlock.GetColor("_Color");
    }
    public Material GetFaceMaterial() => faceRenderer.sharedMaterial;

    void OnDestroy()
    {
        if (bodyRenderer != null) Destroy(bodyRenderer.material);
    }
}