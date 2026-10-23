using System;
using UnityEngine;

namespace AquaGarden.Api
{
    public class InventoryCache : MonoBehaviour
    {
        public static InventoryCache Instance { get; private set; }

        [SerializeField] private GameApiClient apiClient;
        [SerializeField] private bool loadOnStartup = true;

        public ItemData[] Items { get; private set; }
            = Array.Empty<ItemData>();

        public bool IsLoaded { get; private set; }
        public bool IsLoading { get; private set; }

        public event Action OnItemsUpdated;
        public event Action<string> OnLoadFailed;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (loadOnStartup)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            if (IsLoading)
                return;

            if (apiClient == null)
            {
                OnLoadFailed?.Invoke("GameApiClientが設定されていません。");
                return;
            }

            IsLoading = true;

            apiClient.Get("items", (success, body, statusCode) =>
            {
                IsLoading = false;

                if (!success)
                {
                    OnLoadFailed?.Invoke(
                        $"アイテム取得失敗: HTTP {statusCode}");
                    return;
                }

                try
                {
                    Items = ItemJsonParser.Parse(body);
                    IsLoaded = true;

                    OnItemsUpdated?.Invoke();

                    Debug.Log($"アイテムキャッシュ更新: {Items.Length}件");
                }
                catch (Exception e)
                {
                    OnLoadFailed?.Invoke(
                        $"アイテムJSON解析失敗: {e.Message}");
                }
            });
        }

        public ItemData FindById(int id)
        {
            foreach (ItemData item in Items)
            {
                if (item.id == id)
                    return item;
            }

            return null;
        }
    }
}