using AquaGarden.Data;
using AquaGarden.Systems;
using TMPro;
using UnityEngine;

namespace AquaGarden.Presentation
{
    /// <summary>
    /// Displays the current pet's details in the Pet Info panel.
    /// Attach this component to the Pet Info panel or one of its active children.
    /// </summary>
    public sealed class PetInfoPanelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PetRegistry petRegistry;
        [SerializeField] private PetCareSystem petCareSystem;

        [Header("UI Labels (TextMeshPro)")]
        [SerializeField] private TMP_Text petNameLabel;
        [SerializeField] private TMP_Text petTypeLabel;
        [SerializeField] private TMP_Text instanceIdLabel;
        [SerializeField] private TMP_Text hungerLabel;
        [SerializeField] private TMP_Text affectionLabel;
        [SerializeField] private TMP_Text growthLabel;
        [SerializeField] private TMP_Text ageLabel;

        private void Awake()
        {
            ResolveReferences();
        }

        private void OnEnable()
        {
            ResolveReferences();

            if (petRegistry != null)
                petRegistry.Changed += Refresh;

            if (petCareSystem != null)
                petCareSystem.StateChanged += HandlePetStateChanged;

            Refresh();
        }

        private void OnDisable()
        {
            if (petRegistry != null)
                petRegistry.Changed -= Refresh;

            if (petCareSystem != null)
                petCareSystem.StateChanged -= HandlePetStateChanged;
        }

        private void ResolveReferences()
        {
            if (petRegistry == null)
                petRegistry = FindFirstObjectByType<PetRegistry>();

            if (petCareSystem == null)
                petCareSystem = FindFirstObjectByType<PetCareSystem>();
        }

        private void HandlePetStateChanged(PetInstanceData pet)
        {
            Refresh();
        }

        public void Refresh()
        {
            PetInstanceData pet = petRegistry != null
                ? petRegistry.CurrentPet
                : null;

            if (pet == null)
            {
                SetText(petNameLabel, "ペット：未登録");
                SetText(petTypeLabel, "種類：—");
                SetText(instanceIdLabel, "個体ID：—");
                SetText(hungerLabel, "満腹度：—");
                SetText(affectionLabel, "愛情度：—");
                SetText(growthLabel, "成長：—");
                SetText(ageLabel, "飼育時間：—");
                return;
            }

            SetText(
                petNameLabel,
                "名前：" + (string.IsNullOrWhiteSpace(pet.petName)
                    ? "名無し"
                    : pet.petName));

            SetText(petTypeLabel, "種類：" + pet.petTypeId);
            SetText(instanceIdLabel, "個体ID：" + pet.instanceId);
            SetText(hungerLabel, $"満腹度：{Mathf.Clamp(pet.hunger, 0f, 100f):0}/100");
            SetText(affectionLabel, $"愛情度：{Mathf.Clamp(pet.affection, 0f, 100f):0}/100");
            SetText(growthLabel, $"成長値：{Mathf.Max(0f, pet.growth):0.0}");
            SetText(ageLabel, "飼育時間：" + FormatDuration(pet.ageGameSeconds));
        }

        private static void SetText(TMP_Text label, string value)
        {
            if (label != null)
                label.text = value;
        }

        private static string FormatDuration(double seconds)
        {
            seconds = System.Math.Max(0d, seconds);

            int totalSeconds = (int)seconds;
            int days = totalSeconds / 86400;
            int hours = (totalSeconds % 86400) / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int remainingSeconds = totalSeconds % 60;

            if (days > 0)
                return $"{days}日 {hours:00}:{minutes:00}:{remainingSeconds:00}";

            return $"{hours:00}:{minutes:00}:{remainingSeconds:00}";
        }
    }
}
