using AquaGarden.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace AquaGarden.Presentation
{
    /// <summary>
    /// Optional UI wiring for explicit Save and Load buttons.
    /// SaveManager also saves on application pause/quit.
    /// </summary>
    public sealed class SaveLoadButtons : MonoBehaviour
    {
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;

        private void Awake()
        {
            if (saveButton != null) saveButton.onClick.AddListener(Save);
            if (loadButton != null) loadButton.onClick.AddListener(Load);
        }

        private void OnDestroy()
        {
            if (saveButton != null) saveButton.onClick.RemoveListener(Save);
            if (loadButton != null) loadButton.onClick.RemoveListener(Load);
        }

        public void Save()
        {
            if (saveManager == null)
            {
                Debug.LogError("SaveLoadButtons: SaveManager is not assigned.", this);
                return;
            }

            if (saveManager.SaveNow())
                Debug.Log("Aqua Garden: save complete.", this);
        }

        public void Load()
        {
            if (saveManager == null)
            {
                Debug.LogError("SaveLoadButtons: SaveManager is not assigned.", this);
                return;
            }

            if (saveManager.LoadNow())
                Debug.Log("Aqua Garden: load complete.", this);
        }
    }
}
