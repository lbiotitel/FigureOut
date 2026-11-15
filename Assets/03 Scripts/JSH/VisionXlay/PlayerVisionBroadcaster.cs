using UnityEngine;

public class PlayerVisionBroadcaster : BaseVisionBroadcaster
{
    protected override Vector3 GetTargetPosition()
    {
        return transform.position;
    }

}
