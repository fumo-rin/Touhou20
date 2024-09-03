using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityAddressables = UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core
{
    public static class AddressablesTools
    {
        #region Keys
        public static string KeyAny => "Any";
        public static string KeyItem => "Items";
        public static string KeyNameGenerationSettings => "Name Generation Settings";
        #endregion
        #region Async
        public static async Task<IList<T>> LoadAllAsync<T>()
        {
            return await LoadKeysAsync<T>(KeyAny);
        }
        public static async Task<IList<T>> LoadKeyListAsync<T>(List<string> keys)
        {
            IList<T> loadRequest = await UnityAddressables.Addressables.LoadAssetsAsync<T>(keys, null, UnityEngine.AddressableAssets.Addressables.MergeMode.Union).Task;
            return loadRequest;
        }
        #region Ugly key list
        public static async Task<IList<T>> LoadKeysAsync<T>(string key) => await LoadKeyListAsync<T>(new List<string> { key });
        public static async Task<IList<T>> LoadKeysAsync<T>(string key1, string key2) => await LoadKeyListAsync<T>(new List<string> { key1, key2 });
        public static async Task<IList<T>> LoadKeysAsync<T>(string key1, string key2, string key3) => await LoadKeyListAsync<T>(new List<string> { key1, key2, key3 });
        public static async Task<IList<T>> LoadKeysAsync<T>(string key1, string key2, string key3, string key4) => await LoadKeyListAsync<T>(new List<string> { key1, key2, key3, key4 });
        public static async Task<IList<T>> LoadKeysAsync<T>(string key1, string key2, string key3, string key4, string key5) => await LoadKeyListAsync<T>(new List<string> { key1, key2, key3, key4, key5 });
        public static async Task<IList<T>> LoadKeysAsync<T>(string key1, string key2, string key3, string key4, string key5, string key6) => await LoadKeyListAsync<T>(new List<string> { key1, key2, key3, key4, key5, key6 });
        public static async Task<IList<T>> LoadKeysAsync<T>(string key1, string key2, string key3, string key4, string key5, string key6, string key7) => await LoadKeyListAsync<T>(new List<string> { key1, key2, key3, key4, key5, key6, key7 });
        public static async Task<IList<T>> LoadKeysAsync<T>(string key1, string key2, string key3, string key4, string key5, string key6, string key7, string key8) => await LoadKeyListAsync<T>(new List<string> { key1, key2, key3, key4, key5, key6, key7, key8 });
        #endregion
        #endregion
        #region Synchronous

        public static IList<T> LoadAll<T>()
        {
            return LoadKeys<T>(KeyAny);
        }
        public static IList<T> LoadKeyList<T>(List<string> keys)
        {
            AsyncOperationHandle<IList<T>> loadRequest;
            loadRequest = UnityEngine.AddressableAssets.Addressables.LoadAssetsAsync<T>(keys, null, UnityEngine.AddressableAssets.Addressables.MergeMode.Union);

            loadRequest.WaitForCompletion();

            if (loadRequest.Status == AsyncOperationStatus.Succeeded)
            {
                return loadRequest.Result;
            }
            return new List<T>();
        }
        #region Ugly key list
        public static IList<T> LoadKeys<T>(string key) => LoadKeyList<T>(new List<string> { key });
        public static IList<T> LoadKeys<T>(string key1, string key2) => LoadKeyList<T>(new List<string> { key1, key2 });
        public static IList<T> LoadKeys<T>(string key1, string key2, string key3) => LoadKeyList<T>(new List<string> { key1, key2, key3 });
        public static IList<T> LoadKeys<T>(string key1, string key2, string key3, string key4) => LoadKeyList<T>(new List<string> { key1, key2, key3, key4 });
        public static IList<T> LoadKeys<T>(string key1, string key2, string key3, string key4, string key5) => LoadKeyList<T>(new List<string> { key1, key2, key3, key4, key5 });
        public static IList<T> LoadKeys<T>(string key1, string key2, string key3, string key4, string key5, string key6) => LoadKeyList<T>(new List<string> { key1, key2, key3, key4, key5, key6 });
        public static IList<T> LoadKeys<T>(string key1, string key2, string key3, string key4, string key5, string key6, string key7) => LoadKeyList<T>(new List<string> { key1, key2, key3, key4, key5, key6, key7 });
        public static IList<T> LoadKeys<T>(string key1, string key2, string key3, string key4, string key5, string key6, string key7, string key8) => LoadKeyList<T>(new List<string> { key1, key2, key3, key4, key5, key6, key7, key8 });
        #endregion
        #endregion
    }
}
