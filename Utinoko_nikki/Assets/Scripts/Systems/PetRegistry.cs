using System;
using System.Collections.Generic;
using AquaGarden.Data;
using UnityEngine;

namespace AquaGarden.Systems
{
    public sealed class PetRegistry : MonoBehaviour
    {
        private readonly List<PetInstanceData> pets = new List<PetInstanceData>();
        private readonly Dictionary<string, PetDefinition> definitions =
            new Dictionary<string, PetDefinition>(StringComparer.Ordinal);

        public IReadOnlyList<PetInstanceData> Pets => pets;
        public event Action Changed;

        public void RegisterDefinitions(IEnumerable<PetDefinition> source)
        {
            definitions.Clear();
            if (source == null) return;

            foreach (PetDefinition definition in source)
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.TypeId))
                {
                    Debug.LogWarning("Skipped a null PetDefinition or one with an empty TypeId.", this);
                    continue;
                }

                if (definitions.ContainsKey(definition.TypeId))
                {
                    Debug.LogError("Duplicate PetDefinition TypeId: " + definition.TypeId, this);
                    continue;
                }

                definitions.Add(definition.TypeId, definition);
            }
        }

        public bool TryGetDefinition(string typeId, out PetDefinition definition)
        {
            return definitions.TryGetValue(typeId ?? string.Empty, out definition);
        }

        public PetInstanceData CreatePet(string typeId, string petName = null)
        {
            PetDefinition definition;
            if (!TryGetDefinition(typeId, out definition))
            {
                Debug.LogError("Unknown pet type ID: " + typeId, this);
                return null;
            }

            PetInstanceData pet = PetInstanceData.Create(definition, petName);
            if (!AddExisting(pet)) return null;
            return pet;
        }

        public bool AddExisting(PetInstanceData pet)
        {
            if (pet == null || string.IsNullOrWhiteSpace(pet.petTypeId) ||
                string.IsNullOrWhiteSpace(pet.instanceId))
                return false;

            if (pets.Exists(x => x != null && x.instanceId == pet.instanceId))
            {
                Debug.LogWarning("Duplicate pet instance ID rejected: " + pet.instanceId, this);
                return false;
            }

            pet.Normalize();
            pets.Add(pet);
            Changed?.Invoke();
            return true;
        }

        public bool RemovePet(string instanceId)
        {
            int index = pets.FindIndex(x => x != null && x.instanceId == instanceId);
            if (index < 0) return false;
            pets.RemoveAt(index);
            Changed?.Invoke();
            return true;
        }

        public PetInstanceData FindById(string instanceId)
        {
            return pets.Find(x => x != null && x.instanceId == instanceId);
        }

        public void ReplaceAll(IEnumerable<PetInstanceData> source)
        {
            pets.Clear();
            if (source != null)
            {
                foreach (PetInstanceData pet in source)
                {
                    if (pet == null) continue;
                    pet.Normalize();
                    if (pets.Exists(x => x.instanceId == pet.instanceId))
                    {
                        Debug.LogWarning("Skipped duplicate pet ID while loading: " + pet.instanceId, this);
                        continue;
                    }
                    pets.Add(pet);
                }
            }
            Changed?.Invoke();
        }
    }
}
