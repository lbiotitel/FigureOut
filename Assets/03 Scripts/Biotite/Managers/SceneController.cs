using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//빌더 패턴을 활용한 씬 관리자 입니다.
public class SceneController : MonoBehaviour,IManager
{
    [SerializeField] private LoadingUI loadingUI;

    private List<string> loadedScenes = new List<string>();
    private bool isLoading = false;


    public void Init()
    {

    }


    public SceneTransitionPlan NewTransition()
    {
        return new SceneTransitionPlan();
    }


    private async UniTask ExcutePlan(SceneTransitionPlan plan)
    {
        if (isLoading) return;
        isLoading = true;
        await ChangeSceneSequence(plan);
    }


    //차례대로 등록된 실행순서에 따라 실행
    private async UniTask ChangeSceneSequence(SceneTransitionPlan plan)
    {
        if (plan.Overlay)
        {
            await loadingUI.FadeIn();
            await UniTask.WaitForSeconds(0.5f);
        }

        foreach (string sceneName in plan.ScenesToUnload)
        {
            await UnloadScene(sceneName);
        }

        //리소스들 언로드
        if(plan.ResourcesToUnload) Managers.Data.UnLoadResource();

        //리소스들 로드
        foreach (string key in plan.ResourcesToLoad)
        {
            await Managers.Data.LoadResource(key);
        }

        foreach (var kvp in plan.ScenesToLoad)
        {
            if (loadedScenes.Contains(kvp))
            {
                await UnloadScene(kvp);
            }
            await LoadScene(kvp, plan.ActiveSceneName == kvp);
        }

        if (plan.Overlay)
        {
            await loadingUI.FadeOut();            
        }

        isLoading = false;
    }

    //씬로드
    private async UniTask LoadScene(string sceneName, bool setActive)
    {
        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if (op == null) return;

        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            await UniTask.Yield();
        }

        op.allowSceneActivation = true;

        while (op.isDone)
        {
            await UniTask.Yield();
        }

        if (setActive)
        {
            Scene newScene = SceneManager.GetSceneByName(sceneName);
            if (newScene.IsValid() && newScene.isLoaded)
            {
                SceneManager.SetActiveScene(newScene);
            }
        }
        loadedScenes.Add(sceneName);
    }

    //씬 언로드
    private async UniTask UnloadScene(string sceneName)
    {
        if (!loadedScenes.Contains(sceneName)) return;
        if (string.IsNullOrEmpty(sceneName)) return;

        var op = SceneManager.UnloadSceneAsync(sceneName);
        if (op == null) return;

        while (op.isDone)
        {
            await UniTask.Yield();
        }

        loadedScenes.Remove(sceneName);
    }


    public void Release()
    {
        
    }



    //빌더 패턴을 위한 클래스
    public class SceneTransitionPlan
    {
        //어떤 씬을 로드 할 건지
        public List<string> ScenesToLoad { get; } = new List<string>();

        //어떤 씬을 언로드 할 건지
        public List<string> ScenesToUnload { get; } = new List<string>();
        
        //활성화 된 씬이름
        public string ActiveSceneName { get; private set; }

        //로드할 리소스 키들
        public List<string> ResourcesToLoad { get; } = new List<string>();

        //언로드할 리소스 키들
        public bool ResourcesToUnload { get; private set; } = false;

        //UI를 켜서 부드럽게 전환 유무
        public bool Overlay { get; private set; } = false;

        public SceneTransitionPlan Load(string sceneName, bool setActive = false)
        {
            ScenesToLoad.Add(sceneName);
            if (setActive) ActiveSceneName = sceneName;
            return this;
        }

        public SceneTransitionPlan Unload(string sceneName)
        { 
            ScenesToUnload.Add(sceneName);
            return this;
        }

        public SceneTransitionPlan WithOverlay()
        {
            Overlay = true;
            return this;
        }

        public SceneTransitionPlan LoadResources(string key)
        {
            ResourcesToLoad.Add(key);
            return this;
        }

        public SceneTransitionPlan UnLoadResources()
        {
            ResourcesToUnload = true;
            return this;
        }

        public async void Perform()
        {
            await Managers.Scene.ExcutePlan(this);
        }
    }

}
