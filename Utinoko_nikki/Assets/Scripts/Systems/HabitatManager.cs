using System;
using System.Collections.Generic;
using AquaGarden.Data;
using UnityEngine;

namespace AquaGarden.Systems
{
    public sealed class HabitatManager : MonoBehaviour
    {
        [SerializeField] private List<HabitatDefinition> habitats = new List<HabitatDefinition>();
        [SerializeField] private string startingHabitatId = "Room";

        public string CurrentHabitatId { get; private set; }
        public event Action<string> HabitatChanged;

        private readonly Dictionary<string, HabitatDefinition> byId =
            new Dictionary<string, HabitatDefinition>(StringComparer.Ordinal);

        private void Awake()
        {
            byId.Clear();
            foreach (HabitatDefinition habitat in habitats)
            {
                if (habitat == null || string.IsNullOrWhiteSpace(habitat.HabitatId)) continue;
                if (byId.ContainsKey(habitat.HabitatId))
                {
                    Debug.LogError("Duplicate habitat ID: " + habitat.HabitatId, this);
                    continue;
                }
                byId.Add(habitat.HabitatId, habitat);
            }

            CurrentHabitatId = byId.ContainsKey(startingHabitatId)
                ? startingHabitatId
                : (habitats.Count > 0 && habitats[0] != null
                    ? habitats[0].HabitatId : startingHabitatId);

            Debug.Log(
                $"[HabitatManager] Starting='{startingHabitatId}', Current='{CurrentHabitatId}', Registered={byId.Count}",
                this
            );
        }

        public bool Contains(string habitatId) => byId.ContainsKey(habitatId ?? string.Empty);

        public bool TrySwitch(string habitatId)
        {
            if (!Contains(habitatId)) return false;
            if (CurrentHabitatId == habitatId) return true;
            CurrentHabitatId = habitatId;
            HabitatChanged?.Invoke(CurrentHabitatId);
            return true;
        }

        public bool TryGetDefinition(string habitatId, out HabitatDefinition definition)
        {
            return byId.TryGetValue(habitatId ?? string.Empty, out definition);
        }

        public void RestoreCurrent(string habitatId)
        {
            if (!TrySwitch(habitatId))
                Debug.LogWarning("Unknown saved habitat ID; keeping current habitat: " + habitatId, this);
        }
    }
}
