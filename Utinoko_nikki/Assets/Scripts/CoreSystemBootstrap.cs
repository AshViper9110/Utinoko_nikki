using AquaGarden.Data;
using AquaGarden.Systems;
using UnityEngine;

namespace AquaGarden
{
    /// <summary>
    /// Wires the common systems together. Add exactly one instance to the Main scene.
    /// </summary>
    public sealed class CoreSystemBootstrap : MonoBehaviour
    {
        [Header("Catalog")]
        [SerializeField] private PetDefinition[] petDefinitions;

        [Header("Systems")]
        [SerializeField] private PetRegistry petRegistry;
        [SerializeField] private SaveManager saveManager;

        [SerializeField] private bool loadSaveOnStart = true;

        private void Start()
        {
            if (petRegistry == null)
            {
                Debug.LogError("CoreSystemBootstrap: PetRegistry is not assigned.", this);
                return;
            }

            petRegistry.RegisterDefinitions(petDefinitions);

            //if (loadSaveOnStart && saveManager != null) saveManager.LoadNow();
        }
    }
}
