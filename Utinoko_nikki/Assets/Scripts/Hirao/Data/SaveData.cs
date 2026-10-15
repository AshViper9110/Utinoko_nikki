using System;
using System.Collections.Generic;

namespace AquaGarden.Data
{
    [Serializable]
    public sealed class SaveData
    {
        public int schemaVersion = CurrentSchemaVersion;
        public string currentHabitatId = "Room";
        public List<PetInstanceData> pets = new List<PetInstanceData>();
        public InventoryData inventory = new InventoryData();
        public GameSettingsData settings = new GameSettingsData();
        public double totalGameSeconds;

        public const int CurrentSchemaVersion = 1;

        public void Normalize()
        {
            if (string.IsNullOrWhiteSpace(currentHabitatId)) currentHabitatId = "Room";
            if (pets == null) pets = new List<PetInstanceData>();
            if (inventory == null) inventory = new InventoryData();
            if (settings == null) settings = new GameSettingsData();

            var ids = new HashSet<string>(StringComparer.Ordinal);
            for (int i = pets.Count - 1; i >= 0; i--)
            {
                PetInstanceData pet = pets[i];
                if (pet == null || string.IsNullOrWhiteSpace(pet.petTypeId))
                {
                    pets.RemoveAt(i);
                    continue;
                }

                pet.Normalize();
                if (!ids.Add(pet.instanceId))
                {
                    // Repair a duplicate ID instead of silently losing a pet.
                    pet.instanceId = Guid.NewGuid().ToString("N");
                    ids.Add(pet.instanceId);
                }
            }

            inventory.Normalize();
            settings.Normalize();
            totalGameSeconds = Math.Max(0d, totalGameSeconds);
        }
    }

    [Serializable]
    public sealed class GameSettingsData
    {
        public float masterVolume = 1f;
        public bool showNotifications = true;

        public void Normalize()
        {
            masterVolume = UnityEngine.Mathf.Clamp01(masterVolume);
        }
    }
}
