using UnityEngine;

namespace AquaGarden.Data
{
    [CreateAssetMenu(fileName = "HabitatDefinition", menuName = "Aqua Garden/Habitat Definition")]
    public sealed class HabitatDefinition : ScriptableObject
    {
        [SerializeField] private string habitatId = "Room";
        [SerializeField] private string displayName = "Room";
        [SerializeField] private PetHabitatType habitatType = PetHabitatType.Room;
        [SerializeField] private Sprite icon;

        public string HabitatId => habitatId;
        public string DisplayName => displayName;
        public PetHabitatType HabitatType => habitatType;
        public Sprite Icon => icon;

#if UNITY_EDITOR
        private void OnValidate()
        {
            habitatId = habitatId == null ? string.Empty : habitatId.Trim();
            displayName = displayName == null ? string.Empty : displayName.Trim();
        }
#endif
    }
}
