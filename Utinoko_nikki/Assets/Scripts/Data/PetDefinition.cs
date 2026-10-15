using UnityEngine;

namespace AquaGarden.Data
{
    public enum PetHabitatType { Aquarium, Room, Garden }
    public enum PetMovementType { Swim, Walk, Hop, Fly, Stationary }

    [CreateAssetMenu(fileName = "PetDefinition", menuName = "Aqua Garden/Pet Definition")]
    public sealed class PetDefinition : ScriptableObject
    {
        [SerializeField] private string typeId = "pet.example";
        [SerializeField] private string displayName = "New Pet";
        [SerializeField] private PetHabitatType defaultHabitat = PetHabitatType.Room;
        [SerializeField] private PetMovementType movementType = PetMovementType.Stationary;
        [SerializeField] private Sprite icon;
        [SerializeField, Min(0f)] private float initialHunger = 100f;
        [SerializeField, Min(0f)] private float hungerDecayPerGameSecond = 0f;
        [SerializeField, Min(0f)] private float growthPerCareAction = 1f;
        [SerializeField] private bool usesHunger = true;
        [SerializeField] private bool usesAffection = true;

        public string TypeId => typeId;
        public string DisplayName => displayName;
        public PetHabitatType DefaultHabitat => defaultHabitat;
        public PetMovementType MovementType => movementType;
        public Sprite Icon => icon;
        public float InitialHunger => Mathf.Clamp(initialHunger, 0f, 100f);
        public float HungerDecayPerGameSecond => hungerDecayPerGameSecond;
        public float GrowthPerCareAction => growthPerCareAction;
        public bool UsesHunger => usesHunger;
        public bool UsesAffection => usesAffection;

#if UNITY_EDITOR
        private void OnValidate()
        {
            typeId = typeId == null ? string.Empty : typeId.Trim();
            displayName = displayName == null ? string.Empty : displayName.Trim();
            initialHunger = Mathf.Clamp(initialHunger, 0f, 100f);
            hungerDecayPerGameSecond = Mathf.Max(0f, hungerDecayPerGameSecond);
            growthPerCareAction = Mathf.Max(0f, growthPerCareAction);
        }
#endif
    }
}
