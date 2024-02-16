using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = System.Object;

public class AssetProvider : IService
{
    private readonly Dictionary<string, object> _loadedAssets = new ();

    public async Task<T> LoadAssetAsync<T>(string address)
    {
        if (_loadedAssets.TryGetValue(address, out var loaded))
        {
            return (T)loaded;
        }

        var operationHandle = Addressables.LoadAssetAsync<T>(address);
        await operationHandle.Task;
        
        if (operationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            T result = operationHandle.Result;
            _loadedAssets[address] = result;
            return result;
        }
        else
        {
            Debug.LogError($"Failed to load asset at address: {address}");
            throw new NullReferenceException($"Failed to load asset at address: {address}");
        }
    }
    public T LoadAsset<T>(string address)
    {
        if (_loadedAssets.TryGetValue(address, out var loaded))
        {
            return (T)loaded;
        }

        var operationHandle = Addressables.LoadAssetAsync<T>(address);
        operationHandle.Task.Wait();

        if (operationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            T result = operationHandle.Result;
            _loadedAssets[address] = result;
            return result;
        }
        else
        {
            Debug.LogError($"Failed to load asset at address: {address}");
            throw new NullReferenceException($"Failed to load asset at address: {address}");
        }
    }

    public void ReleaseAsset(string address)
    {
        if (_loadedAssets.TryGetValue(address, out var loaded))
        {
            Addressables.Release(loaded);
            _loadedAssets.Remove(address);
        }
    }

    public void ReleaseAllAssets()
    {
        foreach (var loaded in _loadedAssets.Values)
        {
            Addressables.Release(loaded);
        }
        _loadedAssets.Clear();
    }
}