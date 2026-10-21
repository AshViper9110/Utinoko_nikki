
using System;
using UnityEngine;

namespace AquaGarden.Api
{
    public sealed class PetApi : MonoBehaviour
    {
        [SerializeField] private GameApiClient client;

        [SerializeField]
        private string petPath = "pets";

        private void Awake()
        {
            if (client == null)
                client = FindFirstObjectByType<GameApiClient>();
        }

        public void GetPets(
            Action<bool, string, long> onComplete)
        {
            if (client == null)
            {
                onComplete?.Invoke(
                    false, "GameApiClient Ç™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÅB", 0);
                return;
            }

            client.Get(petPath, onComplete);
        }

        public void UpdatePet(
            string updatePath,
            string requestJson,
            Action<bool, string, long> onComplete)
        {
            if (client == null)
            {
                onComplete?.Invoke(
                    false, "GameApiClient Ç™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÅB", 0);
                return;
            }

            client.Put(updatePath, requestJson, onComplete);
        }
    }
}
