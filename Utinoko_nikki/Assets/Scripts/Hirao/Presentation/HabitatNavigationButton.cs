using AquaGarden.Data;
using UnityEngine;
using UnityEngine.UI;

namespace AquaGarden.Presentation
{
    /// <summary>
    /// Connects a UI Button to a habitat ID. Can optionally update its label/icon.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public sealed class HabitatNavigationButton : MonoBehaviour
    {
        [SerializeField] private HabitatViewController viewController;
        [SerializeField] private string habitatId = "Aquarium";
        [SerializeField] private Button button;
        [SerializeField] private Text label;
        [SerializeField] private Image icon;
        [SerializeField] private HabitatDefinition definition;

        private void Awake()
        {
            if (button == null) button = GetComponent<Button>();
            if (definition != null)
            {
                habitatId = definition.HabitatId;
                if (label != null) label.text = definition.DisplayName;
                if (icon != null) icon.sprite = definition.Icon;
            }

            if (button != null)
                button.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(HandleClick);
        }

        public void HandleClick()
        {
            if (viewController == null)
            {
                Debug.LogError("HabitatNavigationButton: ViewController is not assigned.", this);
                return;
            }

            viewController.ShowHabitat(habitatId);
        }
    }
}
