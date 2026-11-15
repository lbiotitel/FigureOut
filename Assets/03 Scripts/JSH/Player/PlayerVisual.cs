using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer faceRenderer;
    [SerializeField] private MeshRenderer bodyRenderer;
    private MaterialPropertyBlock propBlock;

    public void Init(Color color, Material faceMat)
    {
        propBlock = new MaterialPropertyBlock();

        bodyRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", color);
        bodyRenderer.SetPropertyBlock(propBlock);

        faceRenderer.material = faceMat;
    }
}
