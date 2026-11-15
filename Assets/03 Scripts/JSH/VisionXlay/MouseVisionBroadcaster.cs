using UnityEngine;

public class MouseVisionBroadcaster : BaseVisionBroadcaster
{
    private PlayerLook playerLook;

    [SerializeField] private float forwardOffset = 3f;
    public void MouseInit(float radius, PlayerLook playerLook)
    {
        this.playerLook = playerLook;

        base.Init("MouseTargetPos", "_MouseHoleRadius", radius);
    }

    protected override Vector3 GetTargetPosition()
    {
        if (playerLook == null) return Vector3.zero;

        Vector3 mousePos = playerLook.MouseWorldPos;
        mousePos.z += forwardOffset;
        return mousePos;
    }
}
