using System;
using System.Collections.Generic;
using AquaGarden.Systems;
using UnityEngine;

namespace AquaGarden.Presentation
{
    /// <summary>
    /// Shows exactly one habitat root at a time and follows HabitatManager changes.
    /// IDs must match the HabitatDefinition assets registered in HabitatManager.
    /// </summary>
    public sealed class HabitatViewController : MonoBehaviour
    {
        [Serializable]
        private sealed class HabitatRoot
        {
            public string habitatId;
            public GameObject root;
        }

        [SerializeField] private HabitatManager habitatManager;
        [SerializeField] private List<HabitatRoot> habitatRoots = new List<HabitatRoot>();
        [SerializeField] private bool hideUnregisteredRoots = true;

        private void OnEnable()
        {
            if (habitatManager != null)
                habitatManager.HabitatChanged += OnHabitatChanged;

            Refresh();
        }

        private void OnDisable()
        {
            if (habitatManager != null)
                habitatManager.HabitatChanged -= OnHabitatChanged;
        }

        public void ShowHabitat(string habitatId)
        {
            if (habitatManager == null)
            {
                Debug.LogError("HabitatViewController: HabitatManager is not assigned.", this);
                return;
            }

            if (!habitatManager.TrySwitch(habitatId))
            {
                Debug.LogWarning("Unknown habitat ID: " + habitatId, this);
                return;
            }

            Refresh();
        }

        public void Refresh()
        {
            string activeId = habitatManager != null ? habitatManager.CurrentHabitatId : null;
            Debug.Log($"[HabitatViewController] CurrentHabitatId = '{activeId}'", this);
            var seen = new HashSet<string>(StringComparer.Ordinal);

            foreach (HabitatRoot entry in habitatRoots)
            {
                if (entry == null || entry.root == null) continue;
                string id = entry.habitatId == null ? string.Empty : entry.habitatId.Trim();
                bool registered = habitatManager != null && habitatManager.Contains(id);
                bool shouldShow = id == activeId && (!hideUnregisteredRoots || registered);
                Debug.Log(
                    $"[HabitatViewController] ID='{id}', Active='{activeId}', Registered={registered}, Show={shouldShow}",
                    this
                );
                entry.root.SetActive(shouldShow);

                if (!seen.Add(id))
                    Debug.LogWarning("Duplicate habitat root ID in HabitatViewController: " + id, this);
            }
        }

        private void OnHabitatChanged(string ignored) => Refresh();
    }
}
