using AquaGarden.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace AquaGarden.Presentation
{
    /// <summary>
    /// Optional small UI label that displays the total registered pet count.
    /// Uses the legacy Unity UI Text component to avoid a TextMeshPro dependency.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public sealed class PetCountLabel : MonoBehaviour
    {
        [SerializeField] private PetRegistry petRegistry;
        [SerializeField] private Text targetText;
        [SerializeField] private string format = "ペット：{0}";

        private void Awake()
        {
            if (targetText == null) targetText = GetComponent<Text>();
        }

        private void OnEnable()
        {
            if (petRegistry != null) petRegistry.Changed += Refresh;
            Refresh();
        }

        private void OnDisable()
        {
            if (petRegistry != null) petRegistry.Changed -= Refresh;
        }

        public void Refresh()
        {
            if (targetText == null) return;
            int count = petRegistry != null ? petRegistry.Pets.Count : 0;
            targetText.text = string.Format(format, count);
        }
    }
}
