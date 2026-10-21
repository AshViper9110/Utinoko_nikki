
using System;
using UnityEngine;

namespace AquaGarden.Api
{
    public sealed class InventoryApi : MonoBehaviour
    {
        [SerializeField]
        private GameApiClient client;

        [Header("Laravel API のパス")]
        [SerializeField]
        private string inventoryPath = "inventory";

        private void Awake()
        {
            if (client == null)
                client = FindFirstObjectByType<GameApiClient>();
        }

        public void GetInventory(
            Action<bool, string, long> onComplete)
        {
            if (client == null)
            {
                onComplete?.Invoke(
                    false, "GameApiClient が見つかりません。", 0);
                return;
            }

            client.Get(inventoryPath, onComplete);
        }

        public void UseItem(
            string itemUsePath,
            string requestJson,
            Action<bool, string, long> onComplete)
        {
            if (client == null)
            {
                onComplete?.Invoke(
                    false, "GameApiClient が見つかりません。", 0);
                return;
            }

            client.Post(itemUsePath, requestJson, onComplete);
        }
    }
}
