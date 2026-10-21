using UnityEngine;
using AquaGarden.Data;
using AquaGarden.Systems;

namespace AquaGarden.Presentation
{
    /// <summary>Generic autonomous pet controller. Visual sprites are stored in PetDefinition.</summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class CatAnimationController : MonoBehaviour
    {
        public enum Facing { Front, Right, Left, Back }
        public enum PetAction { None, Smile, Groom, Crouch, Sleep }
        private enum BrainState { Idle, Moving, Acting }

        [Header("Autonomous behaviour")]
        [SerializeField] private bool autonomous = true;
        [SerializeField, Min(0f)] private float moveSpeed = 0.45f;
        [SerializeField, Min(0.1f)] private float minIdleSeconds = 0.6f;
        [SerializeField, Min(0.1f)] private float maxIdleSeconds = 2f;
        [SerializeField, Min(0.1f)] private float minMoveSeconds = 2f;
        [SerializeField, Min(0.1f)] private float maxMoveSeconds = 5f;
        [SerializeField, Range(0f, 1f)] private float actionChance = 0.15f;
        [SerializeField, Min(0.1f)] private float minActionSeconds = 1.5f;
        [SerializeField, Min(0.1f)] private float maxActionSeconds = 3f;
        [SerializeField] private Vector2 wanderAreaSize = new Vector2(4f, 2f);
        [SerializeField] private bool clampToStartingArea = true;

        [Header("Animation timing")]
        [SerializeField, Min(0.03f)] private float movementFrameInterval = 0.14f;
        [SerializeField, Min(0.1f)] private float petReactionSeconds = 1.5f;
        [SerializeField] private bool useUnscaledTime = false;

        private SpriteRenderer spriteRenderer;
        private PetRegistry petRegistry;
        private PetDefinition petDefinition;
        private string boundPetInstanceId;
        private BrainState state = BrainState.Idle;
        private Facing facing = Facing.Front;
        private PetAction currentAction = PetAction.None;
        private Vector3 homePosition, targetPosition;
        private float stateTimer, frameTimer, petReactionTimer;
        private int movementFrame;

        public Facing CurrentFacing => facing;
        public PetAction CurrentAction => currentAction;
        public bool IsMoving => state == BrainState.Moving;
        public PetDefinition Definition => petDefinition;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            homePosition = transform.position;
            targetPosition = homePosition;
        }

        private void OnEnable()
        {
            TryBindRegistry();
        }

        private void Start()
        {
            TryBindRegistry();
            RefreshPetDefinition();
            if (autonomous) ChooseNextBehaviour();
        }

        private void OnDisable()
        {
            if (petRegistry != null)
                petRegistry.Changed -= HandleRegistryChanged;
        }

        private void TryBindRegistry()
        {
            if (petRegistry == null)
                petRegistry = FindFirstObjectByType<PetRegistry>();

            if (petRegistry == null)
                return;

            petRegistry.Changed -= HandleRegistryChanged;
            petRegistry.Changed += HandleRegistryChanged;
            RefreshPetDefinition();
        }

        private void HandleRegistryChanged()
        {
            RefreshPetDefinition();
        }

        private void RefreshPetDefinition()
        {
            if (petRegistry == null)
                return;

            PetInstanceData currentPet = petRegistry.CurrentPet;
            string instanceId = currentPet != null ? currentPet.instanceId : null;

            if (currentPet == null)
            {
                boundPetInstanceId = null;
                petDefinition = null;
                currentAction = PetAction.None;
                state = BrainState.Idle;
                return;
            }

            if (instanceId == boundPetInstanceId && petDefinition != null)
                return;

            boundPetInstanceId = instanceId;
            if (!petRegistry.TryGetDefinition(currentPet.petTypeId, out petDefinition))
            {
                Debug.LogWarning(
                    "PetDefinition not registered for pet type ID: " + currentPet.petTypeId,
                    this);
                petDefinition = null;
                return;
            }

            homePosition = transform.position;
            targetPosition = homePosition;
            state = BrainState.Idle;
            currentAction = PetAction.None;
            petReactionTimer = 0f;
            movementFrame = 0;
            RefreshVisual();
            if (autonomous)
                ChooseNextBehaviour();
        }

        private void Update()
        {
            if (petRegistry == null)
                TryBindRegistry();

            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (dt <= 0f) return;

            if (petReactionTimer > 0f)
            {
                petReactionTimer -= dt;
                if (petReactionTimer <= 0f)
                {
                    currentAction = PetAction.None;
                    if (autonomous) ChooseNextBehaviour();
                    else { state = BrainState.Idle; ShowIdlePose(); }
                }
                return;
            }

            if (!autonomous) return;
            stateTimer -= dt;

            switch (state)
            {
                case BrainState.Idle:
                    ShowIdlePose();
                    if (stateTimer <= 0f) ChooseNextBehaviour();
                    break;
                case BrainState.Moving:
                    UpdateMovement(dt);
                    if (stateTimer <= 0f || ReachedTarget()) ChooseNextBehaviour();
                    break;
                case BrainState.Acting:
                    if (stateTimer <= 0f) ChooseNextBehaviour();
                    break;
            }
        }

