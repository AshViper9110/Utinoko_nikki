
using AquaGarden.Data;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AquaGarden.Presentation
{
    public class PetStatusGauges : MonoBehaviour
    {
        [Header("ゲージ")]
        [SerializeField] private Slider hungerSlider;
        [SerializeField] private Slider affectionSlider;

        [Header("数値表示（任意）")]
        [SerializeField] private TMP_Text hungerLabel;
        [SerializeField] private TMP_Text affectionLabel;

        [Header("ペット名")]
        [SerializeField] private TMP_Text petNameLabel;

        [Header("参照")]
        [SerializeField] private PetCareSystem petCareSystem;

        private void Awake()
        {
            if (petCareSystem == null)
                petCareSystem =
                    FindFirstObjectByType<PetCareSystem>();

            SetupSlider(hungerSlider);
            SetupSlider(affectionSlider);
        }

        private void OnEnable()
        {
            if (petCareSystem != null)
                petCareSystem.StateChanged += RefreshGauges;

            RefreshGauges(
                petCareSystem != null ? petCareSystem.CurrentPet : null);
        }

        private void OnDisable()
        {
            if (petCareSystem != null)
                petCareSystem.StateChanged -= RefreshGauges;
        }

        private void Update()
        {
            // 初期化順序の違いで Awake 時に取得できなかった場合に対応
            if (petCareSystem == null)
            {
                petCareSystem =
                    FindFirstObjectByType<PetCareSystem>();

                if (petCareSystem != null)
                {
                    petCareSystem.StateChanged += RefreshGauges;
                    RefreshGauges(petCareSystem.CurrentPet);
                }
            }
        }

        private void SetupSlider(Slider slider)
        {
            if (slider == null)
                return;

            slider.minValue = 0f;
            slider.maxValue = 100f;
            slider.interactable = false;
        }

        private void RefreshGauges(PetInstanceData pet)
        {
            if (pet == null)
            {
                if (petNameLabel != null)
                    petNameLabel.text = "ペット未登録";

                SetGauge(hungerSlider, hungerLabel, 0f, "満腹度");
                SetGauge(affectionSlider, affectionLabel, 0f, "なかよし度");
                return;
            }

            if (petNameLabel != null)
            {
                petNameLabel.text = string.IsNullOrWhiteSpace(pet.petName)
                    ? "名前なし"
                    : pet.petName;
            }

            SetGauge(hungerSlider, hungerLabel, pet.hunger, "満腹度");
            SetGauge(affectionSlider, affectionLabel, pet.affection, "なかよし度");
        }

        private void SetGauge(
            Slider slider,
            TMP_Text label,
            float value,
            string displayName)
        {
            value = Mathf.Clamp(value, 0f, 100f);

            if (slider != null)
                slider.SetValueWithoutNotify(value);

            if (label != null)
                label.text = $"{displayName}  {value:0}/{100}";
        }
    }
}
