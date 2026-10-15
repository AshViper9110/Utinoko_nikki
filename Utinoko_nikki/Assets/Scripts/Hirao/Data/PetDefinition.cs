using UnityEngine;

namespace AquaGarden.Data
{
    public enum PetHabitatType { Aquarium, Room, Garden }
    public enum PetMovementType { Swim, Walk, Hop, Fly, Stationary }

    [CreateAssetMenu(fileName = "PetDefinition", menuName = "Aqua Garden/Pet Definition")]
    public sealed class PetDefinition : ScriptableObject
    {
        [Header("Basic")]
        [SerializeField] private string typeId = "pet.example";
        [SerializeField] private string displayName = "New Pet";
        [SerializeField] private PetHabitatType defaultHabitat = PetHabitatType.Room;
        [SerializeField] private PetMovementType movementType = PetMovementType.Stationary;
        [SerializeField] private Sprite icon;

        [Header("Animation sprites")]
        [Tooltip("Order: Front, Right, Left, Back.")]
        [SerializeField] private Sprite[] sittingPoses = new Sprite[4];
        [SerializeField] private Sprite[] walkRightFrames = new Sprite[4];
        [SerializeField] private Sprite[] walkLeftFrames = new Sprite[4];
        [SerializeField] private Sprite smileSprite;
        [SerializeField] private Sprite groomingSprite;
        [SerializeField] private Sprite crouchSprite;
        [SerializeField] private Sprite sleepSprite;

        [Header("Needs and growth")]
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
        public Sprite[] SittingPoses => sittingPoses;
        public Sprite[] WalkRightFrames => walkRightFrames;
        public Sprite[] WalkLeftFrames => walkLeftFrames;
        public Sprite SmileSprite => smileSprite;
        public Sprite GroomingSprite => groomingSprite;
        public Sprite CrouchSprite => crouchSprite;
        public Sprite SleepSprite => sleepSprite;
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
            if (sittingPoses == null || sittingPoses.Length != 4) System.Array.Resize(ref sittingPoses, 4);
            if (walkRightFrames == null || walkRightFrames.Length != 4) System.Array.Resize(ref walkRightFrames, 4);
            if (walkLeftFrames == null || walkLeftFrames.Length != 4) System.Array.Resize(ref walkLeftFrames, 4);
        }
#endif
    }
}
