
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AquaGarden.Systems;

namespace AquaGarden.Presentation
{
    public class DailyCarePanelController : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject taskPanel;

        [Header("Buttons")]
        [SerializeField] private Button videoButton;
        [SerializeField] private Button gameButton;

        [Header("Text")]
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text rewardText;

        private void OnEnable()
        {
            if (DailyCareManager.Instance == null)
                return;

            DailyCareManager.Instance.OnStatusChanged += RefreshUI;

            RefreshUI();
        }

        private void OnDisable()
        {
            if (DailyCareManager.Instance != null)
            {
                DailyCareManager.Instance.OnStatusChanged -= RefreshUI;
            }
        }

        private void Start()
        {
            RefreshUI();
        }

        public void RefreshUI()
        {
            var manager = DailyCareManager.Instance;

            if (manager == null)
                return;

            bool completed = manager.IsCompleted;

            if (taskPanel != null)
                taskPanel.SetActive(!completed);

            if (videoButton != null)
                videoButton.interactable = !completed;

            if (gameButton != null)
                gameButton.interactable = !completed;

            if (statusText != null)
            {
                statusText.text = completed
                    ? "今日のタスク達成済み！"
                    : "Video または Game をクリアしてください";
            }

            if (rewardText != null)
            {
                rewardText.text = completed
                    ? $"獲得アイテム ID: {manager.TodayRewardItemId}"
                    : "";
            }
        }

        public void OnClickVideo()
        {
            DailyCareManager.Instance?.OpenVideoScene();
        }

        public void OnClickGame()
        {
            DailyCareManager.Instance?.OpenGameScene();
        }
    }
}
