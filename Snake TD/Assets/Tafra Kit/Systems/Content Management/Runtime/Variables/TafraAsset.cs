using System;
using System.Threading.Tasks;
using UnityEngine;
#if TAFRA_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace TafraKit.ContentManagement
{
    [System.Serializable]
    public class TafraAsset<T> where T : UnityEngine.Object
    {
        [SerializeField] private AssetType assetType;
        [SerializeField] private T directAsset;
        #if TAFRA_ADDRESSABLES
        [SerializeField] private AssetReferenceT<T> addressablesAsset;
        #endif

        [NonSerialized] private bool isLoaded;
        #if TAFRA_ADDRESSABLES
        [NonSerialized] private AsyncOperationHandle<T> instantLoadOperationHandle;
        #endif

        public AssetType AssetType => assetType;
        public bool IsDirectAsset
        {
            get
            {
                if(assetType == AssetType.Direct)
                    return true;

                return false;
            }
        }
        public bool IsLoaded
        {
            get
            {
                if(assetType == AssetType.Direct)
                    return true;

                return isLoaded;
            }
        }
        public T DirectAsset => directAsset;
        public T LoadedAsset 
        {
            get
            {
                if (assetType == AssetType.Direct)
                    return directAsset;


                if (!isLoaded)
                    return null;

                #if TAFRA_ADDRESSABLES
                if (assetType== AssetType.Addressables) 
                    return addressablesAsset.OperationHandle.Result as T;
                #endif

                return null;
            }
        }
        /// <summary>
        /// Returns true if there's a referenced asset that can be loaded.
        /// </summary>
        public bool HasReference 
        {
            get
            {
                switch(assetType)
                {
                    case AssetType.Direct:
                        return directAsset != null;
                    #if TAFRA_ADDRESSABLES
                    case AssetType.Addressables:
                        return !string.IsNullOrEmpty(addressablesAsset.AssetGUID);
                    #endif
                    default:
                        return true;
                }
            }
        }

        //public async Task<T> Load()
        //{
        //    switch(assetType)
        //    {
        //        case AssetType.Direct:
        //            return directAsset;
        //        case AssetType.Addressables:
        //            {
        //                var handler = addressablesAsset.LoadAssetAsync();

        //                await handler.Task;

        //                if (handler.Status == AsyncOperationStatus.Succeeded)
        //                    return handler.Result;

        //                return null;
        //            }
        //        default:
        //            return null;
        //    }
        //}
        public void LoadAsync(Action<T> onLoad, Action onFail)
        {
            switch(assetType)
            {
                case AssetType.Direct:
                    {
                        onLoad?.Invoke(directAsset);
                        break;
                    }
                case AssetType.Addressables:
                    {
#if TAFRA_ADDRESSABLES
                        if(string.IsNullOrEmpty(addressablesAsset.AssetGUID))
                        {
                            TafraDebugger.Log("Tafra Asset", "Asset reference is null.", TafraDebugger.LogType.Error);
                            onFail?.Invoke();
                            return;
                        }

                        var existingOperationHandle = addressablesAsset.OperationHandle;
                        //If the asset reference already loaded the asset before, then return it.
                        if(existingOperationHandle.IsValid() && existingOperationHandle.Result != null)
                        {
                            onLoad?.Invoke(existingOperationHandle.Result as T);
                        }
                        else if(instantLoadOperationHandle.IsValid() && instantLoadOperationHandle.Result != null)
                        {
                            onLoad?.Invoke(instantLoadOperationHandle.Result);
                        }
                        else
                        {
                            addressablesAsset.LoadAssetAsync().Completed += (handler) =>
                            {
                                if(handler.Status == AsyncOperationStatus.Succeeded)
                                {
                                    onLoad?.Invoke(handler.Result);
                                    isLoaded = true;
                                }
                                else
                                    onFail?.Invoke();
                            };
                        }
#endif
                        break;
                    }
                default:
                    onFail?.Invoke();
                    break;
            }
        }
        public T Load()
        {
            switch(assetType)
            {
                case AssetType.Direct:
                    return directAsset;
                case AssetType.Addressables:
                    {
#if TAFRA_ADDRESSABLES
                        if(string.IsNullOrEmpty(addressablesAsset.AssetGUID))
                        {
                            TafraDebugger.Log("Tafra Asset", "Asset reference is null.", TafraDebugger.LogType.Error);
                            return null;
                        }

                        var existingOperationHandle = addressablesAsset.OperationHandle;
                        //If the asset reference already loaded the asset before, then return it.
                        if(existingOperationHandle.IsValid() && existingOperationHandle.Result != null)
                        {
                            return existingOperationHandle.Result as T;
                        }
                        else if(instantLoadOperationHandle.IsValid() && instantLoadOperationHandle.Result != null)
                        {
                            return instantLoadOperationHandle.Result;
                        }
                        else
                        {
                            instantLoadOperationHandle = Addressables.LoadAssetAsync<T>(addressablesAsset);

                            instantLoadOperationHandle.WaitForCompletion();

                            if(instantLoadOperationHandle.Status == AsyncOperationStatus.Succeeded)
                                return instantLoadOperationHandle.Result;
                        }
#endif
                        break;
                    }
                default:
                    return null;
            }

            return null;
        }
        public void Release()
        {
            if(assetType == AssetType.Addressables)
            {
                #if TAFRA_ADDRESSABLES
                addressablesAsset.ReleaseAsset();

                if(instantLoadOperationHandle.IsValid())
                {
                    instantLoadOperationHandle.Release();
                }
                
                isLoaded = false;
                #endif
            }
        }
    }
}