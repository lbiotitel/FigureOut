using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCam : MonoBehaviour
{
    private PlayerUnit playerUnit;
    private Vector3 offset;

    private void Start()
    {
        Managers.Event.AddEvent<PlayerSpawnEvent>(HandlePlayerSpawnEvent);
    }


    private void HandlePlayerSpawnEvent(PlayerSpawnEvent playerSpawnEvent)
    {
        playerUnit = playerSpawnEvent.player;
        offset = transform.position - playerUnit.transform.position;
        offset.z = 0;
        offset.x = 0f;
        Managers.Event.RemoveEvent<PlayerSpawnEvent>(HandlePlayerSpawnEvent);
    }

    private void LateUpdate()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        if (playerUnit == null) return;
        transform.position = playerUnit.transform.position + offset;
    }
}
