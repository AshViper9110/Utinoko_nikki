
using UnityEngine;

namespace AquaGarden.Presentation
{
    public class SettingsPanelController : MonoBehaviour
    {
        [Header("設定画面")]
        [SerializeField] private GameObject settingsPanel;

        [Header("開くボタン（任意）")]
        [SerializeField] private GameObject settingsButton;

        private void Awake()
        {
            if (settingsPanel == null)
            {
                Debug.LogError(
                    "SettingsPanel が設定されていません。",
                    this);
                return;
            }

            settingsPanel.SetActive(false);
        }

        // 設定ボタンから呼び出す
        public void ToggleSettings()
        {
            if (settingsPanel == null)
                return;

            bool show = !settingsPanel.activeSelf;
            settingsPanel.SetActive(show);

            // 設定画面表示中は開くボタンを隠したい場合に使用
            if (settingsButton != null)
                settingsButton.SetActive(!show);
        }

        // 閉じるボタンから呼び出す
        public void CloseSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);

            if (settingsButton != null)
                settingsButton.SetActive(true);
        }

        // 設定画面を開く専用メソッド
        public void OpenSettings()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(true);

            if (settingsButton != null)
                settingsButton.SetActive(false);
        }
    }
}
