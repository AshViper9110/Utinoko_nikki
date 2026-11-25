
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AquaGarden.Systems
{
    public class DailyCareManager : MonoBehaviour
    {
        public static DailyCareManager Instance { get; private set; }

        [Header("Scene Settings")]
        [SerializeField] private string videoSceneName = "VideoScene";
        [SerializeField] private string gameSceneName = "GameScene";

        [Header("Reward Settings")]
        [SerializeField] private int[] rewardItemIds = { 1, 2 };

        private const string CompletedDateKey = "DailyCare_CompletedDate";
        private const string RewardItemKey = "DailyCare_RewardItem";

        public bool IsCompleted { get; private set; }
        public bool IsUnlocked => IsCompleted;
        public int TodayRewardItemId { get; private set; }

        public event Action OnStatusChanged;
        public event Action<int> OnRewardReceived;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

#if DEBUG
            PlayerPrefs.DeleteKey("DailyCare_CompletedDate");
            PlayerPrefs.DeleteKey("DailyCare_RewardItem");
            PlayerPrefs.Save();
#endif

            RefreshStatus();

            StartCoroutine(DailyResetWatcher());
        }

        private string GetToday()
        {
            return DateTime.UtcNow
                .AddHours(9)
                .ToString("yyyy-MM-dd");
        }

        public void RefreshStatus()
        {
            string today = GetToday();

            string savedDate =
                PlayerPrefs.GetString(CompletedDateKey, "");

            IsCompleted = savedDate == today;

            TodayRewardItemId = IsCompleted
                ? PlayerPrefs.GetInt(RewardItemKey, 0)
                : 0;

            OnStatusChanged?.Invoke();
        }

        public void OpenVideoScene()
        {
            if (IsCompleted) return;

            SceneManager.LoadScene(videoSceneName);
        }

        public void OpenGameScene()
        {
            if (IsCompleted) return;

            SceneManager.LoadScene(gameSceneName);
        }

        public void CompleteTest()
        {
            RefreshStatus();

            if (IsCompleted)
            {
                Debug.Log("本日のタスクは達成済みです。");
                return;
            }

            if (rewardItemIds == null || rewardItemIds.Length == 0)
            {
                Debug.LogError("報酬アイテムが設定されていません。");
                return;
            }

            int index = UnityEngine.Random.Range(
                0,
                rewardItemIds.Length
            );

            int rewardId = rewardItemIds[index];

            string today = GetToday();

            PlayerPrefs.SetString(CompletedDateKey, today);
            PlayerPrefs.SetInt(RewardItemKey, rewardId);
            PlayerPrefs.Save();

            IsCompleted = true;
            TodayRewardItemId = rewardId;

            Debug.Log($"タスク達成！報酬ID: {rewardId}");

            OnRewardReceived?.Invoke(rewardId);
            OnStatusChanged?.Invoke();
        }

        public void CompleteAndReturn()
        {
            CompleteTest();

            SceneManager.LoadScene(
                PlayerPrefs.GetString("AquariumSceneName", "AquaGardenScene")
            );
        }

        // アプリ復帰時にも日付を再確認
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
                RefreshStatus();
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause)
                RefreshStatus();
        }

        private System.Collections.IEnumerator DailyResetWatcher()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(10f);

                string today = GetToday();

                string savedDate =
                    PlayerPrefs.GetString(CompletedDateKey, "");

                if (savedDate != "" && savedDate != today)
                {
                    RefreshStatus();
                    Debug.Log("日付が変わりました。今日のタスクを再表示します。");
                }
            }
        }
    }
}