        public void RefreshVisual()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (petDefinition == null) return;
            if (state == BrainState.Idle) ShowIdlePose();
            else if (petDefinition.Icon != null && spriteRenderer.sprite == null)
                spriteRenderer.sprite = petDefinition.Icon;
        }

        private void ChooseNextBehaviour()
        {
            frameTimer = 0f;
            currentAction = PetAction.None;

            if (petDefinition != null && Random.value < actionChance)
            {
                PetAction action = ChooseRandomAutonomousAction();
                Sprite actionSprite = GetActionSprite(action);
                if (actionSprite != null)
                {
                    currentAction = action;
                    state = BrainState.Acting;
                    stateTimer = Random.Range(minActionSeconds, maxActionSeconds);
                    spriteRenderer.sprite = actionSprite;
                    spriteRenderer.flipX = facing == Facing.Left;
                    return;
                }
            }

            if (Random.value < 0.2f)
            {
                state = BrainState.Idle;
                stateTimer = Random.Range(minIdleSeconds, maxIdleSeconds);
                ShowIdlePose();
                return;
            }

            targetPosition = PickMovementTarget();
            UpdateFacingFromTarget(targetPosition);
            state = BrainState.Moving;
            stateTimer = Random.Range(minMoveSeconds, maxMoveSeconds);
            movementFrame = 0;
            ShowMovementFrame();
        }

        private PetAction ChooseRandomAutonomousAction()
        {
            float value = Random.value;
            if (value < 0.45f) return PetAction.Groom;
            if (value < 0.75f) return PetAction.Crouch;
            return PetAction.Sleep;
        }

        private Vector3 PickMovementTarget()
        {
            float halfX = Mathf.Max(0.1f, wanderAreaSize.x * 0.5f);
            float halfY = Mathf.Max(0.1f, wanderAreaSize.y * 0.5f);
            Vector3 candidate = homePosition + new Vector3(
                Random.Range(-halfX, halfX), Random.Range(-halfY, halfY), 0f);
            return new Vector3(candidate.x, candidate.y, homePosition.z);
        }

        private void UpdateMovement(float dt)
        {
            if (petDefinition != null && petDefinition.MovementType == PetMovementType.Stationary)
            {
                transform.position = homePosition;
                state = BrainState.Idle;
                ShowIdlePose();
                return;
            }

            Vector3 next = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * dt);
            if (clampToStartingArea)
            {
                float halfX = wanderAreaSize.x * 0.5f, halfY = wanderAreaSize.y * 0.5f;
                next.x = Mathf.Clamp(next.x, homePosition.x - halfX, homePosition.x + halfX);
                next.y = Mathf.Clamp(next.y, homePosition.y - halfY, homePosition.y + halfY);
            }
            transform.position = next;

            if (facing == Facing.Front || facing == Facing.Back) { ShowIdlePose(); return; }

            frameTimer += dt;
            if (frameTimer >= movementFrameInterval)
            {
                frameTimer = 0f;
                movementFrame = (movementFrame + 1) % 2;
                ShowMovementFrame();
            }
        }

        private bool ReachedTarget() =>
            (transform.position - targetPosition).sqrMagnitude < 0.0025f;

        private void UpdateFacingFromTarget(Vector3 target)
        {
            float dx = target.x - transform.position.x;
            if (Mathf.Abs(dx) > 0.03f) facing = dx >= 0f ? Facing.Right : Facing.Left;
            else facing = target.y >= transform.position.y ? Facing.Back : Facing.Front;
        }

        private void ShowMovementFrame()
        {
            if (petDefinition == null) return;
            Sprite[] frames = petDefinition.WalkFrames;

            if (facing == Facing.Front || facing == Facing.Back ||
                frames == null || frames.Length == 0 ||
                movementFrame >= frames.Length || frames[movementFrame] == null)
            {
                ShowIdlePose();
                return;
            }
            spriteRenderer.sprite = frames[movementFrame];
            spriteRenderer.flipX = facing == Facing.Left;
        }

        private void ShowIdlePose()
        {
            if (petDefinition == null) return;
            Sprite[] poses = petDefinition.SittingPoses;
            int index = facing switch
            {
                Facing.Front => 0,
                Facing.Right => 1,
                Facing.Left => 1,
                Facing.Back => 2,
                _ => 0
            };

            if (poses != null && index < poses.Length && poses[index] != null)
                spriteRenderer.sprite = poses[index];
            else if (petDefinition.Icon != null)
                spriteRenderer.sprite = petDefinition.Icon;

            // The side pose is authored facing right; mirror it when facing left.
            spriteRenderer.flipX = facing == Facing.Left;
        }

        private Sprite GetActionSprite(PetAction action)
        {
            if (petDefinition == null) return null;
            switch (action)
            {
                case PetAction.Groom: return petDefinition.GroomingSprite;
                case PetAction.Crouch: return petDefinition.CrouchSprite;
                case PetAction.Sleep: return petDefinition.SleepSprite;
                default: return null;
            }
        }

        public void ReactToPet()
        {
            if (petDefinition == null || petDefinition.SmileSprite == null) return;
            currentAction = PetAction.Smile;
            spriteRenderer.sprite = petDefinition.SmileSprite;
            spriteRenderer.flipX = facing == Facing.Left;
            petReactionTimer = petReactionSeconds;
            state = BrainState.Acting;
            stateTimer = petReactionTimer;
        }

        public void SetAutonomous(bool enabled)
        {
            autonomous = enabled;
            if (autonomous) ChooseNextBehaviour();
            else
            {
                state = BrainState.Idle;
                currentAction = PetAction.None;
                petReactionTimer = 0f;
                ShowIdlePose();
            }
        }

        public void PlayGrooming() => PlayAction(PetAction.Groom);
        public void PlayCrouch() => PlayAction(PetAction.Crouch);
        public void PlaySleep() => PlayAction(PetAction.Sleep);

        public void PlayAction(PetAction action)
        {
            if (action == PetAction.Smile) return; // smile only on petting
            Sprite actionSprite = GetActionSprite(action);
            if (actionSprite == null) return;
            currentAction = action;
            state = BrainState.Acting;
            stateTimer = Random.Range(minActionSeconds, maxActionSeconds);
            spriteRenderer.sprite = actionSprite;
            spriteRenderer.flipX = facing == Facing.Left;
        }
    }
}
