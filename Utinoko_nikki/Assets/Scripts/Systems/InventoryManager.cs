using AquaGarden.Data;
using UnityEngine;

namespace AquaGarden.Systems
{
    public sealed class InventoryManager : MonoBehaviour
    {
        [SerializeField] private InventoryData data = new InventoryData();
        public InventoryData Data => data;

        public int GetCount(string itemId) => data.GetCount(itemId);

        public bool Add(string itemId, int amount)
        {
            bool result = data.Add(itemId, amount);
            if (result) data.Normalize();
            return result;
        }

        public bool TryConsume(string itemId, int amount)
        {
            return data.TryRemove(itemId, amount);
        }

        public void Restore(InventoryData saved)
        {
            data = saved ?? new InventoryData();
            data.Normalize();
        }
    }
}
