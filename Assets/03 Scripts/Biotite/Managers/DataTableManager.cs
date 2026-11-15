using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class DataTableManager : MonoBehaviour
{
    private Dictionary<Type, Dictionary<string, object>> dataDict
        = new Dictionary<Type, Dictionary<string, object>>();

    private ResourceLoader resourceLoader;


    public void Init()
    {
        resourceLoader = new ResourceLoader();
    }

    public async UniTask LoadResource(string key)
    {
        await LoadResource<ScriptableObject>(key);
        await LoadResource<GameObject>(key);
        await LoadResource<AudioClip>(key);

        Logger.Log($"{key}에 대한 로드 완료");
    }

    private async UniTask LoadResource<T>(string key) where T : UnityEngine.Object
    {
        //미리 해당 키의 리소스가 있는지 확인 확인 안하면 무한 루핑으로 빠져나가지 못해서 다음 리소스 로드 불가
        bool isExists = await resourceLoader.IsExists<T>(key);
        if (!isExists)
        {
            Logger.LogWarning($"{key}키의 {typeof(T)} 에 등록된 리소스가 없습니다.");
            return;
        }

        var utcs = new UniTaskCompletionSource();

        resourceLoader.LoadAllAsync<T>(key, (name, loadCount, totalCount, resource) =>
        {
            Type actualType = resource.GetType();
            if (!dataDict.ContainsKey(actualType))
                dataDict.Add(actualType, new Dictionary<string, object>());

            if (typeof(T).IsAssignableFrom(typeof(ScriptableObject)))
            {
                ScriptableObject so = resource as ScriptableObject;
                dataDict[actualType][name] = Instantiate(so);
            }
            else
            {
                dataDict[actualType][name] = resource;
            }

            if (loadCount >= totalCount)
            {
                utcs.TrySetResult();
            }
        });

        await utcs.Task;
    }


    public void UnLoadResource()
    {
        dataDict.Clear();
        resourceLoader.Clear();
    }


    public T GetData<T>(string key) where T : UnityEngine.Object
    {
        bool isScriptableObject = typeof(ScriptableObject).IsAssignableFrom(typeof(T));

        if (dataDict.TryGetValue(typeof(T), out var specificDict))
        {
            if (specificDict.TryGetValue(key, out object obj))
            {
                if (isScriptableObject && obj is ScriptableObject so)
                {
                    return Instantiate(so) as T;
                }
                return obj as T;
            }
        }

        foreach (var pair in dataDict)
        {
            if (typeof(T).IsAssignableFrom(pair.Key))
            {
                if (pair.Value.TryGetValue(key, out object obj))
                {
                    if (typeof(ScriptableObject).IsAssignableFrom(pair.Key) && obj is ScriptableObject so)
                    {
                        return Instantiate(so) as T;
                    }
                    return obj as T;
                }
            }
        }

        return null;
    }


    //타입에 해당하는 아이템 전체 가져오기
    public List<T> GetDatas<T>() where T : UnityEngine.Object
    {
        List<T> result = new List<T>();

        foreach (var pair in dataDict)
        {
            if (typeof(T).IsAssignableFrom(pair.Key))
            {
                foreach (string key in pair.Value.Keys)
                {
                    T data = GetData<T>(key);
                    if (data != null) result.Add(data);
                }
            }
        }

        return result;
    }


    public void Release()
    {
        resourceLoader.Clear();
    }
}
