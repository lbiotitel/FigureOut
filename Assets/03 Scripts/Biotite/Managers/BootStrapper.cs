using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Biotite
{

    //테스트를 용이하게 하기 위해 무조건 부트스트랩 부터 로드합니다.
    public static class BootStrapper
    {
      
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]

        private static void Bootstrap()
        {
            string targetActiveSceneName = SceneManager.GetActiveScene().name;
            if (targetActiveSceneName == EScene.BootStrap.ToString()) return;
            InitBootstrapSequence(targetActiveSceneName);
        }

        private static async void InitBootstrapSequence(string targetActiveSceneName)
        {
            Scene currentScene = SceneManager.GetSceneByName(targetActiveSceneName);

            if (currentScene.name == EScene.Title.ToString()) return;

            var op = SceneManager.LoadSceneAsync(EScene.BootStrap.ToString(), LoadSceneMode.Additive);
            await op;

            await Managers.Data.LoadResource(EScene.Game.ToString());
            await Managers.Data.LoadResource(EScene.CharacterSelect.ToString());

            Scene editorScene = SceneManager.GetSceneByName(targetActiveSceneName);

            if (editorScene.IsValid())
            {
                SceneManager.SetActiveScene(editorScene);
                await UniTask.WaitForSeconds(1, ignoreTimeScale: true);
            }
        }

    }
}
