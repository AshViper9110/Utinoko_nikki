
using System;
using UnityEngine;

namespace AquaGarden.Api
{
    public sealed class EncyclopediaApi : MonoBehaviour
    {
        [SerializeField] private GameApiClient client;

        [SerializeField]
        private string encyclopediaPath = "encyclopedia";

        private void Awake()
        {
            if (client == null)
                client = FindFirstObjectByType<GameApiClient>();
        }

        public void GetEntries(
            Action<bool, string, long> onComplete)
        {
            if (client == null)
            {
                onComplete?.Invoke(
                    false, "GameApiClient Ç™å©Ç¬Ç©ÇËÇ‹ÇπÇÒÅB", 0);
                return;
            }

            client.Get(encyclopediaPath, onComplete);
        }
    }
}
