using AquaGarden.Data;
using AquaGarden.Systems;
using UnityEngine;

namespace AquaGarden
{
    public sealed class CoreSystemBootstrap : MonoBehaviour
    {
        [Header("Catalog")]
        [SerializeField] private PetDefinition[] petDefinitions;

        [Header("Systems")]
        [SerializeField] private PetRegistry petRegistry;
        [SerializeField] private SaveManager saveManager;

        [Header("Initial Pet")]
        [SerializeField] private string initialPetTypeId = "cat";
        [SerializeField] private string initialPetName = "ねこ";

        [SerializeField] private bool loadSaveOnStart = true;

        private void Start()
        {
            if (petRegistry == null)
            {
                Debug.LogError(
                    "CoreSystemBootstrap: PetRegistry is not assigned.",
                    this);
                return;
            }

            petRegistry.RegisterDefinitions(petDefinitions);

            // 先にセーブデータを読み込む
            if (loadSaveOnStart && saveManager != null)
            {
                saveManager.LoadNow();
            }

            // ペットがいなければ初期ペットを生成
            if (!petRegistry.HasPet)
            {
                PetInstanceData pet = petRegistry.CreatePet(
                    initialPetTypeId,
                    initialPetName);

                if (pet == null)
                {
                    Debug.LogWarning(
                        "Aqua Garden: initial pet was not created.",
                        this);
                    return;
                }

                if (saveManager != null)
                {
                    saveManager.SaveNow();
                }
            }

            // ペットの種類に対応した飼育場所へ移動
            PetInstanceData currentPet = petRegistry.CurrentPet;

            if (currentPet != null &&
                petRegistry.TryGetDefinition(
                    currentPet.petTypeId,
                    out PetDefinition definition))
            {
                HabitatManager habitatManager =
                    FindFirstObjectByType<HabitatManager>();

                if (habitatManager != null)
                {
                    string habitatId = definition.DefaultHabitat.ToString();

                    if (habitatManager.TrySwitch(habitatId))
                    {
                        Debug.Log(
                            "Aqua Garden: habitat switched to " + habitatId,
                            this);
                    }
                }
            }
        }
    }
}