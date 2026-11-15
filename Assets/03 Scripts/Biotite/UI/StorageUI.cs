using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageUI : DynamicInventoryUI
{
    protected override void Start()
    {
        Managers.Event.AddEvent<PlayerSpawnEvent>(HandlePlayerSpawn);
    }

 
    private void HandlePlayerSpawn(PlayerSpawnEvent data)
    {
        base.Start();
        Managers.Event.RemoveEvent<PlayerSpawnEvent>(HandlePlayerSpawn);
    }
}
