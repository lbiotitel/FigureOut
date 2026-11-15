using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPos;
    [SerializeField] private Transform disappearPos;
    [SerializeField] private Transform movePos;
    [SerializeField] private float animDuration;

    [SerializeField] private GameObject rotor;
    [SerializeField] private float rotSpeed;



    private void Start()
    {
        MoveAnim();
    }


    private void Update()
    {
        RotorAnim();
    }

    private void RotorAnim() => rotor.transform.Rotate(0, rotSpeed * Time.deltaTime, 0);

    private void MoveAnim()
    {
        transform.DOMove(movePos.position, animDuration).
            OnComplete(() => { SpawnPlayer(); });
    }


    private void SpawnPlayer()
    {
        UnitData unitData = Managers.Data.GetData<UnitData>(KeyDefines.ResourceKey.PLAYER_DATA);

        GameObject playerObject = Managers.Object.CreateUnit(unitData, spawnPos.position);
        PlayerUnit playerUnit = playerObject.GetComponent<PlayerUnit>();

        playerUnit.SetSpawnPoint(spawnPos.position);

        Managers.Event.NotifyEvent<PlayerSpawnEvent>(new PlayerSpawnEvent { player = playerUnit });

        Managers.Audio.PlayBgm(KeyDefines.SoundKey.GAME_BGM, true);

        transform.DOMove(disappearPos.position,animDuration)
            .OnComplete(() => { gameObject.SetActive(false); });
    }
}
