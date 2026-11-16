using System.Collections.Generic;
using UnityEngine;

public class ConsoleSystem : MonoBehaviour
{
    private bool showConsole = false;
    string input = "";

    private List<string> inputHistory = new List<string>();
    private int historyIndex = -1;



    private static ConsoleCommand<string> QUEST_COMMAND;
    private static ConsoleCommand<int> INVINCIBLE_COMMAND;
    private static ConsoleCommand<string,int> SPAWN_COMMAND;
    private static ConsoleCommand<string> ENEMY_SPAWN_COMMAND;
    private static ConsoleCommand<int> ADD_DOLLAR_COMMAND;
    private static ConsoleCommand<int, int, int> TELEPORT_PLAYER;
    private static ConsoleCommand<string> TELEPORT_TO_WAYPOINT;
    private static ConsoleCommand HELP_COMMAND;
    private static ConsoleCommand<int> TEST_COMMAND;

    private List<ConsoleCommandBase> commandList = new List<ConsoleCommandBase>();

    private Vector2 scrollPosition;

    private void Start()
    {
        TEST_COMMAND = new ConsoleCommand<int>("test", "Test Command", "test [test number]", (int index) =>
        {
            PlayerUnit playerUnit = GameObject.FindAnyObjectByType<PlayerUnit>();
            Vector3 targetPos = playerUnit != null ? playerUnit.transform.position + new Vector3(0, 0.25F, 0) : Vector3.zero;
            if (index == 1)
            {

                SPAWN_COMMAND.Execute("10010_WE_AR_001", 1);
                SPAWN_COMMAND.Execute("10020_AM_001", 999);
                SPAWN_COMMAND.Execute("10030_1_0011", 999);
            }
            else if (index == 2)
            {
                SPAWN_COMMAND.Execute("10010_AR_BA_001", 1);
                SPAWN_COMMAND.Execute("10010_AR_HE_001", 1);
            }
        });

        QUEST_COMMAND = new ConsoleCommand<string>("quest", "regist quest", "quest [quest ID]", (questID) =>
        {
            Managers.Quest.RegistQuest(questID);
        });

        INVINCIBLE_COMMAND = new ConsoleCommand<int>("invincible", "set invincible", "invincible [bool]", (onOff) =>
        {
            PlayerUnit playerUnit = Managers.Game.PlayerUnit;
            playerUnit.IsInvincible = onOff == 1 ? true : false;

        });

        ENEMY_SPAWN_COMMAND = new ConsoleCommand<string>("enemy", "spawn enemy", "enemy [enemy ID]", (string enemyID) =>
        {
            PlayerUnit playerUnit = GameObject.FindAnyObjectByType<PlayerUnit>();
            Vector3 targetPos = playerUnit != null ? playerUnit.transform.position + new Vector3(0, 0.25F, 0) : Vector3.zero;

            EnemyData enemyData = Managers.Data.GetData<EnemyData>(enemyID);

            Managers.Object.CreateUnit(enemyData, targetPos, null);
        });

        SPAWN_COMMAND = new ConsoleCommand<string, int>("spawn", "Spawns an item.", "spawn [itemCode] [amount]", (itemCode, amount) =>
        {
            PlayerUnit playerUnit = GameObject.FindAnyObjectByType<PlayerUnit>();
            ItemData itemData = Managers.Data.GetData<ItemData>(itemCode);

            Vector3 targetPos = playerUnit != null ? playerUnit.transform.position + new Vector3(0, 0.25F, 0) : Vector3.zero;
            int currentAmount = amount;
            int stackSize = itemData.MaxStackCount;
            while (currentAmount > 0)
            {
                if (currentAmount > stackSize)
                {
                    currentAmount -= stackSize;
                    Managers.Object.CreateDropObject(itemData, stackSize, targetPos, Quaternion.identity);
                }
                else
                {
                    Managers.Object.CreateDropObject(itemData, currentAmount, targetPos, Quaternion.identity);
                    currentAmount = 0;
                }
            }

        });

        ADD_DOLLAR_COMMAND = new ConsoleCommand<int>("dollar", "Adds dollar.", "dollar [amount]", (amount) =>
        {
            Debug.Log("골드 추가");
            Managers.Game.AddDollar(amount);
        });

        TELEPORT_PLAYER = new ConsoleCommand<int, int, int>("teleport", "Teleport Player", "teleport [x] [y] [z]", (x, y, z) =>
        {
            PlayerUnit player = Managers.Game.PlayerUnit;
            if (player != null && !player.IsDead)
            {
                player.PlayerMovement.Teleport(new Vector3(x, y, z));
            }
        });

        HELP_COMMAND = new ConsoleCommand("help", "Displays all commands", "help", () =>
        {
            inputHistory.Add("--- Commands ---");
            foreach (var command in commandList)
            {
                string res = $"{command.CommandID} : {command.CommandDescription} | {command.CommandFormat}";
                inputHistory.Add(res);
            }
        });

        TELEPORT_TO_WAYPOINT = new ConsoleCommand<string>("teleport", "teleport player to waypoint", "teleport [waypoint name]", (string name) =>
        {
            Vector3 wayPointPos = Utils.WayPoint.GetWayPointPos(name);
            if (wayPointPos == Vector3.zero) return;
            TELEPORT_PLAYER.Execute((int)wayPointPos.x, (int)wayPointPos.y, (int)wayPointPos.z);
        });

        commandList.Add(INVINCIBLE_COMMAND);
        commandList.Add(QUEST_COMMAND);
        commandList.Add(SPAWN_COMMAND);
        commandList.Add(ENEMY_SPAWN_COMMAND);
        commandList.Add(ADD_DOLLAR_COMMAND);
        commandList.Add(TELEPORT_PLAYER);
        commandList.Add(HELP_COMMAND);
        commandList.Add(TEST_COMMAND);
        commandList.Add(TELEPORT_TO_WAYPOINT);
    }


