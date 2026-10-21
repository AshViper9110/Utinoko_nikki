
using System;
using AquaGarden.Data;
using AquaGarden.Systems;
using UnityEngine;

/// <summary>
/// ペットのお世話、満腹度、なかよし度、成長を管理する。
/// </summary>
public sealed class PetCareSystem : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private PetRegistry petRegistry;

    [Header("ごはん")]
    [SerializeField, Min(0f)]
    private float defaultFeedAmount = 25f;

    [Header("なでる")]
    [SerializeField, Min(0f)]
    private float defaultPetAffection = 8f;

    [Header("一緒に遊ぶ")]
    [SerializeField, Min(0f)]
    private float defaultPlayAffection = 12f;

    [SerializeField, Min(0f)]
    private float playHungerCost = 10f;

    [Header("時間経過")]
    [SerializeField]
    private bool needsDecayEnabled = true;

    [SerializeField]
    private bool useUnscaledTime = false;

    [SerializeField, Min(0f)]
    private float stateUpdateInterval = 1f;

    [Header("成長")]
    [Tooltip("満腹度100のときの、1ゲーム秒あたりの成長値")]
    [SerializeField, Min(0f)]
    private float growthPerGameSecond = 0.001f;

    public event Action<PetInstanceData> StateChanged;

    public PetInstanceData CurrentPet =>
        petRegistry != null ? petRegistry.CurrentPet : null;

    private float stateUpdateTimer;

    private void Awake()
    {
        if (petRegistry == null)
            petRegistry = FindFirstObjectByType<PetRegistry>();

        if (petRegistry == null)
        {
            Debug.LogError(
                "PetCareSystem: PetRegistry が見つかりません。",
                this);
        }
    }

    private void Update()
    {
        if (!needsDecayEnabled || petRegistry == null)
            return;

        PetInstanceData pet = petRegistry.CurrentPet;

        if (pet == null)
            return;

        if (!petRegistry.TryGetDefinition(
                pet.petTypeId,
                out PetDefinition definition))
            return;

        float dt = useUnscaledTime
            ? Time.unscaledDeltaTime
            : Time.deltaTime;

        if (dt <= 0f)
            return;

        // ペットの年齢・経過時間
        pet.ageGameSeconds += dt;

        // 時間経過で満腹度が減る
        if (definition.UsesHunger &&
            definition.HungerDecayPerGameSecond > 0f)
        {
            pet.hunger = Mathf.Clamp(
                pet.hunger -
                definition.HungerDecayPerGameSecond * dt,
                0f,
                100f);
        }

        // 満腹度に応じて成長する。
        // 満腹度100なら成長速度100%、満腹度50なら50%。
        float hungerRatio = definition.UsesHunger
            ? Mathf.Clamp01(pet.hunger / 100f)
            : 1f;

        pet.growth = Mathf.Max(
            0f,
            pet.growth +
            growthPerGameSecond * hungerRatio * dt);

        // なかよし度は時間経過では減らさない
        stateUpdateTimer += dt;

        float interval = Mathf.Max(0.1f, stateUpdateInterval);

        if (stateUpdateTimer >= interval)
        {
            stateUpdateTimer %= interval;
            NotifyStateChanged(pet);
        }
    }

    /// <summary>ごはんをあげる。</summary>
    public bool Feed()
    {
        return Feed(defaultFeedAmount);
    }

    public bool Feed(float amount)
    {
        PetInstanceData pet = CurrentPet;

        if (pet == null || petRegistry == null || amount <= 0f)
            return false;

        if (!petRegistry.TryGetDefinition(
                pet.petTypeId,
                out PetDefinition definition))
            return false;

        if (!definition.UsesHunger)
            return false;

        float previous = pet.hunger;

        pet.hunger = Mathf.Clamp(
            pet.hunger + amount,
            0f,
            100f);

        if (Mathf.Approximately(previous, pet.hunger))
        {
            Debug.Log("[Feed] おなかいっぱいです！", this);
            return true;
        }

        NotifyStateChanged(pet);

        Debug.Log(
            $"ごはんをあげました！ 満腹度: {previous:F1} → {pet.hunger:F1}",
            this);

        return true;
    }

    /// <summary>ペットをなでる。なかよし度が増える。</summary>
    public bool Pet()
    {
        return Pet(defaultPetAffection);
    }

    public bool Pet(float affectionAmount)
    {
        PetInstanceData pet = CurrentPet;

        if (pet == null || petRegistry == null || affectionAmount <= 0f)
            return false;

        if (!petRegistry.TryGetDefinition(
                pet.petTypeId,
                out PetDefinition definition))
            return false;

        if (!definition.UsesAffection)
            return false;

        float previous = pet.affection;

        pet.affection = Mathf.Clamp(
            pet.affection + affectionAmount,
            0f,
            100f);

        if (Mathf.Approximately(previous, pet.affection))
        {
            Debug.Log("[Pet] なかよし度は最大です！", this);
            return true;
        }

        NotifyStateChanged(pet);

        Debug.Log(
            $"なでました！ なかよし度: {previous:F1} → {pet.affection:F1}",
            this);

        return true;
    }

    /// <summary>
    /// 一緒に遊ぶ。なかよし度が増え、満腹度が減る。
    /// </summary>
    public bool Play()
    {
        return Play(defaultPlayAffection);
    }

    public bool Play(float affectionAmount)
    {
        PetInstanceData pet = CurrentPet;

        if (pet == null || petRegistry == null || affectionAmount <= 0f)
            return false;

        if (!petRegistry.TryGetDefinition(
                pet.petTypeId,
                out PetDefinition definition))
            return false;

        if (!definition.UsesAffection)
            return false;

        if (definition.UsesHunger && pet.hunger <= 0f)
        {
            Debug.Log("[Play] おなかがすいていて遊べません！", this);
            return false;
        }

        float previousAffection = pet.affection;
        float previousHunger = pet.hunger;

        // 遊ぶとなかよし度が増える
        pet.affection = Mathf.Clamp(
            pet.affection + affectionAmount,
            0f,
            100f);

        // 遊ぶと満腹度が減る
        if (definition.UsesHunger)
        {
            pet.hunger = Mathf.Clamp(
                pet.hunger - playHungerCost,
                0f,
                100f);
        }

        bool affectionChanged =
            !Mathf.Approximately(previousAffection, pet.affection);

        bool hungerChanged =
            !Mathf.Approximately(previousHunger, pet.hunger);

        if (!affectionChanged && !hungerChanged)
        {
            Debug.Log("[Play] 状態が変化しませんでした。", this);
            return false;
        }

        NotifyStateChanged(pet);

        Debug.Log(
            $"一緒に遊びました！ なかよし度: {previousAffection:F1} → {pet.affection:F1}, " +
            $"満腹度: {previousHunger:F1} → {pet.hunger:F1}",
            this);

        return true;
    }

    private void NotifyStateChanged(PetInstanceData pet)
    {
        if (petRegistry != null)
            petRegistry.NotifyChanged();

        StateChanged?.Invoke(pet);
    }
}
