using UnityEngine;
using UnityEngine.SceneManagement;

public class Managers : MonoBehaviour
{
    private static GameManager gameManager;
    private static DataTableManager dataTableManager;
    private static ObjectManager objectManager;
    private static UIManager uiManager;
    private static EventManager eventManager;
    private static InputManager inputManager;
    private static AudioManager audioManager;
    private static SceneController sceneController;
    private static QuestManager questManager;

    public static GameManager Game => gameManager;
    public static DataTableManager Data => dataTableManager;
    public static ObjectManager Object => objectManager;
    public static UIManager UI => uiManager;
    public static EventManager Event => eventManager;
    public static InputManager Input => inputManager;
    public static AudioManager Audio => audioManager;
    public static SceneController Scene => sceneController;
    public static QuestManager Quest => questManager;


    public bool IsInitialized { get; private set; } = false;

    protected void Awake()
    {
        InitManagers();
    }

    private void InitManagers()
    {
        eventManager = new EventManager();
        dataTableManager = GetComponentInChildren<DataTableManager>();
        objectManager = GetComponentInChildren<ObjectManager>();
        uiManager = GetComponentInChildren<UIManager>();
        inputManager = GetComponentInChildren<InputManager>();
        audioManager = GetComponentInChildren<AudioManager>();
        sceneController = GetComponentInChildren<SceneController>();
        gameManager = GetComponentInChildren<GameManager>();
        questManager = GetComponentInChildren<QuestManager>();

        eventManager.Init();
        dataTableManager.Init();
        objectManager.Init();
        uiManager.Init();
        audioManager.Init();
        sceneController.Init();
        gameManager.Init();
        questManager.Init();

        IsInitialized = true;
        GoNextScene();
    }

    private void GoNextScene()
    {
        if (SceneManager.GetActiveScene().name == EScene.BootStrap.ToString())
        {
           Scene
           .NewTransition()
           .Load(EScene.CharacterSelect.ToString())
           .LoadResources(EScene.CharacterSelect.ToString())
           .WithOverlay()
           .Perform();
        }
    }


}