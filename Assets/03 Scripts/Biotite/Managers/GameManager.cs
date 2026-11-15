using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IManager
{
    private int dollar = 0;
    public int Dollar => dollar;
    public event Action<int> OnDollarValueChanged;

    [SerializeField] private GameOverUI gameOverUI;


    public PlayerUnit PlayerUnit { get; private set; }

    public void Init()
    {
        Managers.Event.AddEvent<PlayerSpawnEvent>(InitPlayer);
        Managers.Event.AddEvent<PlayerDeathEvent>(ShowGameOverUI);
    }


    private void InitPlayer(PlayerSpawnEvent playerSpawnEvent)
    {
        PlayerUnit = playerSpawnEvent.player;
        OnDollarValueChanged?.Invoke(dollar);
        Managers.Event.RemoveEvent<PlayerSpawnEvent>(InitPlayer);
    }

    #region About Dollar

    public void SpendDollar(int dollar)
    {
        if (this.dollar < dollar) return;
        this.dollar = this.dollar - dollar;
        OnDollarValueChanged?.Invoke(this.dollar);
    }

    public void AddDollar(int dollar)
    {
        if (dollar < 0) return; 
        this.dollar += dollar;
        OnDollarValueChanged?.Invoke(this.dollar);
    }

    #endregion


    private void ShowGameOverUI(PlayerDeathEvent playerDeathEvent)
    {
        InventorySystem inventorySys = PlayerUnit.InventoryHolder.InventorySys;

        List<Sprite> lossItemImages = new List<Sprite>();

        foreach (var slot in inventorySys.InventorySlots)
        {
            if (slot.IsEmpty()) continue;
            lossItemImages.Add(slot.ItemData.ItemIcon);
        }

        GameOverData gameOverData = new GameOverData
        {
            killerName = playerDeathEvent.killerName,
            lossItemImages = lossItemImages
        };

        gameOverUI.ShowUI(gameOverData);
        inventorySys.ClearItem();
    }


    //게임을 계속합니다.
    public void ResetGame()
    {
        Managers.Object.ResetAllEnemies();
        PlayerUnit.ResetPlayer();
        gameOverUI.HideUI();        
    }



    public void Release()
    {
        Managers.Event.RemoveEvent<PlayerDeathEvent>(ShowGameOverUI);
    }

}
