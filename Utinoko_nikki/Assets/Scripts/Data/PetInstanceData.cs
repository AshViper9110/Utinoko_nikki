using System;
using UnityEngine;

namespace AquaGarden.Data
{
    [Serializable]
    public sealed class PetInstanceData
    {
        public string instanceId;
        public string petTypeId;
        public string petName;
        public string habitatId;
        [Range(0f, 100f)] public float hunger;
        [Range(0f, 100f)] public float affection;
        public float growth;
        public double ageGameSeconds;
        public long createdUtcTicks;

        public static PetInstanceData Create(PetDefinition definition, string name = null)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (string.IsNullOrWhiteSpace(definition.TypeId))
                throw new ArgumentException("PetDefinition.TypeId must not be empty.", nameof(definition));

            return new PetInstanceData
            {
                instanceId = Guid.NewGuid().ToString("N"),
                petTypeId = definition.TypeId,
                petName = string.IsNullOrWhiteSpace(name) ? definition.DisplayName : name.Trim(),
                habitatId = definition.DefaultHabitat.ToString(),
                hunger = definition.UsesHunger ? definition.InitialHunger : 0f,
                affection = 0f,
                growth = 0f,
                ageGameSeconds = 0d,
                createdUtcTicks = DateTime.UtcNow.Ticks
            };
        }

        public void Normalize()
        {
            if (string.IsNullOrWhiteSpace(instanceId))
                instanceId = Guid.NewGuid().ToString("N");
            if (petTypeId == null) petTypeId = string.Empty;
            if (petName == null) petName = string.Empty;
            if (habitatId == null) habitatId = PetHabitatType.Room.ToString();

            hunger = Mathf.Clamp(hunger, 0f, 100f);
            affection = Mathf.Clamp(affection, 0f, 100f);
            growth = Mathf.Max(0f, growth);
            ageGameSeconds = Math.Max(0d, ageGameSeconds);
        }
    }
}
