using AquaGarden.Data;
using AquaGarden.Systems;
using UnityEngine;

namespace AquaGarden.Tests
{
    // Optional in-scene smoke test. Add to a GameObject and assign PetRegistry.
    public sealed class CoreSystemSmokeTest : MonoBehaviour
    {
        [SerializeField] private PetRegistry registry;
        [SerializeField] private PetDefinition testDefinition;

        [ContextMenu("Run Core Smoke Test")]
        public void Run()
        {
            if (registry == null || testDefinition == null)
            {
                Debug.LogError("Assign PetRegistry and PetDefinition before running the test.", this);
                return;
            }

            int before = registry.Pets.Count;
            PetInstanceData first = registry.CreatePet(testDefinition.TypeId, "Test A");
            PetInstanceData second = registry.CreatePet(testDefinition.TypeId, "Test B");

            bool uniqueIds = first != null && second != null &&
                             first.instanceId != second.instanceId;
            bool added = registry.Pets.Count == before + 2;
            bool removed = second != null && registry.RemovePet(second.instanceId);
            bool remaining = first != null && registry.FindById(first.instanceId) == first;

            Debug.Log($"Core smoke test: unique IDs={uniqueIds}, added={added}, remove={removed}, lookup={remaining}", this);
            if (!(uniqueIds && added && removed && remaining))
                Debug.LogError("Core smoke test FAILED.", this);
        }
    }
}
