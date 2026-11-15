using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController cc;
    private StatBase playerSpeed;
    private bool enableMove = true;
    private PlayerUnit player;

    private Vector3 fallVelocity;
    private float gravity = -9.81f;

    public void Init(PlayerUnit player, StatBase speed)
    {
        cc = GetComponent<CharacterController>();

        this.player = player;
        playerSpeed = speed;

        //Managers.Event.AddEvent<InteractEvent>(OnInteractToggle);

    }

    private void OnDisable()
    {
        //Managers.Event.RemoveEvent<InteractEvent>(OnInteractToggle);
    }

    //private void OnInteractToggle(InteractEvent data) => enableMove = !enableMove;
    

    private void Update()
    {
        if (player == null || !enableMove) return;
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        if(cc.isGrounded && fallVelocity.y < 0)
        {
            fallVelocity.y = -2f;
        }

        fallVelocity.y += gravity * Time.deltaTime;
        cc.Move(fallVelocity * Time.deltaTime);
        
    }

    public void Move(Vector2 dir, bool isSprinting)
    {
        float baseSpeed = player.unitStat.GetStat(EStatType.Speed);

        float currentSpeed = isSprinting ? (baseSpeed + 4f) : baseSpeed;

        cc.Move(new Vector3(dir.x, 0, dir.y) * currentSpeed * Time.deltaTime);
    }

    public void DashMove(Vector3 direction, float speed)
    {
        cc.Move(direction * speed * Time.deltaTime);
    }

    public void Teleport(Vector3 pos)
    {
        cc.enabled = false;
        
        player.transform.position = pos;

        cc.enabled = true;
    }

}
    