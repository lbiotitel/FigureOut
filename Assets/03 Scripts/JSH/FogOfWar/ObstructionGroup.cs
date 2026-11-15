
using UnityEngine;

public class ObstructionGroup : MonoBehaviour
{
    private MeshRenderer[] childRenderers;
    private bool isHidden = false;

    private void Awake()
    {
        // 게임 시작 시 딱 한 번만 자식들의 렌더러를 싹 모아서 보관합니다. (성능 소모 없음)
        childRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    // 호출되면 보관해둔 렌더러들을 일괄적으로 켜거나 끕니다.
    public void SetVisibility(bool visible)
    {
        if (isHidden == !visible) return; // 이미 원하는 상태라면 중복 연산 방지
        isHidden = !visible;

        for (int i = 0; i < childRenderers.Length; i++)
        {
            if (childRenderers[i] != null)
            {
                childRenderers[i].enabled = visible;
            }
        }
    }
}
