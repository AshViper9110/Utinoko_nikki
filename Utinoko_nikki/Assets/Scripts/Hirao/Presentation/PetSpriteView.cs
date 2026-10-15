using AquaGarden.Data;
using AquaGarden.Systems;
using UnityEngine;

namespace AquaGarden.Presentation
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PetSpriteView : MonoBehaviour
    {
        [SerializeField] private PetRegistry petRegistry;
        [SerializeField] private HabitatManager habitatManager;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            if (petRegistry != null)
                petRegistry.Changed += Refresh;

            if (habitatManager != null)
                habitatManager.HabitatChanged += OnHabitatChanged;

            Refresh();
        }

        private void OnDisable()
        {
            if (petRegistry != null)
                petRegistry.Changed -= Refresh;

            if (habitatManager != null)
                habitatManager.HabitatChanged -= OnHabitatChanged;
        }

        public void Refresh()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            PetInstanceData pet =
                petRegistry != null ? petRegistry.CurrentPet : null;

            if (pet == null || petRegistry == null)
            {
                spriteRenderer.enabled = false;
                return;
            }

            if (!petRegistry.TryGetDefinition(
                pet.petTypeId, out PetDefinition definition))
            {
                spriteRenderer.enabled = false;
                Debug.LogWarning(
                    "PetSpriteView: PetDefinition not found: " +
                    pet.petTypeId, this);
                return;
            }

            string petHabitat = definition.DefaultHabitat.ToString();
            string activeHabitat =
                habitatManager != null
                    ? habitatManager.CurrentHabitatId
                    : petHabitat;

            bool shouldShow = petHabitat == activeHabitat;

            spriteRenderer.sprite = definition.Icon;
            spriteRenderer.enabled =
                shouldShow && definition.Icon != null;
        }

        private void OnHabitatChanged(string ignored)
        {
            Refresh();
        }
    }
}