using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool ReloadButton { get; private set; } = false;
    public bool InteractButton { get; private set; } = false;
    public bool InventoryButton { get; private set; } = false;
    public bool InventorySplitButton { get; private set; } = false;
    public bool PlayerStatButton { get; private set; } = false;
    public bool SprintButton { get; private set; } = false; 
    public bool RollButton { get; private set; } = false;
    public bool QuestButton { get; private set; } = false;  
    public Vector2 MoveInput { get; private set; } = Vector2.zero;
    public Vector2 MousePosition { get; private set; } = Vector2.zero;


    private void Update()
    {
        CheckMoveInput();
        CheckMousePosition();
        CheckInventoryButton();
        CheckInteractButton();
        CheckInventorySplitButton();
        CheckPlayerStatButton();
        CheckReloadButton();
        CheckSprintButton();
        CheckRollButton();
        CheckQuestButton();
    }

    private void CheckQuestButton()
     => QuestButton = Input.GetKeyDown(KeyCode.Q);
 
    private void CheckMoveInput()
        => MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

    private void CheckMousePosition()
        => MousePosition = Input.mousePosition;

    private void CheckInventorySplitButton()
        => InventorySplitButton = Input.GetKey(KeyCode.LeftAlt);

    private void CheckInventoryButton()
        => InventoryButton = Input.GetKeyDown(KeyCode.Tab);

    private void CheckPlayerStatButton()
        => PlayerStatButton = Input.GetKeyDown(KeyCode.C);

    private void CheckInteractButton()
        => InteractButton = Input.GetKey(KeyCode.E);

    private void CheckReloadButton()
        => ReloadButton = Input.GetMouseButtonDown((int)MouseButton.Right);

    private void CheckSprintButton()
        => SprintButton = Input.GetKey(KeyCode.LeftShift);

    private void CheckRollButton()
        => RollButton = Input.GetKeyDown(KeyCode.Space);
}
