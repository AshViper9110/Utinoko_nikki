using System;
using System.Collections.Generic;
using AquaGarden.Data;
using UnityEngine;

namespace AquaGarden.Systems
{
    public sealed class PetRegistry : MonoBehaviour
    {
        // 育成できるペットは最大1匹
        private const int MaxPetCount = 1;

        private readonly List<PetInstanceData> pets =
            new List<PetInstanceData>();

        private readonly Dictionary<string, PetDefinition> definitions =
            new Dictionary<string, PetDefinition>(StringComparer.Ordinal);

        public IReadOnlyList<PetInstanceData> Pets => pets;
        public bool HasPet => pets.Count > 0;
        public PetInstanceData CurrentPet => pets.Count > 0 ? pets[0] : null;

        public event Action Changed;

        public void RegisterDefinitions(IEnumerable<PetDefinition> source)
        {
            definitions.Clear();

            if (source == null)
                return;

            foreach (PetDefinition definition in source)
            {
                if (definition == null ||
                    string.IsNullOrWhiteSpace(definition.TypeId))
                {
                    Debug.LogWarning(
                        "Skipped a null PetDefinition or one with an empty TypeId.",
                        this);
                    continue;
                }

                if (definitions.ContainsKey(definition.TypeId))
                {
                    Debug.LogError(
                        "Duplicate PetDefinition TypeId: " + definition.TypeId,
                        this);
                    continue;
                }

                definitions.Add(definition.TypeId, definition);
            }
        }

        public bool TryGetDefinition(
            string typeId,
            out PetDefinition definition)
        {
            return definitions.TryGetValue(
                typeId ?? string.Empty,
                out definition);
        }

        public PetInstanceData CreatePet(
            string typeId,
            string petName = null)
        {
            // すでに育成中なら新しいペットを作らない
            if (HasPet)
            {
                Debug.LogWarning(
                    "Cannot create another pet. Only one pet can be raised at a time.",
                    this);
                return null;
            }

            if (!TryGetDefinition(typeId, out PetDefinition definition))
            {
                Debug.LogError("Unknown pet type ID: " + typeId, this);
                return null;
            }

            PetInstanceData pet = PetInstanceData.Create(definition, petName);

            if (!AddExisting(pet))
                return null;

            return pet;
        }

        public bool AddExisting(PetInstanceData pet)
        {
            if (pet == null ||
                string.IsNullOrWhiteSpace(pet.petTypeId) ||
                string.IsNullOrWhiteSpace(pet.instanceId))
            {
                Debug.LogWarning("Invalid pet data rejected.", this);
                return false;
            }

            if (pets.Exists(x =>
                x != null && x.instanceId == pet.instanceId))
            {
                Debug.LogWarning(
                    "Duplicate pet instance ID rejected: " + pet.instanceId,
                    this);
                return false;
            }

            // 既存のペットを自動で置き換えない
            if (pets.Count >= MaxPetCount)
            {
                Debug.LogWarning(
                    "Cannot add another pet. Only one pet can be raised at a time.",
                    this);
                return false;
            }

            pet.Normalize();
            pets.Add(pet);
            Changed?.Invoke();

            return true;
        }

        public bool RemovePet(string instanceId)
        {
            int index = pets.FindIndex(
                x => x != null && x.instanceId == instanceId);

            if (index < 0)
                return false;

            pets.RemoveAt(index);
            Changed?.Invoke();

            return true;
        }

        public PetInstanceData FindById(string instanceId)
        {
            return pets.Find(
                x => x != null && x.instanceId == instanceId);
        }

        public void ReplaceAll(IEnumerable<PetInstanceData> source)
        {
            pets.Clear();

            if (source != null)
            {
                foreach (PetInstanceData pet in source)
                {
                    if (pet == null)
                        continue;

                    pet.Normalize();

                    if (pets.Exists(x =>
                        x != null && x.instanceId == pet.instanceId))
                    {
                        Debug.LogWarning(
                            "Skipped duplicate pet ID while loading: " +
                            pet.instanceId,
                            this);
                        continue;
                    }

                    // セーブデータに複数匹いても、先頭の1匹だけを採用
                    if (pets.Count >= MaxPetCount)
                    {
                        Debug.LogWarning(
                            "Skipped extra pet while loading. " +
                            "Only one pet can be raised at a time.",
                            this);
                        continue;
                    }

                    pets.Add(pet);
                }
            }

            Changed?.Invoke();
        }

        public void NotifyChanged()
        {
            Changed?.Invoke();
        }
    }
}