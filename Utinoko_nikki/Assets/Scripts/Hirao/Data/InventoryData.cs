using System;
using System.Collections.Generic;

namespace AquaGarden.Data
{
    [Serializable]
    public sealed class InventoryData
    {
        public List<InventoryEntry> entries = new List<InventoryEntry>();

        public int GetCount(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId) || entries == null) return 0;
            InventoryEntry entry = entries.Find(x => x != null && x.itemId == itemId);
            return entry == null ? 0 : entry.count;
        }

        public bool Add(string itemId, int amount)
        {
            if (string.IsNullOrWhiteSpace(itemId) || amount <= 0) return false;
            EnsureList();
            InventoryEntry entry = entries.Find(x => x != null && x.itemId == itemId);
            if (entry == null)
            {
                entry = new InventoryEntry { itemId = itemId, count = 0 };
                entries.Add(entry);
            }
            if (entry.count > int.MaxValue - amount) return false;
            entry.count += amount;
            return true;
        }

        public bool TryRemove(string itemId, int amount)
        {
            if (string.IsNullOrWhiteSpace(itemId) || amount <= 0) return false;
            EnsureList();
            InventoryEntry entry = entries.Find(x => x != null && x.itemId == itemId);
            if (entry == null || entry.count < amount) return false;
            entry.count -= amount;
            entries.RemoveAll(x => x == null || x.count <= 0);
            return true;
        }

        public void Normalize()
        {
            EnsureList();
            var merged = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (InventoryEntry entry in entries)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.itemId) || entry.count <= 0) continue;
                int current;
                merged.TryGetValue(entry.itemId, out current);
                long total = (long)current + entry.count;
                merged[entry.itemId] = total > int.MaxValue ? int.MaxValue : (int)total;
            }
            entries.Clear();
            foreach (var pair in merged)
                entries.Add(new InventoryEntry { itemId = pair.Key, count = pair.Value });
        }

        private void EnsureList()
        {
            if (entries == null) entries = new List<InventoryEntry>();
        }
    }

    [Serializable]
    public sealed class InventoryEntry
    {
        public string itemId;
        public int count;
    }
}