    private void NavigateHistory(int direction)
    {
        if (inputHistory.Count == 0) return;

        historyIndex += direction;

        if (historyIndex < 0)
        {
            historyIndex = -1;
            input = "";
            return;
        }

        if (historyIndex >= inputHistory.Count)
        {
            historyIndex = inputHistory.Count - 1;
        }

        input = inputHistory[inputHistory.Count - 1 - historyIndex];
    }

    public void OnGUI()
    {
        Event currentEvent = Event.current;
        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.F1)
        {
            showConsole = !showConsole;
            if (showConsole) input = "";
        }

        if (!showConsole) return;

        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Return)
        {
            OnReturn();
            currentEvent.Use();
        }

        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.UpArrow)
        {
            NavigateHistory(1);
        }
        else if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.DownArrow)
        {
            NavigateHistory(-1);
        }


        float consoleHeight = 300f;
        float inputFieldHeight = 50f;
        float padding = 10f;

        GUI.backgroundColor = new Color(0, 0, 0, 0.85f);
        GUI.Box(new Rect(0, 0, Screen.width, consoleHeight), "");

        GUIStyle logStyle = new GUIStyle(GUI.skin.label);
        logStyle.fontSize = 20;
        logStyle.normal.textColor = Color.green;

        float scrollAreaHeight = consoleHeight - inputFieldHeight - (padding * 2);
        Rect viewport = new Rect(padding, padding, Screen.width - 20, scrollAreaHeight);
        Rect content = new Rect(0, 0, Screen.width - 40, inputHistory.Count * 25f);

        scrollPosition = GUI.BeginScrollView(viewport, scrollPosition, content);
        for (int i = 0; i < inputHistory.Count; i++)
        {
            Rect labelRect = new Rect(5, i * 25f, content.width, 25f);
            GUI.Label(labelRect, $"> {inputHistory[i]}", logStyle);
        }
        GUI.EndScrollView();

        GUIStyle inputStyle = new GUIStyle(GUI.skin.textField);
        inputStyle.fontSize = 24;
        inputStyle.alignment = TextAnchor.MiddleLeft;


        float inputY = consoleHeight - inputFieldHeight + 5f;
        GUI.SetNextControlName("ConsoleInput");
        input = GUI.TextField(new Rect(padding, inputY, Screen.width - (padding * 2), 40f), input, inputStyle);
        GUI.FocusControl("ConsoleInput");
        
    }

    public void OnReturn()
    {
        if (string.IsNullOrWhiteSpace(input)) return;

        HandleInput();

        if (inputHistory.Count == 0 || inputHistory[inputHistory.Count - 1] != input)
        {
            inputHistory.Add(input);
        }

        historyIndex = -1;
        scrollPosition.y = inputHistory.Count * 25f;
        input = "";
    }

    private void HandleInput()
    {
        string[] properties = input.Split(' ');
        if (properties.Length == 0) return;

        for (int i = 0; i < commandList.Count; i++)
        {
            if (properties[0].ToLower() == commandList[i].CommandID.ToLower())
            {
                if (commandList[i] is ConsoleCommand command)
                {
                    command.Execute();
                }
                else if (commandList[i] is ConsoleCommand<int> intCommand)
                {
                    intCommand.Execute(int.Parse(properties[1]));
                }
                else if (commandList[i] is ConsoleCommand<string> stringCommands)
                { 
                    stringCommands.Execute(properties[1]);
                }
                else if (commandList[i] is ConsoleCommand<string, int> stringCommand)
                {
                    if (properties.Length > 2) stringCommand.Execute(properties[1], int.Parse(properties[2]));
                }
                else if (commandList[i] is ConsoleCommand<int, int, int> int3Command)
                {
                    if (properties.Length > 3)
                    {
                        int.TryParse(properties[1], out int x);
                        int.TryParse(properties[2], out int y);
                        int.TryParse(properties[3], out int z);
                        int3Command.Execute(x, y, z);
                    }
                }
            }
        }
    }
}