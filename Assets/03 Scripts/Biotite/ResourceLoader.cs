using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceLoader
{
    private Dictionary<string, UnityEngine.Object> _resources = new Dictionary<string, UnityEngine.Object>();
    private Dictionary<string, AsyncOperationHandle> _handles = new Dictionary<string, AsyncOperationHandle>();

    #region Load Resource

    public T Get<T>(string key) where T : UnityEngine.Object
    {
        if (_resources.TryGetValue(key, out UnityEngine.Object resource))
            return resource as T;

        return null;
    }

    #endregion


    #region Addresable


    void LoadAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
    {
        if (_resources.TryGetValue(key, out UnityEngine.Object resource))
        {
            callback?.Invoke(resource as T);
            return;
        }

        string loadKey = key;
        if (key.Contains(".sprite"))
            loadKey = $"{key}[{key.Replace(".sprite", "")}]";

        var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);

        asyncOperation.Completed += (op) =>
        {
            _resources.Add(key, op.Result);
            _handles.Add(key, asyncOperation);
            callback?.Invoke(asyncOperation.Result);
        };
    }


    //해당 키에 해당하고 타입은 T인 데이터를 전부 비동기 로드 합니다.
    public void LoadAllAsync<T>(string label, Action<string, int, int,object> callback) where T : UnityEngine.Object
    {
        var opHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        opHandle.Completed += (op) =>
        {
            int loadCount = 0;
            int totalCount = op.Result.Count;

            foreach (var result in op.Result)
            {
                if (result.PrimaryKey.Contains(".sprite"))
                {
                    LoadAsync<Sprite>(result.PrimaryKey, (obj) =>
                    {
                        loadCount++;
                        callback?.Invoke(result.PrimaryKey, loadCount, totalCount, obj);
                    });
                }
                else
                {
                    LoadAsync<T>(result.PrimaryKey, (obj) =>
                    {
                        loadCount++;
                        callback?.Invoke(result.PrimaryKey, loadCount, totalCount, obj);
                    });
                }
            }
        };
    }


    //해당 키의 리소스가 있는지 확인합니다.
    public async UniTask<bool> IsExists<T>(string key)
    {
        var opHandle = Addressables.LoadResourceLocationsAsync(key, typeof(T));
        var locations = await opHandle;

        bool exists = locations != null && locations.Count > 0;
        Addressables.Release(opHandle);

        return exists;
    }


    public void Clear()
    {
        _resources.Clear();

        foreach (var handle in _handles)
        {
            if (handle.Value.IsValid())
            {
                Addressables.Release(handle.Value);
            }
        }
        _handles.Clear();
    }

    #endregion
}