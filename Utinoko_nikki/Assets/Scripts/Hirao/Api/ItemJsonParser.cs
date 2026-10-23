using NUnit.Framework.Interfaces;
using UnityEngine;

namespace AquaGarden.Api
{
    public static class ItemJsonParser
    {
        public static ItemData[] Parse(string json)
        {
            string wrappedJson = "{\"items\":" + json + "}";

            ItemListResponse response =
                JsonUtility.FromJson<ItemListResponse>(wrappedJson);

            return response?.items ?? new ItemData[0];
        }
    }
}